using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly  UserManager<IdentityUser> _userManager;
        private readonly  RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;


        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public async void Initializer()
        {
            //Migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch
            {
            }

            //Create Roles if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();

                //if  roles are not created ,then we will create admin user as well

                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "Deepak kumar Das",
                    Email = "en.deepak1996@gmail.com",
                    Name = "Deepak",
                    PhoneNumber = "9090227262",
                    StreetAddress = "Deulahat",
                    State = "Odisha",
                    PostalCode = "756036",
                    City = "Balasore"
                }, "Deepak@1996kumar").GetAwaiter().GetResult();

                ApplicationUser user = _db.applicationUsers.FirstOrDefault(u => u.Email == "en.deepak1996@gmail.com");
                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }
            //if (_db.applicationUsers.FirstOrDefault(u => u.Email == "en.deepak1996@gmail.com") == null)
            //{
            //    _userManager.CreateAsync(new ApplicationUser
            //    {
            //        UserName = "Deepak kumar Das",
            //        Email = "en.deepak1996@gmail.com",
            //        Name = "Deepak",
            //        PhoneNumber = "9090227262",
            //        StreetAddress = "Deulahat",
            //        State = "Odisha",
            //        PostalCode = "756036",
            //        City = "Balasore"
            //    }, "Deepak@1996kumar").GetAwaiter().GetResult();




            //    ApplicationUser user = _db.applicationUsers.FirstOrDefault(u => u.Email == "en.deepak1996@gmail.com");
            //    _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();


            //}
            return;
    }   }
}
