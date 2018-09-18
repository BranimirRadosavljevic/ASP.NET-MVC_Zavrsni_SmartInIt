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
    public class FurnitureShopsController : Controller
    {
        private FurnitureShopsEntities db = new FurnitureShopsEntities();

        // GET: FurnitureShops
        public ActionResult Index()
        {
            if (User.IsInRole(RoleName.Admin))
                return View("IndexAdmin");
            return View("IndexBuyer");
        }

        // GET: FurnitureShops/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FurnitureShop furnitureShop = db.FurnitureShops.Find(id);
            if (furnitureShop == null)
            {
                return HttpNotFound();
            }
            return View(furnitureShop);
        }

        // GET: FurnitureShops/Create
        public ActionResult Add()
        {
            return View();
        }

        // POST: FurnitureShops/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add([Bind(Include = "FurnitureShopId,Name,Owner,Address,Phone,Email,Website,Pib,AccountNo")] FurnitureShop furnitureShop)
        {
            if (ModelState.IsValid)
            {
                db.FurnitureShops.Add(furnitureShop);
                db.SaveChanges();
                return Json(new { success = true, message = "Added Successfully" }, JsonRequestBehavior.AllowGet);
            }

            return View(furnitureShop);
        }

        // GET: FurnitureShops/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FurnitureShop furnitureShop = db.FurnitureShops.Find(id);
            if (furnitureShop == null)
            {
                return HttpNotFound();
            }
            return View(furnitureShop);
        }

        // POST: FurnitureShops/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FurnitureShopId,Name,Owner,Address,Phone,Email,Website,Pib,AccountNo")] FurnitureShop furnitureShop)
        {
            if (ModelState.IsValid)
            {
                db.Entry(furnitureShop).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
            }
            return View(furnitureShop);
        }        

        // POST: FurnitureShops/Delete/5
        [HttpPost]        
        public ActionResult Delete(int id)
        {
            FurnitureShop furnitureShop = db.FurnitureShops.Find(id);
            db.FurnitureShops.Remove(furnitureShop);
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
            List<FurnitureShop> lst = db.FurnitureShops.ToList();
            var subCategoryToReturn = lst.Select(s => new
            {
                s.FurnitureShopId,
                s.Name,
                s.Owner,
                s.Address,
                s.Phone,
                s.Email,
                s.Website,
                s.Pib,
                s.AccountNo
            });

            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoesPibExist(int Pib)
        {
            return Json(!db.FurnitureShops.Any(x => x.Pib == Pib), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoesAccountExist(string AccountNo)
        {
            return Json(!db.FurnitureShops.Any(x => x.AccountNo == AccountNo), JsonRequestBehavior.AllowGet);
        }
    }
}
