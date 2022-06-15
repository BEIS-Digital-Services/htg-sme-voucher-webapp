using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
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
    public class SelectProductTypeControllerTest : BaseControllerTest
    {
        private SelectProductTypeController _sut;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IProductRepository> _mockProductRepository;
        private Mock<ProductTypeViewModel> _productTypeViewModel;
        private ControllerContext _controllerContext;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _mockProductRepository = new Mock<IProductRepository>();
            _controllerContext = SetupControllerContext(_controllerContext);
            _productTypeViewModel = new Mock<ProductTypeViewModel>();
            _sut = new SelectProductTypeController(
                _mockSessionService.Object,
                _mockProductRepository.Object);

            SetupProductRepository(_mockProductRepository);
        }

        [Test]
        public void IndexMissingSession()
        {
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            Assert.ThrowsAsync<Exception>(() => _sut.Index());
        }

        [Test]
        public async Task Index()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ProductTypeList = null,
                SelectedProductType = new settings_product_type()
            };

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var fakeProductTypes = new List<settings_product_type> { userVoucherDto.SelectedProductType };

            _mockProductRepository
                .Setup(_ => _.ProductTypes())
                .Returns(Task.FromResult(fakeProductTypes));

            var index = await _sut.Index();
            var viewResult = (ViewResult) index;

            Assert.That(viewResult.Model is ProductTypeViewModel);
            Assert.That(((ProductTypeViewModel) viewResult.Model).ProductTypeList.Any());
        }

        [Test]
        public async Task IndexPostInvalid()
        {
            var fakeProductType = new settings_product_type();
            var fakeProductTypes = new List<settings_product_type> { fakeProductType };
            var userVoucherDto = new UserVoucherDto
            {
                ProductTypeList = fakeProductTypes,
                SelectedProductType = fakeProductType
            };

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _mockProductRepository
                .Setup(_ => _.ProductTypes())
                .Returns(Task.FromResult(fakeProductTypes));

            var invalidModel = new ProductTypeViewModel();

            var index = await _sut.Index(invalidModel);
            var viewResult = (ViewResult)index;

            Assert.That(viewResult.Model is ProductTypeViewModel);
            Assert.AreEqual(fakeProductTypes, ((ProductTypeViewModel)viewResult.Model).ProductTypeList);
        }

        [Test]
        public async Task IndexPostMissingProductList()
        {
            var expectedModel = await SetupSelection(_mockProductRepository);
            
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            _sut.ControllerContext = _controllerContext;
            _productTypeViewModel.Object.ProductTypeList = await _mockProductRepository.Object.ProductTypes();
            _productTypeViewModel.Object.ProductType = 1;
            var controllerResult = (RedirectToActionResult) await _sut.Index(_productTypeViewModel.Object);

            Assert.AreEqual("SelectSoftware", controllerResult.ControllerName);
            Assert.AreEqual("Index", controllerResult.ActionName);
        }

        [Test]
        public async Task IndexPost()
        {
            var fakeProductType = new settings_product_type();
            var fakeProductTypes = new List<settings_product_type> { fakeProductType };
            var userVoucherDto = new UserVoucherDto
            {
                ProductTypeList = fakeProductTypes,
                SelectedProductType = fakeProductType
            };

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _mockProductRepository
                .Setup(_ => _.ProductTypes())
                .Returns(Task.FromResult(fakeProductTypes));

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(userVoucherDto);

            var productTypeViewModel = new ProductTypeViewModel
            {
                ProductType = 1
            };
            var controllerResult = (RedirectToActionResult) await _sut.Index(productTypeViewModel);

            Assert.AreEqual("SelectSoftware", controllerResult.ControllerName);
            Assert.AreEqual("Index", controllerResult.ActionName);
        }

        [Test]
        public void IndexPostHandlesSessionException()
        {
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            Assert.ThrowsAsync<Exception>(() => _sut.Index(new ProductTypeViewModel()));
        }
    }
}