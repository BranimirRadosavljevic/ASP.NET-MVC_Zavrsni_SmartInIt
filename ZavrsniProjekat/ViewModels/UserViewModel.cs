using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZavrsniProjekat.ViewModels
{
    public class UserViewModel
    {

        public string Id { get; set; }

        [Display(Name = "User name")]        
        public string UserName { get; set; }

        
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        
        public string Address { get; set; }
        public string Email { get; set; }

        [Display(Name = "User role")]
        public string UserRole { get; set; }

    }
}