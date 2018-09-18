using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZavrsniProjekat.Models;

namespace ZavrsniProjekat.Controllers
{
    public class HomeController : Controller
    {
        private FurnitureShopsEntities db = new FurnitureShopsEntities();

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("IndexAuth");
            }
            ViewBag.FurnitureShopId = new SelectList(db.FurnitureShops, "FurnitureShopId", "Name");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}