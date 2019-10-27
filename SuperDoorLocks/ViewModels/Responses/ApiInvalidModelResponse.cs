using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace SuperDoorLocks.ViewModels.Responses
{
    public class ApiInvalidModelResponse : ApiResponse
    {
        public IEnumerable<string> Errors { get; }

        public ApiInvalidModelResponse(ModelStateDictionary modelState)
            : base(HttpStatusCode.BadRequest)
        {
            if (modelState.IsValid)
            {
                throw new ArgumentException("Model state must be invalid", nameof(modelState));
            }

            Errors = modelState.SelectMany(x => x.Value.Errors)
                .Select(x => x.ErrorMessage).ToArray();
        }
    }
}
