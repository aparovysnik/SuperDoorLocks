using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SuperDoorLocks.ViewModels.Responses
{
    public class ApiAuthenticationSuccessResponse : ApiResponse
    {
        public string Token { get; }

        public ApiAuthenticationSuccessResponse(string token): base(HttpStatusCode.OK)
        {
            Token = token;
        }
    }
}
