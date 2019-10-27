using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SuperDoorLocks.DomainModels.ServiceResultModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SuperDoorLocks.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;

        public IdentityService(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<AuthenticationOutcome> RegisterUser(string username, string password, IEnumerable<string> roles)
        {
            var outcome = new AuthenticationOutcome();
            try
            {
                if (await _userManager.FindByNameAsync(username) != null)
                {
                    outcome.Errors.Add("User with the specified username already exists.");
                    return outcome;
                }

                var user = new IdentityUser
                {
                    UserName = username
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    foreach (var role in roles)
                    {
                        if (await _roleManager.RoleExistsAsync(role))
                        {
                            _userManager.AddToRoleAsync(user, role).Wait();
                        }
                    }
                    var token = await GenerateJwtToken(username, user);
                    outcome.Token = token;
                    return outcome;
                }
                outcome.Errors.AddRange(result.Errors.Select(x => x.Description).ToList());
                return outcome;
            }
            catch (Exception _)
            {
                outcome.Errors.Add("Failed to authenticate user.");
                return outcome;
            }
        }

        public async Task<AuthenticationOutcome> LoginUser(string username, string password)
        {
            var outcome = new AuthenticationOutcome();

            try
            {
                var result = await _signInManager.PasswordSignInAsync(username, password, false, false);

                if (result.Succeeded)
                {
                    var user = _userManager.Users.SingleOrDefault(r => r.UserName == username);
                    var token = await GenerateJwtToken(username, user);

                    return new AuthenticationOutcome
                    {
                        Token = token
                    };
                }

                outcome.Errors.Add("Failed to authenticate user.");
                return outcome;
            } catch (Exception _)
            {
                outcome.Errors.Add("Failed to authenticate user.");
                return outcome;
            }
        }

        private async Task<string> GenerateJwtToken(string username, IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            var roles = await _userManager.GetRolesAsync(user);

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token");
            claimsIdentity.AddClaims(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claimsIdentity.Claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
