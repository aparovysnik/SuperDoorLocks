using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SuperDoorLocks;
using SuperDoorLocks.Services;
using SuperDoorLocks.ViewModels;
using SuperDoorLocks.ViewModels.Responses;

namespace WebApiJwt.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IIdentityService _identityService;

        public AccountController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IIdentityService identityService,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _identityService = identityService;
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiInvalidModelResponse(ModelState));
            }
            var result = await _identityService.LoginUser(model.Username, model.Password);
            if (!result.Success)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest, result.Errors));
            }

            return Ok(new ApiAuthenticationSuccessResponse(result.Token));
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = UserRoles.ADMIN)]
        public async Task<ActionResult> Register([FromBody] RegisterViewModel model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(new ApiInvalidModelResponse(ModelState));
            }
            var result = await _identityService.RegisterUser(model.Username, model.Password, model.Roles);
            if(!result.Success)
            {
                return BadRequest(new ApiResponse(HttpStatusCode.BadRequest, result.Errors));
            }

            return Ok(new ApiAuthenticationSuccessResponse(result.Token));
        }

        
    }
}