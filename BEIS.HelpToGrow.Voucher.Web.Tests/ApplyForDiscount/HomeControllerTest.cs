
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class HomeControllerTest : BaseControllerTest
    {
        private HomeController _sut;
        private Mock<ILogger<HomeController>> _mockLogger;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IProductRepository> _productRepository;
        private ControllerContext _controllerContext;
        private Mock<ICookieService> _cookieService;

        private HomeController HomeControllerObject() => new(
            _mockSessionService.Object,
            _cookieService.Object,
            _productRepository.Object,
            _mockLogger.Object,
            Options.Create(new UrlOptions { LearningPlatformUrl = "https://fake-test-webapp.azurewebsites.net/" }));

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();
            _mockSessionService = new Mock<ISessionService>();
            _cookieService = new Mock<ICookieService>();
            _productRepository = new Mock<IProductRepository>();
            _controllerContext = SetupControllerContext(_controllerContext);
            SetupProductRepository(_productRepository);
            _sut = HomeControllerObject();
            _sut.ControllerContext = _controllerContext;
        }

        [Test]
        public async Task PostIndex()
        {
            var viewModel = new ProductSelectionViewModel();

            var cookieBannerViewModel = new CookieBannerViewModel();

            _cookieService
                .Setup(_ => _.SyncCookieSelection(It.IsAny<HttpRequest>(), It.IsAny<CookieBannerViewModel>()))
                .Returns(cookieBannerViewModel);

            var userVoucherDto = new UserVoucherDto();

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var result = await _sut.Index(viewModel);

            var actionResult = (RedirectToActionResult) result;

            Assert.AreEqual(nameof(SoftwareNotChosen), actionResult.ActionName);
        }

        [Test]
        public async Task PostIndexSelectedProduct()
        {
            var model = new ProductSelectionViewModel
            {
                ProductId = 1,
                ProductTypeId = 2
            };

            var cookieBannerViewModel = new CookieBannerViewModel();

            _cookieService
                .Setup(_ => _.SyncCookieSelection(It.IsAny<HttpRequest>(), It.IsAny<CookieBannerViewModel>()))
                .Returns(cookieBannerViewModel);

            var userVoucherDto = new UserVoucherDto();

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var product = new product
            {
                product_id = 1,
                product_type = 2
            };

            _productRepository
                .Setup(_ => _.GetProductSingle(1))
                .Returns(Task.FromResult(product));

            var productTypes = new List<settings_product_type>
            {
                new() { id = 2 }
            };

            _productRepository.Setup(_ => _.ProductTypes()).Returns(Task.FromResult(productTypes));
            
            var result = await _sut.Index(model);
            var viewResult = (ViewResult) result;
            var viewModel = viewResult.Model as HomeViewModel;

            Assert.Null(viewResult.ViewName);
            Assert.NotNull(viewModel);
            Assert.AreEqual(0, viewResult.ViewData.Count);
            Assert.AreEqual(new Uri("https://fake-test-webapp.azurewebsites.net/"), viewModel.LearningPlatformUrl);
        }

        [Test]
        public void PostIndexSelectedProductNotFound()
        {
            var viewModel = new ProductSelectionViewModel
            {
                ProductId = 1,
                ProductTypeId = 2
            };

            var cookieBannerViewModel = new CookieBannerViewModel();

            _cookieService
                .Setup(_ => _.SyncCookieSelection(It.IsAny<HttpRequest>(), It.IsAny<CookieBannerViewModel>()))
                .Returns(cookieBannerViewModel);

            var userVoucherDto = new UserVoucherDto();

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _productRepository.Setup(_ => _.GetProductSingle(1)).Throws(new Exception("fake error message"));

            var productTypes = new List<settings_product_type>
            {
                new() { id = 2 }
            };

            _productRepository.Setup(_ => _.ProductTypes()).Returns(Task.FromResult(productTypes));

            Assert.ThrowsAsync<Exception>(() => _sut.Index(viewModel));
        }

        [Test]
        public async Task GetIndexReturnsViewWithNoViewModel()
        {
            var viewModel = new ProductSelectionViewModel { 
                ProductId =1,
                ProductTypeId = 1
            };

            var expectedModel = await SetupSelection(_productRepository, 1, 1);
            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);
            _productRepository.Setup(x => x.GetProductSingle(1).Result).Returns(new product{product_id =1, vendor_id=1, product_type=1});

            var controllerResult = (ViewResult) await _sut.Index(viewModel);

            Assert.That(controllerResult.Model is HomeViewModel);
        }

        [Test]
        public async Task GetIndexWithoutProductIdRedirectsToActionSoftwareNotChosen()
        {
            var viewModel = new ProductSelectionViewModel
            {
                ProductTypeId = 1
            };
            var expectedModel = await SetupSelection(_productRepository, 1);
            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);
            var controllerResult = (RedirectToActionResult) await _sut.Index(viewModel);

            Assert.That(controllerResult.ActionName == "SoftwareNotChosen");
        }

        [Test]
        public async Task GetIndexWithoutProductTypeIdRedirectsToActionSoftwareNotChosen()
        {
            var viewModel = new ProductSelectionViewModel
            {
                ProductId = 1
            };
            var expectedModel = await SetupSelection(_productRepository, 0, 1);
            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);
            var controllerResult = (RedirectToActionResult) await _sut.Index(viewModel);

            Assert.That(controllerResult.ActionName == "SoftwareNotChosen");
        }

        [TestCase("fakeControllerName", "fakeControllerAction", false)]
        [TestCase("home", "index", true)]
        public async Task ProcessCookieActAccepted(string controllerName, string actionName, bool hasRouteValues)
        {
            var userVoucherDto = new UserVoucherDto { CookieBannerViewModel = new CookieBannerViewModel(), SelectedProduct = new product { product_id = 1 }, SelectedProductType = new settings_product_type { id = 2 } };
            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);
            _cookieService.Setup(_ => _.ProcessCookie("act", true, It.IsAny<HttpResponse>())).Returns(Task.FromResult(true));

            var result = await _sut.ProcessCookie(controllerName, actionName, "act", true);
            var actionResult = (RedirectToActionResult)result;

            Assert.True(userVoucherDto.CookieBannerViewModel.IsCookieProcessed);
            Assert.AreEqual(controllerName, actionResult.ControllerName);
            Assert.AreEqual(actionName, actionResult.ActionName);
            Assert.AreEqual(hasRouteValues, actionResult.RouteValues?.Count > 0);
        }

        [Test]
        public async Task ProcessCookieCloseAccepted()
        {
            var userVoucherDto = new UserVoucherDto { CookieBannerViewModel = new CookieBannerViewModel() };
            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);
            _cookieService.Setup(_ => _.ProcessCookie("close", true, It.IsAny<HttpResponse>())).Returns(Task.FromResult(true));

            var result = await _sut.ProcessCookie("fakeControllerName", "fakeControllerAction", "close", true);
            var actionResult = (RedirectToActionResult)result;

            Assert.True(userVoucherDto.CookieBannerViewModel.IsBannerClosed);
            Assert.AreEqual("fakeControllerName", actionResult.ControllerName);
            Assert.AreEqual("fakeControllerAction", actionResult.ActionName);
        }

        [Test]
        public async Task SaveCookiesPreferences()
        {
            var userVoucherDto = new UserVoucherDto();
            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);
            
            var viewModel = new CookieBannerViewModel();
            var result = await _sut.SaveCookiesPreferences(viewModel);
            var actionResult = (RedirectToActionResult) result;

            Assert.AreEqual("Cookies", actionResult.ActionName);
        }

        [Test]
        public void SoftwareNotChosen()
        {
            _sut = HomeControllerObject();

            var viewResult = (ViewResult)_sut.SoftwareNotChosen();
            var viewModel = viewResult.Model as HomeViewModel;
            
            Assert.NotNull(viewModel);
            Assert.IsEmpty(viewResult.ViewData);
            Assert.AreEqual("https://fake-test-webapp.azurewebsites.net/comparison-tool", $"{viewModel.LearningPlatformUrl}comparison-tool");
        }

        [Test]
        public void Cookies()
        {
            _sut = HomeControllerObject();

            var userVoucherDto = new UserVoucherDto { CookieBannerViewModel = new CookieBannerViewModel() };

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);

            var viewResult = (ViewResult)_sut.Cookies();

            Assert.That(viewResult.Model is CookieBannerViewModel);
        }

        [Test]
        public void Error()
        {
            _sut = HomeControllerObject();

            var viewResult = (ViewResult)_sut.Error();

            Assert.That(viewResult.Model is ErrorViewModel);
        }
    }
}