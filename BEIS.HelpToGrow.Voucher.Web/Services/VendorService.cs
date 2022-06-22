
namespace Beis.HelpToGrow.Voucher.Web.Services
{
    public class VendorService : IVendorService
    {
        private readonly IVendorCompanyRepository _repository;

        public VendorService(IVendorCompanyRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> IsRegisteredVendor(string companyNumber)
        {
            bool HasMatchingNumericValue(string vendorCompanyHouseRegistrationNumber) =>
                int.TryParse(vendorCompanyHouseRegistrationNumber, out var vendorCompaniesHouseNumber) &&
                int.TryParse(companyNumber, out var number) &&
                vendorCompaniesHouseNumber == number;

            var existingVendors = await _repository.GetVendorCompanies();

            return existingVendors.Any(_ => _.vendor_company_house_reg_no == companyNumber || HasMatchingNumericValue(_.vendor_company_house_reg_no));
        }
    }
}