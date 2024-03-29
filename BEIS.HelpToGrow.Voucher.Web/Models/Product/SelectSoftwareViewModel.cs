﻿
namespace Beis.HelpToGrow.Voucher.Web.Models.Product
{
    public class SelectSoftwareViewModel
    {
        [Required]
        [NotDefault]
        public long ProductId { get; set; }
        public List<product> ProductList { get; set; }
        public product SelectedProduct { get; set; }
        public string SelectedProductTypeName { get; set; }
    }
}