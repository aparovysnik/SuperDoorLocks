using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SuperDoorLocks.ViewModels.Responses
{
    public class ApiCreatedResponse: ApiResponse
    {
        public ApiCreatedResponse(string id) : base(System.Net.HttpStatusCode.OK)
        {
            Id = id;
        }
        public string Id { get; }
    }
}
