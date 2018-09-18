using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZavrsniProjekat.Infrastructure;
using ZavrsniProjekat.Models;
using ZavrsniProjekat.ViewModels;

namespace ZavrsniProjekat.Controllers
{
    [Authorize(Roles = RoleName.Admin)]
    public class BillsController : Controller
    {
        private FurnitureShopsEntities db = new FurnitureShopsEntities();

        // GET: Bills
        public ActionResult Index()
        {
            return View("BillsList");
        }

        public ActionResult Report()
        {
            ViewBag.Category = new SelectList(db.Categories, "Name", "Name");
            ViewBag.StartDate = new DateTime();
            ViewBag.EndDate = new DateTime();
            return View("FormSubmit");           
        }

        public ActionResult GenerateReport(string Category, DateTime StartDate, DateTime EndDate)
        {
            DateTime endDate = EndDate.AddDays(1);

            var result = (from item in db.BillItems
                          join bill in db.Bills on item.BillId equals bill.BillId
                          where bill.TimeOfPurchase >= StartDate.Date && bill.TimeOfPurchase <= endDate
                          && item.FurnitureCategory.Equals(Category)
                          group item by new { item.FurnitureShop, item.FurnitureCategory, item.Amount, item.Price } into g
                          let sumPrice = g.Sum(m => m.Price)
                          let sumAmount = g.Sum(m => m.Amount)
                          select new ReportViewModel
                          {
                              Category = g.Key.FurnitureCategory,
                              Shop = g.Key.FurnitureShop,
                              AmountSold = sumAmount,
                              Price = sumPrice + sumPrice * Tax.RegularTax,
                              
                          }).ToList();

            var report = result.GroupBy(d => d.Shop)
                        .Select(m => new ReportViewModel
                        {
                            Shop = m.First().Shop,
                            Category = m.First().Category,
                            AmountSold = m.Sum(c => c.AmountSold),
                            Price = m.Sum(c => c.Price),
                            StartDate = StartDate,
                            EndDate = EndDate
                        }).ToList();
            return View(report);
        }

        public ActionResult GetAllData()
        {
            List<Bill> lst = db.Bills.ToList();
            var subCategoryToReturn = lst.Select(s => new
            {
                s.BillId,
                s.Buyer,
                s.TimeOfPurchase,
                TotalTax = s.TotalTax(),
                s.TotalPrice,
                FurnitureShops = s.BillItems.Select(m => new
                {
                    Shop = m.FurnitureShop
                }),
                BillItems = s.BillItems.Select(m => new
                {
                    Item = m.Furniture
                }),
                Subtotal = s.Subtotal()
            });

            return this.Json(subCategoryToReturn, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
       
    }
}
