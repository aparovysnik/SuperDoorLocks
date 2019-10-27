using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SuperDoorLocks.DomainModels;
using System.IO;

public class DoorsDBContext : IdentityDbContext<IdentityUser>
{
    private static bool _created = false;
    public DoorsDBContext()
    {
        if (!_created)
        {
            _created = true;
            Database.EnsureDeleted();
            Database.EnsureCreated();
        }
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionbuilder)
    {
        optionbuilder.UseSqlite(@"Data Source=SuperDoorLocks.db");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //Seed user data
        builder.Entity<IdentityRole>()
            .HasData(new IdentityRole { Name = SuperDoorLocks.UserRoles.ADMIN,
                    NormalizedName = SuperDoorLocks.UserRoles.ADMIN.ToUpper() },
                new IdentityRole { Name = SuperDoorLocks.UserRoles.EMPLOYEE,
                    NormalizedName = SuperDoorLocks.UserRoles.EMPLOYEE.ToUpper() });

        builder.Entity<Door>()
                .HasMany(p => p.PermittedRoles);
        base.OnModelCreating(builder);
    }

    public DbSet<Door> Doors { get; set; }
}