using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models.Product;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class SelectSoftwareControllerTest : BaseControllerTest
    {
        private SelectSoftwareController _sut;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IProductRepository> _productRepository;
        private ControllerContext _controllerContext;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _productRepository = new Mock<IProductRepository>();
            _controllerContext = SetupControllerContext(_controllerContext);
            _sut = new SelectSoftwareController(_mockSessionService.Object, _productRepository.Object);
            _sut.ControllerContext = _controllerContext;
            SetupProductRepository(_productRepository);
        }

        [Test]
        public async Task GetIndexMissingProductList()
        {
            var userVoucherDto = new UserVoucherDto { SelectedProductType = new settings_product_type { id = 1 }};

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(userVoucherDto);

            var result = await _sut.Index();
            var viewResult = (ViewResult) result;

            Assert.AreNotEqual(0, ((SelectSoftwareViewModel)viewResult.Model).ProductList);
        }

        [Test]
        public void PostIndexMissingProduct()
        {
            var userVoucherDto = new UserVoucherDto { SelectedProductType = new settings_product_type { id = 1 } };

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(userVoucherDto);

            var result = _sut.Index(new SelectSoftwareViewModel());
            var viewResult = (ViewResult)result;

            Assert.That(viewResult.Model is SelectSoftwareViewModel);
            Assert.Null(viewResult.ViewName);
        }

        [Test]
        public void PostIndexMissingSession()
        {
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns((UserVoucherDto)null);

            var result = _sut.Index(new SelectSoftwareViewModel());
            var viewResult = (ViewResult)result;

            Assert.That(viewResult.Model is SelectSoftwareViewModel);
            Assert.Null(viewResult.ViewName);
        }

        [Test]
        public void PostIndex()
        {
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProductType = new settings_product_type { id = 1 },
                ProductList = new List<product> { new() { product_id = 1 } }
            };

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(userVoucherDto);

            var viewModel = new SelectSoftwareViewModel { ProductId = 1 };
            var result = _sut.Index(viewModel);
            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("ConfirmSoftware", actionResult.ControllerName);
            Assert.AreEqual("Index", actionResult.ActionName);
        }

        [Test]
        public async Task GetIndexReturnsUserVoucherDtoModelWithSelectedProductTypeIdAsOne()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);
            
            expectedModel.ProductTypeList = await _productRepository.Object.ProductTypes();

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var result = await _sut.Index();
            
            var controllerResult = (ViewResult)result;
            
            Assert.AreEqual(1, ((SelectSoftwareViewModel)controllerResult.Model).SelectedProduct.product_id);
        }
    }
}