using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZavrsniProjekat.Infrastructure;

namespace ZavrsniProjekat.ViewModels
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }
        public string ReturnUrl { get; set; }
    }
}