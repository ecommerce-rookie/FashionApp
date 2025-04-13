using System.Net;
using System.Text.Json;

namespace Domain.Models.Common
{
    public class APIResponse<T>
    {
        public HttpStatusCode Status { get; set; }
        public string? Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public bool IsSuccess => Status == HttpStatusCode.OK || Status == HttpStatusCode.Created || Status == HttpStatusCode.Accepted;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }

    public class APIResponse : APIResponse<object>
    {
        public APIResponse() { }
    }

}
