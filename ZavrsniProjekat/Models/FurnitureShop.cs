//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZavrsniProjekat.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;

    public partial class FurnitureShop
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FurnitureShop()
        {
            this.Furnitures = new HashSet<Furniture>();
        }
    
        public int FurnitureShopId { get; set; }

        [Required(ErrorMessage = "Name of the shop is required!")]
        public string Name { get; set; }
        public string Owner { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        [Url]
        public string Website { get; set; }

        [Required(ErrorMessage = "PIB is required!")]
        [Display(Name = "PIB")]
        [Remote("DoesPibExist", "FurnitureShops", ErrorMessage = "PIB already exists!")]
        public long Pib { get; set; }

        [Required(ErrorMessage = "Account Number is required!")]
        [Display(Name = "Account Number")]
        [Remote("DoesAccountExist", "FurnitureShops", ErrorMessage = "Account Number already exists!")]
        public string AccountNo { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Furniture> Furnitures { get; set; }
    }
}
