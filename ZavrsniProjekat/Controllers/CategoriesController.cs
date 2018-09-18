using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZavrsniProjekat.Models;

namespace ZavrsniProjekat.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private FurnitureShopsEntities db = new FurnitureShopsEntities();

        // GET: Categories
        public ActionResult Index()
        {
            if (User.IsInRole(RoleName.Admin))
                return View("IndexAdmin");

            return View("IndexBuyer");

        }

        // GET: Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Category category)
        {
            if (db.Categories.Any(x => x.Name == category.Name))
            {
                ModelState.AddModelError("", "Name already exists!");
            }
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                return Json(new { success = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return View(category);
        }
        
        public ActionResult Edit(int id)
        {
            return View(db.Categories.Find(id));
        }

        [HttpPost]
        public ActionResult Edit(Category category)
        { 
            db.Entry(category).State = EntityState.Modified;
            db.SaveChanges();
            return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
           
        }

        [HttpPost]        
        public ActionResult Delete(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet); 
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult GetData()
        {
            List<Category> lst = db.Categories.ToList();
            var subCategoryToReturn = lst.Select(s => new
            {
                s.CategoryId,
                Category = s.Name,
                s.Description
            });

            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoesCategoryNameExist(string Name)
        {           
            return Json(!db.Categories.Any(x => x.Name == Name), JsonRequestBehavior.AllowGet);
        }
    }
}
