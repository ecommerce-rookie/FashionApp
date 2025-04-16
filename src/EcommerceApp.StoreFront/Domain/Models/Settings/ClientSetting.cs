using System.Text.Json;

namespace StoreFront.Domain.Models.Settings
{
    public class ClientSetting
    {
        public string ApiEndpoint { get; set; } = string.Empty;
        public string NamingPolicy { get; set; } = string.Empty;
    }
}
