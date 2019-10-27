using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SuperDoorLocks.DomainModels;
using SuperDoorLocks.DomainModels.ServiceResultModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SuperDoorLocks.Services
{
    public class DoorsService : IDoorsService
    {
        private RoleManager<IdentityRole> _rolesManager;
        private DoorsDBContext _dbContext;
        private IConfiguration _configuration;
        private IServiceScopeFactory _serviceScopeFactory;

        public DoorsService(RoleManager<IdentityRole> rolesManager, DoorsDBContext dbContext,
            IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            _rolesManager = rolesManager;
            _dbContext = dbContext;
            _configuration = configuration;
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task<CreateActionOutcome> CreateDoor(string type, IEnumerable<string> roleNames)
        {
            var outcome = new CreateActionOutcome();

            try
            {
                var roles = new List<IdentityRole>();
                foreach (var roleName in roleNames)
                {
                    var role = await _rolesManager.FindByNameAsync(roleName);
                    if (role == null)
                    {
                        outcome.Errors.Add($"Role {roleName} does not exist.");
                    }
                    else
                    {
                        roles.Add(role);
                    }
                }

                if (outcome.Errors.Any())
                {
                    return outcome;
                }
                var door = new Door { Type = type, PermittedRoles = roles };
                _dbContext.Doors.Add(door);
                _dbContext.SaveChanges();
                outcome.Id = door.Id.ToString();
                return outcome;
            }
            catch (Exception ex)
            {
                outcome.Errors.Add(ex.Message);
                return outcome;
            }
        }

        public AuthorizationRequiredActionOutcome OpenDoor(int id, ClaimsPrincipal user)
        {
            var outcome = new AuthorizationRequiredActionOutcome();

            try
            {
                var door = _dbContext.Doors
                    .Where(x => x.Id == id)
                    .Include(x => x.PermittedRoles)
                    .SingleOrDefault();

                if(door == null)
                {
                    outcome.Errors.Add($"Door with id {id} does not exist.");
                    return outcome;
                }
                if (door.PermittedRoles.Any())
                {
                    var isOperationPermitted = false;
                    foreach (var role in door.PermittedRoles)
                    {
                        if (user.IsInRole(role.Name))
                        {
                            isOperationPermitted = true;
                            break;
                        }
                    }
                    if (!isOperationPermitted)
                    {
                        outcome.AuthErrors.Add("User has insufficient permissions to execute this operation.");
                        return outcome;
                    }
                }
                
                if(door.IsOpen)
                {
                    outcome.Errors.Add($"Door {id} is already open.");
                    return outcome;
                }
                door.IsOpen = true;
                _dbContext.SaveChanges();

                //TODO strongly typed configuration would be better.
                int doorCloseTimeout;
                int.TryParse(_configuration["DoorCloseTimeoutSeconds"], out doorCloseTimeout);

                //Door will close automatically
                Task.Delay(doorCloseTimeout * 1000).ContinueWith(_ => CloseDoor(id));
                return outcome;
            }
            catch (Exception ex)
            {
                outcome.Errors.Add(ex.Message);
                return outcome;
            }
        }

        private void CloseDoor(int id)
        {
            try
            {
                using (var serviceScope = _serviceScopeFactory.CreateScope())
                {
                    var taskDbContext = serviceScope.ServiceProvider.GetService<DoorsDBContext>();
                    var door = taskDbContext.Doors
                        .Where(x => x.Id == id)
                        .Include(x => x.PermittedRoles)
                        .SingleOrDefault();

                    if (door == null)
                    {
                        return;
                    }

                    door.IsOpen = false;
                    taskDbContext.SaveChanges();
                }
            }
            catch (Exception _)
            {
                //Log error
            }
        }

        public ListActionOutcome<Door> ListDoors()
        {
            var outcome = new ListActionOutcome<Door>();
            try
            {
                outcome.Items = _dbContext.Doors.Include(x => x.PermittedRoles).ToList();
                return outcome;
            }
            catch (Exception ex)
            {
                outcome.Errors.Add(ex.Message);
                return outcome;
            };
        }

        public GetActionOutcome<Door> GetDoor(int id)
        {
            var outcome = new GetActionOutcome<Door>();
            try
            {
                outcome.Item = _dbContext.Doors
                    .Where(door => door.Id == id)
                    .Include(x => x.PermittedRoles)
                    .SingleOrDefault();

                return outcome;
            }
            catch (Exception ex)
            {
                outcome.Errors.Add(ex.Message);
                return outcome;
            };
        }
    }
}
