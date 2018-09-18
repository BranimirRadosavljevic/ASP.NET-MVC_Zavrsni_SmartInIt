using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ZavrsniProjekat.Models;
using ZavrsniProjekat.ViewModels;

namespace ZavrsniProjekat.Controllers
{
    [Authorize(Roles = RoleName.Admin)]
    public class UsersController : Controller
    {

        private ApplicationDbContext context = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Users
        public ActionResult Index()
        {           
            return View();
        }        

        public ActionResult IndexBuyers()
        {
            return View("BuyersList");
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            IdentityUserRole userRole = user.Roles.Where(m => m.UserId == user.Id).FirstOrDefault();
            IdentityRole role = context.Roles.Where(m => m.Id == userRole.RoleId).FirstOrDefault();
            var userDetails = new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                Email = user.Email,
                UserRole = role.Name
            };
            return View(userDetails);
        }

        public ActionResult Add()
        {
            ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name");
            return View();
        }

        [HttpPost]        
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName, Address = model.Address };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, model.UserRole);                  

                    return Json(new { success = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
                }                
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = context.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            IdentityUserRole userRole = user.Roles.Where(m => m.UserId == user.Id).FirstOrDefault();
            IdentityRole role = context.Roles.Where(m => m.Id == userRole.RoleId).FirstOrDefault();
            ViewBag.Name = new SelectList(context.Roles.Where(m =>!m.Name.Contains(role.Name)), "Name", "Name");
            ViewBag.Role = role.Name;
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var store = new UserStore<ApplicationUser>(context);
                var manager = new UserManager<ApplicationUser>(store);
                var currentUser = manager.FindById(user.Id);
                currentUser.FirstName = user.FirstName;
                currentUser.LastName = user.LastName;
                currentUser.Address = user.Address;
                currentUser.Email = user.Email;
                await manager.UpdateAsync(currentUser);
                //context.Entry(user).State = EntityState.Modified;
                if (user.UserRoles == RoleName.Admin || user.UserRoles == RoleName.Buyer)                
                {
                    string role = user.UserRoles == RoleName.Admin ? RoleName.Buyer : RoleName.Admin;
                    await UserManager.RemoveFromRoleAsync(user.Id, role);
                    await UserManager.AddToRoleAsync(user.Id, user.UserRoles);
                }

                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            //ViewBag.Name = new SelectList(context.Roles.ToList(), "Name", "Name");            
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost]        
        public ActionResult Delete(string id)
        {
            ApplicationUser user = context.Users.Find(id);
            context.Users.Remove(user);
            context.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetBuyers()
        {
            var buyers = (from user in context.Users
                                  from userRole in user.Roles
                                  join role in context.Roles on userRole.RoleId equals role.Id
                                  where role.Name.Equals("Buyer")
                                  select new UserViewModel()
                                  {
                                      Id = user.Id,
                                      UserName = user.UserName,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      Address = user.Address,
                                      Email = user.Email,
                                      UserRole = role.Name
                                  }).ToList();

            return Json(buyers, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetAllUsers()
        {
            var usersWithRoles = (from user in context.Users
                                  from userRole in user.Roles
                                  join role in context.Roles on userRole.RoleId equals role.Id
                                  select new UserViewModel()
                                  {
                                      Id = user.Id,
                                      UserName = user.UserName,
                                      FirstName = user.FirstName,
                                      LastName = user.LastName,
                                      Address = user.Address,
                                      Email = user.Email,
                                      UserRole = role.Name
                                  }).ToList();
            return Json(usersWithRoles, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult DoesUserNameExist(string UserName)
        {
            return Json(!context.Users.Any(x => x.UserName == UserName), JsonRequestBehavior.AllowGet);
        }
                
    }
}