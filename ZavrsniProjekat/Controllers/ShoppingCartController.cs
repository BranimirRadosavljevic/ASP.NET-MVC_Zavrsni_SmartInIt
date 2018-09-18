using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZavrsniProjekat.Infrastructure;
using ZavrsniProjekat.Models;
using ZavrsniProjekat.ViewModels;

namespace ZavrsniProjekat.Controllers
{
    [Authorize(Roles = RoleName.Buyer)]
    public class ShoppingCartController : Controller
    {
        private FurnitureShopsEntities context = new FurnitureShopsEntities();
        private ApplicationDbContext dbUsers = new ApplicationDbContext();

        public ViewResult Index(string returnUrl)
        {
            return View(new ShoppingCartViewModel
            {
                ShoppingCart = GetCart(),
                ReturnUrl = returnUrl
            });
        }        
        
        public ActionResult AddToCart(int furnitureId)
        {
            Furniture item = context.Furnitures.Find(furnitureId);            
            if (item.AmountInStore > 0)
            {
                GetCart().AddItem(item, 1);
                return RedirectToAction("Index");
            }
            return View("OutOfStock"); 
        }

        public ActionResult IncreaseItemQuantity(int furnitureId)
        {
            Furniture item = context.Furnitures.Find(furnitureId);
            if (item.AmountInStore > 0)
            {
                GetCart().AddItem(item, 1);
                return RedirectToAction("Index");
            }
            return View("OutOfStock");
        }

        public ActionResult DecreaseItemQuantity(int furnitureId)
        {
            Furniture item = context.Furnitures.Find(furnitureId);
            if (item.AmountInStore > 0)
            {
                GetCart().DecreaseQuantity(item);
                return RedirectToAction("Index");
            }
            return View("OutOfStock");
        }

        public RedirectToRouteResult RemoveFromCart(int furnitureId, string returnUrl)
        {
            Furniture item = context.Furnitures.Find(furnitureId);
            if (item != null)
            {
                GetCart().RemoveItem(item);
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        private ShoppingCart GetCart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart == null)
            {
                cart = new ShoppingCart();
                Session["Cart"] = cart;
            }
            return cart;
        }

        public PartialViewResult Summary(ShoppingCart cart)
        {
            return PartialView(cart);
        }

        public ActionResult Purchase(ShoppingCart cart)
        {
            if (cart.Items.Count() == 0)
            {
                ModelState.AddModelError("", "Cart is empty!");
            }

            if (ModelState.IsValid)
            {
                BillViewModel bill = new BillViewModel();

                foreach (var item in cart.Items)
                {
                    BillItem billItem = new BillItem
                    {
                        Furniture = item.Furniture.Name,
                        FurnitureCategory = item.Furniture.Category.Name,
                        Amount = item.Quantity
                    };
                    Furniture furniture = context.Furnitures.Find(item.Furniture.FurnitureId);
                    if (furniture.AmountInStore - billItem.Amount < 0)
                    {
                        return View("QuantityExceeded");
                    }
                    billItem.Price = item.Furniture.Price * item.Quantity;
                    billItem.FurnitureShop = item.Furniture.FurnitureShop.Name;                    
                    bill.BillItems.Add(billItem);
                }

                bill.Tax = Tax.RegularTax;
                decimal subTotal = bill.BillItems.Sum(m => m.Price);
                bill.TotalPrice = subTotal + (subTotal * bill.Tax);
                bill.TimeOfPurchase = DateTime.Now;
                string firstName = (from users in dbUsers.Users where users.UserName.Equals(User.Identity.Name) select users.FirstName).First();
                string lastName = (from users in dbUsers.Users where users.UserName.Equals(User.Identity.Name) select users.LastName).First();
                bill.Buyer = firstName + " " + lastName;
                return View("../Bills/Bill", bill);
            }
            return RedirectToRoute(new { controller = "ShoppingCart", action = "Index" });
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompletePurchase (ShoppingCart cart)
        {
            if (cart.Items.Count() == 0)
            {
                ModelState.AddModelError("", "Cart is empty!");
            }

            if (ModelState.IsValid)
            {

                Bill bill = new Bill();

                foreach (var item in cart.Items)
                {
                    BillItem billItem = new BillItem
                    {
                        Furniture = item.Furniture.Name,
                        FurnitureCategory = item.Furniture.Category.Name,
                        Amount = item.Quantity,
                        Price = item.Furniture.Price * item.Quantity,
                        FurnitureShop = item.Furniture.FurnitureShop.Name
                    };
                    Furniture furniture = context.Furnitures.Find(item.Furniture.FurnitureId);                    
                    furniture.AmountInStore = furniture.AmountInStore - billItem.Amount;
                    context.Entry(furniture).State = EntityState.Modified; 
                    bill.BillItems.Add(billItem);
                    
                }

                bill.Tax = Tax.RegularTax;
                decimal subTotal = bill.BillItems.Sum(m => m.Price);
                bill.TotalPrice = subTotal + (subTotal * bill.Tax);
                bill.TimeOfPurchase = DateTime.Now;
                string firstName = (from users in dbUsers.Users where users.UserName.Equals(User.Identity.Name) select users.FirstName).First();
                string lastName = (from users in dbUsers.Users where users.UserName.Equals(User.Identity.Name) select users.LastName).First();
                bill.Buyer = firstName + " " + lastName;

                context.Bills.Add(bill);
                context.SaveChanges();      
                cart.Clear();
                return View("OrderSubmited");
            }
            return RedirectToRoute(new { controller = "ShoppingCart", action = "Index" });
        }
    }
}