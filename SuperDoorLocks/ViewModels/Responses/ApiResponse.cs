using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace SuperDoorLocks.ViewModels.Responses
{
    public class ApiResponse
    {
        public int StatusCode { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> Message { get; }

        public ApiResponse(HttpStatusCode statusCode, IEnumerable<string> message = null)
        {
            StatusCode = (int)statusCode;
            Message = message ?? new List<string>() { statusCode.ToString() };
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
