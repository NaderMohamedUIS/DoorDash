using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using DoorDash.Models;
using DoorDash.DTOs;
using System.Text.Json.Serialization;
namespace DoorDash.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WebHookController : ControllerBase
    {
        private readonly IOptions<WebhookSettings> _webhookSettings;
        private readonly HttpClient _httpClient = new HttpClient();
        public WebHookController(IOptions<WebhookSettings> webhookSettings)
        {
            _webhookSettings = webhookSettings;
        }
        [HttpPost]
        public async Task<IActionResult> Index()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetDeliveryById(string id)
        {
            var token = GetToken();
            var result = await CallDoorDashApi($"https://openapi.doordash.com/drive/v2/deliveries/{id}", token);
            return await HandleHttpResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuote(QuoteDTO quoteDTO)
        {
            var token = GetToken();
            var jsonContent = SerializeToJson(quoteDTO);
            var result = await PostToDoorDashApi("https://openapi.doordash.com/drive/v2/quotes", jsonContent, token);
            return await HandleHttpResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDelivery(DeliveryDto deliveryDto)
        {
            var token = GetToken();
            var jsonContent = SerializeToJson(deliveryDto);
            var result = await PostToDoorDashApi("https://openapi.doordash.com/drive/v2/deliveries", jsonContent, token);
            return await HandleHttpResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusiness(BusinessDto businessDto)
        {
            var token = GetToken();
            var jsonContent = SerializeToJson(businessDto);
            var result = await PostToDoorDashApi("https://openapi.doordash.com/developer/v1/businesses", jsonContent, token);
            return await HandleHttpResponse(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStore(StoreDto storeDto, string externalBusinessId)
        {
            var token = GetToken();
            var jsonContent = SerializeToJson(storeDto);
            var result = await PostToDoorDashApi($"https://openapi.doordash.com/developer/v1/businesses/{externalBusinessId}/stores", jsonContent, token);
            return await HandleHttpResponse(result);
        }


        private async Task<HttpResponseMessage> CallDoorDashApi(string url, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return await _httpClient.GetAsync(url);
        }

        private async Task<HttpResponseMessage> PostToDoorDashApi(string url, string jsonContent, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            return await _httpClient.PostAsync(url, content);
        }

        private async Task<IActionResult> HandleHttpResponse(HttpResponseMessage result)
        {
            var resultString = await result.Content.ReadAsStringAsync();
            if (result.IsSuccessStatusCode)
            {
                return Ok(resultString);
            }
            else
            {
                return StatusCode((int)result.StatusCode, resultString);
            }
        }

        private string GetToken()
        {
            var decodedSecret = Base64UrlEncoder.DecodeBytes(_webhookSettings.Value.SigningSecret);
            var securityKey = new SymmetricSecurityKey(decodedSecret);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(credentials);
            header["dd-ver"] = "DD-JWT-V1";

            var payload = new JwtPayload(
                issuer: _webhookSettings.Value.DeveloperID,
                audience: "doordash",
                claims: new List<Claim> { new Claim("kid", _webhookSettings.Value.KeyID) },
                notBefore: null,
                expires: DateTime.UtcNow.AddSeconds(300),
                issuedAt: DateTime.UtcNow);

            var securityToken = new JwtSecurityToken(header, payload);
            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            return token;
        }

        private string SerializeToJson(object obj)
        {
            var jsonSerializerOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            return JsonSerializer.Serialize(obj, jsonSerializerOptions);
        }
    }
}
