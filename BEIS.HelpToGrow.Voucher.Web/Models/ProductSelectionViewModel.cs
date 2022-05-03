using Microsoft.AspNetCore.Mvc;

namespace BEIS.HelpToGrow.Voucher.Web.Models
{
    public class ProductSelectionViewModel
    {
        [FromQuery(Name = "product_id")]
        public int ProductId { get; set; }

        [FromQuery(Name = "product_type")]
        public int ProductTypeId { get; set; }
    }
}