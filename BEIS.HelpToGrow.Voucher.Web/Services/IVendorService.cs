﻿
namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public interface IVendorService
    {
        Task<bool> IsRegisteredVendor(string companyNumber);
    }
}