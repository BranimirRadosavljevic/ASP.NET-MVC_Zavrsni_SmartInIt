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
    public class FurnituresController : Controller
    {
        private FurnitureShopsEntities db = new FurnitureShopsEntities();

        // GET: Furnitures
        public ActionResult Index()
        {
            if (User.IsInRole(RoleName.Admin))
                return View("IndexAdmin");
            return View("IndexBuyer");
        }        

        // GET: Furnitures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Furniture furniture = db.Furnitures.Find(id);
            if (furniture == null)
            {
                return HttpNotFound();
            }
            return View(furniture);
        }

        // GET: Furnitures/Create
        public ActionResult Add()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            ViewBag.FurnitureShopId = new SelectList(db.FurnitureShops, "FurnitureShopId", "Name");
            return View();
        }

        // POST: Furnitures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Furniture furniture, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    furniture.PhotoType = image.ContentType;
                    furniture.Photo = new byte[image.ContentLength];
                    image.InputStream.Read(furniture.Photo, 0, image.ContentLength);
                }
                db.Furnitures.Add(furniture);
                db.SaveChanges();
                return Json(new { success = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", furniture.CategoryId);
            ViewBag.FurnitureShopId = new SelectList(db.FurnitureShops, "FurnitureShopId", "Name", furniture.FurnitureShopId);
            return View(furniture);
        }
                
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Furniture furniture = db.Furnitures.Find(id);
            if (furniture == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", furniture.CategoryId);
            ViewBag.FurnitureShopId = new SelectList(db.FurnitureShops, "FurnitureShopId", "Name", furniture.FurnitureShopId);
            return View(furniture);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Furniture furniture, HttpPostedFileBase image = null)
        {
            if (ModelState.IsValid)
            {
                if (image == null)
                {
                    db.Entry(furniture).State = EntityState.Modified;
                    db.Entry(furniture).Property(x => x.Photo).IsModified = false;                    
                }
                else
                {
                    furniture.PhotoType = image.ContentType;
                    furniture.Photo = new byte[image.ContentLength];
                    image.InputStream.Read(furniture.Photo, 0, image.ContentLength);
                    db.Entry(furniture).State = EntityState.Modified;
                }                
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", furniture.CategoryId);
            ViewBag.FurnitureShopId = new SelectList(db.FurnitureShops, "FurnitureShopId", "Name", furniture.FurnitureShopId);
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public ActionResult Delete(int id)
        {
            Furniture furniture = db.Furnitures.Find(id);
            db.Furnitures.Remove(furniture);
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

        //[AllowAnonymous]
        //public ActionResult GetImage(int FurnitureId)
        //{
        //    Furniture furniture = db.Furnitures.Find(FurnitureId);
        //    if (furniture.Photo != null && furniture.PhotoType != null)
        //    {
        //        return File(furniture.Photo, furniture.PhotoType);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        [AllowAnonymous]
        public ActionResult GetDataFromShop(int id)
        {
            List<Furniture> lst = db.Furnitures.ToList();
            var subCategoryToReturn = lst.Where(m => m.FurnitureShopId == id).Select(s => new
            {
                s.FurnitureId,
                s.Name,
                Category = s.Category.Name,
                s.AmountInStore,
                s.Price,
                Photo = s.Photo != null ? Convert.ToBase64String(s.Photo) : null,
                s.PhotoType
            });

            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAllData()
        {
            List<Furniture> lst = db.Furnitures.ToList();
            var subCategoryToReturn = lst.Select(s => new
            {
                s.FurnitureId,
                s.Code,
                s.Name,
                s.CountryOfOrigin,
                s.YearMade,
                Category = s.Category.Name,
                FurnitureShop = s.FurnitureShop.Name,
                s.AmountInStore,
                s.Price,
                Photo = s.Photo != null ? Convert.ToBase64String(s.Photo) : null,
                s.PhotoType

            });

            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoesCodeExist(string Code)
        {
            return Json(!db.Furnitures.Any(x => x.Code == Code), JsonRequestBehavior.AllowGet);
        }
    }
}
