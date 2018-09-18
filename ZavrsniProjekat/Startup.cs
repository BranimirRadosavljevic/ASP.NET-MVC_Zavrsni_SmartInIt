using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using ZavrsniProjekat.Models;

[assembly: OwinStartupAttribute(typeof(ZavrsniProjekat.Startup))]
namespace ZavrsniProjekat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers();
        }

        private void CreateRolesAndUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists(RoleName.Admin))
            {
                var role = new IdentityRole();
                role.Name = RoleName.Admin;
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "bane";
                user.Email = "bane@gmail.com";
                user.FirstName = "Branimir";
                user.LastName = "Radosavljevic";
                user.Address = "Vojvode Bojovica 14, Novi Sad";

                string userPWD = "Web.123";

                var chkUser = userManager.Create(user, userPWD);
                if (chkUser.Succeeded)
                { var result1 = userManager.AddToRole(user.Id, RoleName.Admin); }
            }            
            else
            {
                var role = new IdentityRole();
                role.Name = RoleName.Buyer;
                roleManager.Create(role);
            }
        }
    }
}
