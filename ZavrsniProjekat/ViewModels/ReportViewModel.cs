using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ZavrsniProjekat.ViewModels
{
    public class ReportViewModel
    {
        public string Category { get; set; }
        public int AmountSold { get; set; }
        public decimal Price { get; set; }
        public string Shop { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime EndDate { get; set; }
    }
}