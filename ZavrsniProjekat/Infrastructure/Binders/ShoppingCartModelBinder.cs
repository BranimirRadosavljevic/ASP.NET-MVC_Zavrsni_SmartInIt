using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ZavrsniProjekat.Infrastructure.Binders
{
    public class ShoppingCartModelBinder : IModelBinder
    {
        private const string sessionKey = "Cart";

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            ShoppingCart cart = null;
            if (controllerContext.HttpContext.Session != null)
            {
                cart = (ShoppingCart)controllerContext.HttpContext.Session[sessionKey];
            }

            if (cart == null)
            {
                cart = new ShoppingCart();
                if (controllerContext.HttpContext.Session != null)
                {
                    controllerContext.HttpContext.Session[sessionKey] = cart;
                }
            }

            return cart;
        }
    }
}