using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperDoorLocks.Services;
using SuperDoorLocks.ViewModels;
using SuperDoorLocks.ViewModels.Responses;

namespace SuperDoorLocks.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DoorsController : Controller
    {
        private IDoorsService _doorsService;

        public DoorsController(IDoorsService doorsService)
        {
            _doorsService = doorsService;
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.ADMIN)]
        public ActionResult Get()
        {
            var result = _doorsService.ListDoors();
            if (!result.Success)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest, result.Errors));
            }

            return Ok(
                new ApiListResponse<DoorViewModel>(
                    result.Items.Select(x => new DoorViewModel(x.Type, x.IsOpen,
                        x.PermittedRoles.Select(role => role.Name).ToList())
                    ).ToList()
                )
            );
        }

        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.ADMIN)]
        public ActionResult List(int id)
        {
            var result = _doorsService.GetDoor(id);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest, result.Errors));
            }

            return Ok(
                new ApiGetResponse<DoorViewModel>(
                    new DoorViewModel(result.Item.Type, result.Item.IsOpen,
                        result.Item.PermittedRoles.Select(role => role.Name).ToList()
                    )
                )
            );
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.ADMIN)]
        public async Task<ActionResult> Post([FromBody]CreateDoorViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiInvalidModelResponse(ModelState));
            }

            var result = await _doorsService.CreateDoor(model.Type, model.PermittedRoles);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest, result.Errors));
            }

            return Ok(new ApiCreatedResponse(result.Id));

        }

        [HttpPut("{id}")]
        public ActionResult Open(int id)
        {
            var result = _doorsService.OpenDoor(id, User);

            if (!result.Success)
            {
                if (result.AuthErrors.Any())
                {
                    return Forbid();
                }
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest, result.Errors));
            }

            return Ok(new ApiResponse(HttpStatusCode.OK));
        }
    }
}
