namespace DoorDash.Models
{
    public class WebhookSettings
    {
        public required string DeveloperID { get; set; }
        public required string KeyID { get; set; }
        public required string SigningSecret { get; set; }
    }
}
