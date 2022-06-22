
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class NewToSoftwareControllerTest : BaseControllerTest
    {
        private NewToSoftwareController _sut;
        private Mock<ILogger<NewToSoftwareController>> _mockLogger;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IProductRepository> _productRepository;
        private ControllerContext _controllerContext;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<NewToSoftwareController>>();
            _mockSessionService = new Mock<ISessionService>();
            _productRepository = new Mock<IProductRepository>();
            _controllerContext = SetupControllerContext(_controllerContext);
            SetupProductRepository(_productRepository);

            _sut = new NewToSoftwareController(_mockLogger.Object, _mockSessionService.Object, Options.Create(new UrlOptions { LearningPlatformUrl = "https://localhost:/"}))
            {
                ControllerContext = _controllerContext
            };
        }

        [Test]
        public void GetIndexHandlesSessionException()
        {
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            Assert.Throws<Exception>(() => _sut.Index());
        }

        [Test]
        public void GetIndexHandlesMissingSelectedProduct()
        {
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto { SelectedProduct = null });

            var actionResult = (RedirectToActionResult)_sut.Index();

            Assert.AreEqual("Home", actionResult.ControllerName);
            Assert.AreEqual("SoftwareNotChosen", actionResult.ActionName);
        }

        [Test]
        public void PostForwardMissingFirstTime()
        {
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto { SelectedProduct = null });

            var viewResult = (ViewResult) _sut.Forward(new NewToSoftwareViewModel());
            Assert.AreEqual("Index", viewResult.ViewName);
            var model = viewResult.Model as NewToSoftwareViewModel;
            Assert.NotNull(model);
            Assert.AreEqual("https://localhost:/", model.LearningPlatformUrl);
        }

        [Test]
        public async Task GetIndexReturnsUserVoucherDtoModelWithSelectedProductIdAsOne()
        {
            var expectedModel = await SetupSelection(_productRepository, 1,1);
            
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var controllerResult = (ViewResult)_sut.Index();

            Assert.NotNull(controllerResult);
            var model = controllerResult.Model as NewToSoftwareViewModel;
            Assert.NotNull(model);
            Assert.That(model.SelectedProduct.product_id == 1);
            Assert.AreEqual("https://localhost:/", model.LearningPlatformUrl);
        }

        [Test]
        public async Task CallForwardWithYesRedirectToCompanySizeControllerWithActionNameAsIndex()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var model = new NewToSoftwareViewModel { FirstTime = "Yes" };
            var controllerResult = (RedirectToActionResult)_sut.Forward(model);

            Assert.That(controllerResult.ControllerName == "ExistingCustomer");
            Assert.That(controllerResult.ActionName == "Index");
        }

        [Test]
        public async Task CallForwardWithNoRedirectToMajorUpgradeControllerWithActionNameAsIndex()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns(expectedModel);

            var model = new NewToSoftwareViewModel { FirstTime = "No" };
            var controllerResult = (RedirectToActionResult)_sut.Forward(model);

            Assert.That(controllerResult.ControllerName == "InEligible");
            Assert.That(controllerResult.ActionName == "NotFirstTime");
        }
    }
}