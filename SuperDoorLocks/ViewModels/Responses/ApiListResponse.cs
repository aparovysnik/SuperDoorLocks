using System.Collections.Generic;
using System.Net;

namespace SuperDoorLocks.ViewModels.Responses
{
    public class ApiListResponse<T>: ApiResponse
    {
        public ApiListResponse(List<T> items): base(HttpStatusCode.OK)
        {
            Items = items;
        }

        public List<T> Items { get; }
    }
}
