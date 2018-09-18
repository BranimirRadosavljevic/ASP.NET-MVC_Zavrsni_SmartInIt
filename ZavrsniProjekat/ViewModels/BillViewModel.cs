using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ZavrsniProjekat.Models;

namespace ZavrsniProjekat.ViewModels
{
    public class BillViewModel
    {
        public BillViewModel()
        {
            this.BillItems = new HashSet<BillItem>();
        }

        public int BillId { get; set; }

        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = true)]
        public decimal Tax { get; set; }

        [Display(Name = "Total Price")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Time of Purchase")]
        public System.DateTime TimeOfPurchase { get; set; }

        public string Buyer { get; set; }

        
        [Display(Name = "Bill Items")]
        public virtual ICollection<BillItem> BillItems { get; set; }

        public decimal TotalTax()
        {
            return TotalPrice * Tax;
        }

        public decimal Subtotal()
        {
            return BillItems.Sum(m => m.Price);
        }
    }
}