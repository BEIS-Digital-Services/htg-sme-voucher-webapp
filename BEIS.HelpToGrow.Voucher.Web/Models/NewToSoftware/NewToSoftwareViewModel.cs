using System;
using System.ComponentModel.DataAnnotations;
using Beis.Htg.VendorSme.Database.Models;

namespace BEIS.HelpToGrow.Voucher.Web.Models.NewToSoftware
{
    public class NewToSoftwareViewModel
    {
        [Required]
        public string FirstTime { get; set; }

        public product SelectedProduct { get; set; }
        public Uri ComparisonToolURL => Urls.ComparisonToolUrl;
    }
}