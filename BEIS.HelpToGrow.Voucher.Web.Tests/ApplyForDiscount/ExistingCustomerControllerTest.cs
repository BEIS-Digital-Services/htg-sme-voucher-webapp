using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Web.Controllers;
using Beis.HelpToGrow.Voucher.Web.Models;
using Beis.HelpToGrow.Voucher.Web.Models.Voucher;
using Beis.HelpToGrow.Voucher.Web.Services;


namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class ExistingCustomerControllerTest : BaseControllerTest
    {
        private ExistingCustomerController _sut;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IProductRepository> _productRepository;
        private Mock<IVendorCompanyRepository> _vendorCompanyRepository;
        private ControllerContext _controllerContext;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _productRepository = new Mock<IProductRepository>();
            _vendorCompanyRepository = new Mock<IVendorCompanyRepository>();
            _controllerContext = SetupControllerContext(_controllerContext);
            SetupProductRepository(_productRepository);
            _vendorCompanyRepository.Setup(x => x.GetVendorCompanySingle(1)).Returns(Task.FromResult(new vendor_company { vendorid = 1, vendor_company_name = "mock_vendor"}));

            _sut = new ExistingCustomerController( _mockSessionService.Object, _productRepository.Object, _vendorCompanyRepository.Object);
            _sut.ControllerContext = _controllerContext;
        }

        [Test]
        public void ForwardMissingExistingCustomer()
        {
            var userVoucherDto = new UserVoucherDto();

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewModel = new ExistingCustomerViewModel { ExistingCustomer = default };

            var viewResult = (ViewResult)_sut.Forward(viewModel);

            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.That(viewResult.Model is ExistingCustomerViewModel);
        }

        [Test]
        public async Task GetIndexReturnsExistingCustomerViewModelWithSelectedProductIdAsOne()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);
            
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var viewResult = (ViewResult) await _sut.Index();
            
            Assert.That(((ExistingCustomerViewModel)viewResult.Model).VendorName == "mock_vendor");
        }

        [Test]
        public async Task CallForwardWithYesRedirectToCompanySizeControllerWithActionNameAsIndex()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var model = new ExistingCustomerViewModel { ExistingCustomer = "Yes" };
            var controllerResult = (RedirectToActionResult)_sut.Forward(model);

            Assert.That(controllerResult.ControllerName == "MajorUpgrade");
            Assert.That(controllerResult.ActionName == "Index");
        }

        [Test]
        public async Task CallForwardWithNoRedirectToMajorUpgradeControllerWithActionNameAsIndex()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var model = new ExistingCustomerViewModel { ExistingCustomer = "No" };
            var controllerResult = (RedirectToActionResult)_sut.Forward(model);

            Assert.That(controllerResult.ControllerName == "CompanySize");
            Assert.That(controllerResult.ActionName == "Index");
        }
    }
}