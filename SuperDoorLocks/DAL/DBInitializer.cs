using Microsoft.AspNetCore.Identity;

namespace SuperDoorLocks.DAL
{
    public static class DBInitializer
    {
        public static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            if (userManager.FindByEmailAsync("admin").Result == null)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = "admin"
                };

                //Not a great idea to keep passwords in source code, but as a quick and dirty hack for this exercise
                IdentityResult result = userManager.CreateAsync(user, "Password123").Result;

                if (result.Succeeded)
                {
                    userManager.AddToRoleAsync(user, UserRoles.ADMIN).Wait();
                }
            }
        }
    }
}
