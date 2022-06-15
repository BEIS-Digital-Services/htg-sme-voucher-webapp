using System;
using System.Threading.Tasks;
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
    public class MajorUpgradeControllerTest : BaseControllerTest
    {
        private MajorUpgradeController _sut;
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
        }

        [Test]
        public void IndexSessionError()
        {
            _sut = new MajorUpgradeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Throws(new Exception("fake error message"));

            Assert.Throws<Exception>(() => _sut.Index());
        }

        [Test]
        public async Task Index()
        {
            _sut = new MajorUpgradeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            var userVoucherDto = await SetupSelection(_productRepository, 1, 1, "", "true");

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(userVoucherDto);

            var viewResult = (ViewResult)_sut.Index();

            Assert.AreEqual(0, viewResult.ViewData.Count);
            Assert.That(viewResult.Model is MajorUpgradeViewModel);
            var viewModel = (MajorUpgradeViewModel)viewResult.Model;
            Assert.AreNotEqual(viewModel.SelectedProduct,null);
        }

        [Test]
        public void ForwardSessionError()
        {
            _sut = new MajorUpgradeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Throws(new Exception("fake error message"));


            Assert.Throws<Exception>(() => _sut.Forward(new MajorUpgradeViewModel()));
        }

        [Test]
        public void ForwardInvalidModel()
        {
            _sut = new MajorUpgradeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(new UserVoucherDto { SelectedProduct = new product() });

            var viewResult = (ViewResult)_sut.Forward(new MajorUpgradeViewModel());

            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [Test]
        public async Task CallForwardWithYesRedirectToCompanySizeControllerWithActionNameAsIndex()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1, "No","Yes");

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new MajorUpgradeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.Forward(new MajorUpgradeViewModel { MajorUpgrade = "Yes" });

            Assert.That(controllerResult.ControllerName == "CompanySize");
            Assert.That(controllerResult.ActionName == "Index");
        }

        [Test]
        public async Task CallForwardWithNoRedirectToMajorUpgradeControllerWithActionNameAsAddons()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1, "No", "No");

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new MajorUpgradeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.Forward(new MajorUpgradeViewModel { MajorUpgrade = "No" });

            Assert.That(controllerResult.ControllerName == "InEligible");
            Assert.That(controllerResult.ActionName == "MajorUpgrade");
        }
    }
}