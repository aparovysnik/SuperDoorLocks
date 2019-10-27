using System.Net;

namespace SuperDoorLocks.ViewModels.Responses
{
    public class ApiGetResponse<T>: ApiResponse
    {
        public T Item { get; }

        public ApiGetResponse(T item) : base(HttpStatusCode.OK)
        {
            Item = item;
        }
    }
}
