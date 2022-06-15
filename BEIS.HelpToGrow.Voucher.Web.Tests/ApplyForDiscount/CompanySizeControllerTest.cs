
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class CompanySizeControllerTest : BaseControllerTest
    {
        private CompanySizeController _sut;
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

            _sut = new CompanySizeController(_mockSessionService.Object);
        }

        [Test]
        public void IndexHandlesSessionException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            Assert.Throws<Exception>(() => _sut.Index());
        }
        
        [Test]
        public void Index()
        {
            var userVoucherDto = new UserVoucherDto { EmployeeNumbers = int.MaxValue };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewResult = (ViewResult)_sut.Index();

            Assert.That(viewResult.Model is CompanySizeViewModel);
            Assert.AreEqual(int.MaxValue, ((CompanySizeViewModel)viewResult.Model).EmployeeNumbers);
        }

        [Test]
        public void BackHandlesSessionException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            Assert.Throws<Exception>(() => _sut.Back());
        }

        [Test]
        public void BackFirstTime()
        {
            var userVoucherDto = new UserVoucherDto { FirstTime = "yes" };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var actionResult = (RedirectToActionResult)_sut.Back();

            Assert.AreEqual("ExistingCustomer", actionResult.ControllerName);
            Assert.AreEqual("Index", actionResult.ActionName);
        }

        [Test]
        public void BackNewCustomer()
        {
            var userVoucherDto = new UserVoucherDto { ExistingCustomer = "no" };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var actionResult = (RedirectToActionResult)_sut.Back();

            Assert.AreEqual("ExistingCustomer", actionResult.ControllerName);
            Assert.AreEqual("Index", actionResult.ActionName);
        }

        [Test]
        public void BackExistingCustomer()
        {
            var userVoucherDto = new UserVoucherDto { ExistingCustomer = "yes" };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var actionResult = (RedirectToActionResult)_sut.Back();

            Assert.AreEqual("MajorUpgrade", actionResult.ControllerName);
            Assert.AreEqual("Index", actionResult.ActionName);
        }

        [Test]
        public void BackMajorUpgrade()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ExistingCustomer = "yes",
                MajorUpgrade = "yes"
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var actionResult = (RedirectToActionResult)_sut.Back();

            Assert.AreEqual("MajorUpgrade", actionResult.ControllerName);
            Assert.AreEqual("Index", actionResult.ActionName);
        }

        [Test]
        public void PostIndexHandlesSessionException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            Assert.Throws<Exception>(() => _sut.Index(new CompanySizeViewModel { EmployeeNumbers = 10 }));
        }

        [Test]
        public void PostIndexMissingEmployeeNumbers()
        {
            var userVoucherDto = new UserVoucherDto();

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewModel = new CompanySizeViewModel { EmployeeNumbers = default };
            var viewResult = (ViewResult)_sut.Index(viewModel);

            Assert.That(viewResult.Model is CompanySizeViewModel);
        }

        [Test]
        public async Task CallBackWithFirstTimeAsYesRedirectToNewToSoftwareWithActionNameAsIndex()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1, "Yes");

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new CompanySizeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.Back();

            Assert.That(controllerResult.ControllerName == "ExistingCustomer");
            Assert.That(controllerResult.ActionName == "Index");
        }

        [Test]
        public async Task PostFormWithIncorrectLowerEmployeeNumbersRedirectToInEligibleCompanySize()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new CompanySizeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.Index(new CompanySizeViewModel {EmployeeNumbers =  4});

            Assert.That(controllerResult.ControllerName == "InEligible");
            Assert.That(controllerResult.ActionName == "CompanySize");
        }

        [Test]
        public async Task PostFormWithIncorrectHigherEmployeeNumbersRedirectToInEligibleCompanySize()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new CompanySizeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.Index(new CompanySizeViewModel { EmployeeNumbers = 250 });

            Assert.That(controllerResult.ControllerName == "InEligible");
            Assert.That(controllerResult.ActionName == "CompanySize");
        }

        [Test]
        public async Task PostFormWithCorrectEmployeeNumbersRedirectToCompaniesHouseIndexPage()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1, "Yes","","","Yes");

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new CompanySizeController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.Index(new CompanySizeViewModel { EmployeeNumbers = 5 });

            Assert.That(controllerResult.ControllerName == "CompaniesHouse");
            Assert.That(controllerResult.ActionName == "Index");
        }
    }
}