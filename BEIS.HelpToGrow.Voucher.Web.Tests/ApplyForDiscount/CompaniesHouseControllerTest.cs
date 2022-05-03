using System.Threading.Tasks;
using Beis.HelpToGrow.Core.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models.CompaniesHouse;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class CompaniesHouseControllerTest : BaseControllerTest
    {
        private CompaniesHouseController _sut;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IProductRepository> _productRepository;
        private ControllerContext _controllerContext;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _productRepository = new Mock<IProductRepository>();
            _controllerContext = SetupControllerContext(_controllerContext);
            SetupProductRepository(_productRepository);

            _sut = new CompaniesHouseController(_mockSessionService.Object);
        }

        [Test]
        public void GotANumber()
        {
            var userVoucherDto = new UserVoucherDto();

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewModel = new CompaniesHouseViewModel();

            var viewResult = (ViewResult)_sut.GotANumber(viewModel);

            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.That(viewResult.Model is CompaniesHouseViewModel);
        }

        [Test]
        public async Task GetIndexReturnsUserVoucherDtoModelWithCompanySizeAsYes()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1, "","","","Yes");
            
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            _sut.ControllerContext = _controllerContext;

            var controllerResult = (ViewResult)_sut.Index();
            
            Assert.That(((CompaniesHouseViewModel)controllerResult.Model).CompanySize == "Yes");
        }

        [Test]
        public async Task CallGotANumberWithYesRedirectsToCompaniesHouseNumberIndex()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1, "Yes","","","Yes");

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            _sut = NewCompaniesHouseController();

            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.GotANumber(new CompaniesHouseViewModel
            {
                HasCompaniesHouseNumber = "Yes"
            });

            Assert.AreEqual("CompaniesHouseNumber", controllerResult.ControllerName);
            Assert.AreEqual("Index", controllerResult.ActionName);
        }

        [Test]
        public async Task CallGotANumberWithNoReturnsFcaControllerWithViewNameAsIndex()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1, "Yes", "", "", "Yes");

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            _sut = NewCompaniesHouseController();
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.GotANumber(new CompaniesHouseViewModel
            {
                HasCompaniesHouseNumber = "No"
            });

            Assert.That(controllerResult.ControllerName == "FCA");
            Assert.That(controllerResult.ActionName == "Index");
        }

        private CompaniesHouseController NewCompaniesHouseController() => new(_mockSessionService.Object);
    }
}