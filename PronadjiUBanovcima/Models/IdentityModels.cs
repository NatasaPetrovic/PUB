using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PronadjiUBanovcima.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string Email { get; set; }
        public string ConfirmationToken { get; set; }
        public bool IsConfirmed { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            return userIdentity;
        }
       
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            

        }
        public DbSet<Slika> Slike { get; set; }
        public DbSet<Delatnost> Delatnosti { get; set; }
        public DbSet<Info> Podaci { get; set; }
        public DbSet<PremiumInfo> PodaciPremium { get; set; }
        public DbSet<Tag> Tagovi { get; set; }

    }
}