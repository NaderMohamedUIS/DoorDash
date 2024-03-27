using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using DoorDash.Models;
using DoorDash.DTOs;
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

            var token = GetToken();
            Console.WriteLine(token);


            var jsonContent = JsonSerializer.Serialize(new
            {
                external_delivery_id = "D-12346",
                pickup_address = "901 Market Street 6th Floor San Francisco, CA 94103",
                pickup_business_name = "Wells Fargo SF Downtown",
                pickup_phone_number = "+16505555555",
                pickup_instructions = "Enter gate code 1234 on the callbox.",
                pickup_reference_tag = "Order number 61",
                dropoff_address = "901 Market Street 6th Floor San Francisco, CA 94103",
                dropoff_business_name = "Wells Fargo SF Downtown",
                dropoff_phone_number = "+16505555555",
                dropoff_instructions = "Enter gate code 1234 on the callbox."
            });
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            //var result = await client.PostAsync("https://openapi.doordash.com/drive/v2/deliveries", content);

            //var status = result.StatusCode;
            //var resultString = await result.Content.ReadAsStringAsync();
            var result = await client.GetAsync($"https://openapi.doordash.com/drive/v2/deliveries/D-123456");

            var status = result.StatusCode;
            var resultString = await result.Content.ReadAsStringAsync();

            Console.WriteLine(status);
            Console.WriteLine(resultString);
            return Ok(resultString);
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuote()
        {
            var jsonContent = JsonSerializer.Serialize(new QuoteDTO
            {
                // Assign values from the provided JSON
                external_delivery_id = "D-1763",
                locale = "en-US",
                order_fulfillment_method = "standard",
                origin_facility_id = "MERCHANTA-CA-1",
                pickup_address = "901 Market Street 6th Floor San Francisco, CA 94103",
                pickup_business_name = "Wells Fargo SF Downtown",
                pickup_phone_number = "+16505555555",
                pickup_instructions = "Go to the bar for pick up.",
                pickup_reference_tag = "Order number 61",
                pickup_external_business_id = "ase-243-dzs",
                pickup_external_store_id = "",
                pickup_verification_metadata = new PickupVerificationMetadataDTO
                {
                    verification_type = "SCAN_BARCODE",
                    verification_code = "12345",
                    verification_format = "CODE_39"
                },
                dropoff_address = "901 Market Street 6th Floor San Francisco, CA 94103",
                dropoff_business_name = "The Avery Condominium",
                dropoff_location = new DropoffLocationDTO
                {
                    lat = 123.1312343,
                    lng = -37.2144343
                },
                dropoff_phone_number = "+16505555555",
                dropoff_instructions = "Enter gate code 1234 on the callbox.",
                dropoff_contact_given_name = "John",
                dropoff_contact_family_name = "Doe",
                dropoff_contact_send_notifications = true,
                dropoff_options = new DropoffOptionsDTO
                {
                    signature = "required",
                    id_verification = "required",
                    proof_of_delivery = "photo_required",
                    catering_setup = "required"
                },
                shopping_options = new ShoppingOptionsDTO
                {
                    payment_method = "red_card",
                    payment_barcode = "12345",
                    payment_gift_cards = new List<string> { "123443434", "123443435" },
                    ready_for_pickup_by = DateTime.Parse("2018-08-22T17:20:28Z"),
                    dropoff_contact_loyalty_number = "1234-5678-9876-5432-1"
                },
                order_value = 1999,
                items = new List<ItemDTO>
                {
                    new ItemDTO
                    {
                        name = "Mega Bean and Cheese Burrito",
                        description = "Mega Burrito contains the biggest beans of the land with extra cheese.",
                        quantity = 2,
                        external_id = "123-123443434b",
                        external_instance_id = 12,
                        volume = 5.3,
                        weight = 2.8,
                        length = 2.8,
                        width = 2.8,
                        height = 2.8,
                        price = 1000,
                        barcode = 12342830041,
                        item_options = new ItemOptionsDTO
                        {
                            substitute_item_ids = new System.Collections.Generic.List<string> { "123443434", "123443435" },
                            weight_unit = "oz",
                            substitution_preference = "refund"
                        }
                    }
                },
                pickup_time = DateTime.Parse("2018-08-22T17:20:28Z"),
                dropoff_time = DateTime.Parse("2018-08-22T17:20:28Z"),
                pickup_window = new TimeWindowDTO
                {
                    start_time = DateTime.Parse("2018-08-22T17:20:28Z"),
                    end_time = DateTime.Parse("2018-08-22T17:20:28Z")
                },
                dropoff_window = new TimeWindowDTO
                {
                    start_time = DateTime.Parse("2018-08-22T17:20:28Z"),
                    end_time = DateTime.Parse("2018-08-22T17:20:28Z")
                },
                contactless_dropoff = false,
                action_if_undeliverable = "return_to_pickup",
                tip = 500,
                order_contains = new OrderContainsDTO
                {
                    alcohol = false,
                    pharmacy_items = false,
                    age_restricted_pharmacy_items = false
                },
                dasher_allowed_vehicles = new List<string> { "car", "bicycle", "walking" },
                dropoff_requires_signature = false,
                promotion_id = "ee680b87-0016-496e-ac3c-d3f33ab54c1c",
                dropoff_cash_on_delivery = 1999,
            });
            var token = GetToken();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var result = await _httpClient.PostAsync("https://openapi.doordash.com/drive/v2/quotes", content);
            var status = result.StatusCode;
            var resultString = await result.Content.ReadAsStringAsync();

            Console.WriteLine(status);
            Console.WriteLine(resultString);
            return Ok(result);
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
    }
}
