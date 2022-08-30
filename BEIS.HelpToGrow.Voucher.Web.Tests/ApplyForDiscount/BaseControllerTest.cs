
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    public class BaseControllerTest
    {
        protected void SetupProductRepository(Mock<IProductRepository> mockProductRepository)
        {
            mockProductRepository.Setup(x => x.GetProducts()).Returns(Task.FromResult(new List<product>
            {
                new() {product_id =1, vendor_id=1, product_type=1},
                new() {product_id =2, vendor_id=1, product_type=2},
                new() {product_id =3, vendor_id=1, product_type=2},
                new() {product_id =4, vendor_id=1, product_type=3},
                new() {product_id =5, vendor_id=1, product_type=3},
                new() {product_id =6, vendor_id=1, product_type=3},
            }));

            mockProductRepository.Setup(x => x.GetProductSingle(1)).Returns(Task.FromResult(new product { product_id = 1, vendor_id = 1, product_type = 1 }));
            
            mockProductRepository.Setup(x => x.ProductTypes()).Returns(Task.FromResult(new List<settings_product_type>
            {
                new() { id=1,item_name="product_type-1" },
                new() { id=2,item_name="product_type-2" }
            }));
        }

        public ControllerContext SetupControllerContext(ControllerContext controllerContext)
        {
            var httpContext = new DefaultHttpContext { Session = new SessionMock() };

            controllerContext ??= new ControllerContext();
            controllerContext.HttpContext = httpContext;

            return controllerContext;
        }

        public async Task<UserVoucherDto> SetupSelection(Mock<IProductRepository> mockProductRepository, int productTypeId = 0, int productId = 0, string isFirstTime = "", string isMajorUpgrade = "", string isAddons = "", string isEligibleCompanySize = "", string hasCompaniesHouseNumber = "", string companiesHouseNumber = "", string hasFCANumber = "", string fcaNumber = "", ApplicantDto applicantDto = null)
        {
            var expectedModel = new UserVoucherDto();
            if (productTypeId != 0)
            {
                expectedModel.ProductTypeList = await mockProductRepository.Object.ProductTypes();
                expectedModel.SelectedProductType = (await mockProductRepository.Object.ProductTypes()).Find(x => x.id == productTypeId);
            }

            if (productId != 0)
            {
                expectedModel.ProductList = mockProductRepository.Object.GetProducts().Result;
                expectedModel.SelectedProduct = mockProductRepository.Object.GetProducts().Result.Find(x => x.product_id == productId);
            }

            if (isFirstTime != string.Empty)
            {
                expectedModel.FirstTime = isFirstTime;
            }

            if (isMajorUpgrade != string.Empty)
            {
                expectedModel.MajorUpgrade = isMajorUpgrade;
            }

            if (isAddons != string.Empty)
            {
                expectedModel.Addons = isAddons;
            }

            if (isEligibleCompanySize != string.Empty)
            {
                expectedModel.CompanySize = isEligibleCompanySize;
            }

            if (hasCompaniesHouseNumber != string.Empty)
            {
                expectedModel.HasCompanyHouseNumber = hasCompaniesHouseNumber;
            }

            if (companiesHouseNumber != string.Empty)
            {
                expectedModel.CompanyHouseNumber = companiesHouseNumber;
            }

            if (hasFCANumber != string.Empty)
            {
                expectedModel.HasFCANumber = hasFCANumber;
            }

            if (fcaNumber != string.Empty)
            {
                expectedModel.FCANumber = fcaNumber;
            }

            if (applicantDto != null)
            {
                expectedModel.ApplicantDto.FullName = applicantDto.FullName;
                expectedModel.ApplicantDto.Role = applicantDto.Role;
                expectedModel.ApplicantDto.EmailAddress = applicantDto.EmailAddress;
                expectedModel.ApplicantDto.PhoneNumber = applicantDto.PhoneNumber;
                expectedModel.ApplicantDto.HasAcceptedTermsAndConditions = applicantDto.HasAcceptedTermsAndConditions;
                expectedModel.ApplicantDto.HasAcceptedPrivacyPolicy = applicantDto.HasAcceptedPrivacyPolicy;
                expectedModel.ApplicantDto.HasAcceptedSubsidyControl = applicantDto.HasAcceptedSubsidyControl;
                expectedModel.ApplicantDto.HasProvidedMarketingConsent = applicantDto.HasProvidedMarketingConsent;
                expectedModel.ApplicantDto.HasProvidedMarketingConsentByPhone = applicantDto.HasProvidedMarketingConsentByPhone;
            }
            
            return expectedModel;
        }
    }
}