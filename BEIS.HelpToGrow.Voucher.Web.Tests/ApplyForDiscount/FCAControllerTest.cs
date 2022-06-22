
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class FCAControllerTest : BaseControllerTest
    {
        private FCAController _sut;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IEnterpriseRepository> _enterpriseRepository;
        private Mock<IProductRepository> _productRepository;
        private Mock<IFCASocietyRepository> _fcaSocietyRepository;
        private Mock<IFCASocietyService> _fcaSocietyService;
        private ControllerContext _controllerContext;
        private Mock<IApplicationStatusService> _mockApplicationStatusService;
        private ApplicationStatus _applicationStatus = ApplicationStatus.NewApplication;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _enterpriseRepository = new Mock<IEnterpriseRepository>();
            _productRepository = new Mock<IProductRepository>();
            _fcaSocietyRepository = new Mock<IFCASocietyRepository>();
            _fcaSocietyService = new Mock<IFCASocietyService>();
            _controllerContext = null;
            _controllerContext = SetupControllerContext(_controllerContext);
            SetupProductRepository(_productRepository);
            _mockApplicationStatusService = new Mock<IApplicationStatusService>();
            _mockApplicationStatusService.Setup(x => x.GetApplicationStatusForFcaNumber(It.IsAny<string>())).ReturnsAsync((string s1) => _applicationStatus);
            _mockApplicationStatusService.Setup(x => x.GetApplicationStatus(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string s1, string s2) => _applicationStatus);
            _sut = new FCAController(
                _mockSessionService.Object,
                _enterpriseRepository.Object,
                _fcaSocietyRepository.Object,
                _fcaSocietyService.Object, _mockApplicationStatusService.Object);

            _sut.ControllerContext = _controllerContext;
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult)_sut.Index();

            Assert.Null(viewResult.ViewName);
        }

        [Test]
        public void GetFCANumber()
        {
            var viewResult = (ViewResult)_sut.GetFCANumber();

            Assert.That(viewResult.Model is FCAViewModel);
        }

        [Test]
        public async Task InvalidFCANumber()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var enterprise = new enterprise();

            _enterpriseRepository
                .Setup(_ => _.GetEnterpriseByFCANumber(It.IsAny<string>()))
                .Returns(Task.FromResult(enterprise));

            var viewModel = new FCAViewModel();

            var result = await _sut.GetFCANumber(viewModel);

            var viewResult = (ViewResult) result;

            Assert.AreEqual(viewModel, viewResult.Model);
        }

        [Test]
        public async Task GetFCANumberNotFound()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            _enterpriseRepository
                .Setup(_ => _.GetEnterpriseByFCANumber(It.IsAny<string>()))
                .Returns(Task.FromResult((enterprise)null));

            var viewModel = new FCAViewModel { FCAFullRegistrationNumber = "fake registration number" };

            var result = await _sut.GetFCANumber(viewModel);

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("FCA", actionResult.ControllerName);
            Assert.AreEqual("FCANumberNotFound", actionResult.ActionName);
        }

        [Test]
        public async Task GetFCANumberDeregistered()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            _enterpriseRepository
                .Setup(_ => _.GetEnterpriseByFCANumber(It.IsAny<string>()))
                .Returns(Task.FromResult((enterprise)null));

            var fcaSociety = new fcasociety { society_status = "deregistered" };

            _fcaSocietyRepository
                .Setup(_ => _.GetFCASociety(It.IsAny<string>()))
                .Returns(Task.FromResult(fcaSociety));

            var viewModel = new FCAViewModel { FCAFullRegistrationNumber = "fake registration number" };

            var result = await _sut.GetFCANumber(viewModel);

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("InEligible", actionResult.ControllerName);
            Assert.AreEqual("Deregistered", actionResult.ActionName);
        }

        [Test]
        public async Task GetFCANumberSuccess()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            _enterpriseRepository
                .Setup(_ => _.GetEnterpriseByFCANumber(It.IsAny<string>()))
                .Returns(Task.FromResult((enterprise)null));

            var fcaSociety = new fcasociety { society_status = "something other than deregistered" };

            _fcaSocietyRepository
                .Setup(_ => _.GetFCASociety(It.IsAny<string>()))
                .Returns(Task.FromResult(fcaSociety));

            var viewModel = new FCAViewModel { FCAFullRegistrationNumber = "fake registration number" };

            var result = await _sut.GetFCANumber(viewModel);

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("FCA", actionResult.ControllerName);
            Assert.AreEqual("CheckCompanyDetails", actionResult.ActionName);
        }

        [Test]
        public async Task SaveFCANumber()
        {
            _applicationStatus = ApplicationStatus.EmailVerified;
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var enterprise = new enterprise();

            _enterpriseRepository
                .Setup(_ => _.GetEnterpriseByFCANumber(It.IsAny<string>()))
                .Returns(Task.FromResult(enterprise));

            var viewModel = new FCAViewModel { FCAFullRegistrationNumber = "123456" };

            var result = await _sut.GetFCANumber(viewModel);

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("FCA", actionResult.ControllerName);
            Assert.AreEqual("VoucherAlreadyApplied", actionResult.ActionName);
        }

        [Test]
        public void VoucherAlreadyApplied()
        {
            var viewResult = (ViewResult)_sut.VoucherAlreadyApplied();

            Assert.Null(viewResult.ViewName);
        }

        [Test]
        public void FCANumberNotFound()
        {
            var viewResult = (ViewResult)_sut.FCANumberNotFound();

            Assert.Null(viewResult.ViewName);
        }

        [Test]
        public async Task CheckCompanyDetailsAsync()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var fcaSociety = new FCASociety();

            _fcaSocietyService
                .Setup(_ => _.GetSociety(It.IsAny<string>()))
                .Returns(Task.FromResult(fcaSociety));

            var checkCompanyDetails = await _sut.CheckCompanyDetails();
            var viewResult = (ViewResult)checkCompanyDetails;

            Assert.AreEqual(fcaSociety, viewResult.Model);
        }

        [Test]
        public async Task GetIndexReturnsFCANumberViewModel()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1, "","","","Yes");
            
            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new FCAController(_mockSessionService.Object, _enterpriseRepository.Object, _fcaSocietyRepository.Object, _fcaSocietyService.Object, _mockApplicationStatusService.Object);
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (ViewResult)_sut.Index();
            
            Assert.IsTrue(controllerResult.Model.GetType() == typeof(FCANumberViewModel));
        }

        [Test]
        public async Task PostForwardInvalidModel()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new FCAController(_mockSessionService.Object, _enterpriseRepository.Object, _fcaSocietyRepository.Object, _fcaSocietyService.Object, _mockApplicationStatusService.Object);
            _sut.ControllerContext = _controllerContext;

            var viewResult = (ViewResult)_sut.Forward(new FCANumberViewModel());

            Assert.That(viewResult.ViewName == "Index");
            Assert.That(viewResult.Model is FCANumberViewModel);
        }

        [Test]
        public async Task CallForwardWithYesRedirectToFCAControllerWithActionNameAsGetFCANumber()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new FCAController(_mockSessionService.Object, _enterpriseRepository.Object, _fcaSocietyRepository.Object, _fcaSocietyService.Object, _mockApplicationStatusService.Object);
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.Forward(new FCANumberViewModel {HasFCANumber = "Yes" });

            Assert.That(controllerResult.ControllerName == "FCA");
            Assert.That(controllerResult.ActionName == "GetFCANumber");
        }

        [Test]
        public async Task CallForwardWithNoRedirectToFCAIneligibleControllerWithActionNameAsIndex()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);
            _sut = new FCAController(_mockSessionService.Object, _enterpriseRepository.Object, _fcaSocietyRepository.Object, _fcaSocietyService.Object, _mockApplicationStatusService.Object);
            _sut.ControllerContext = _controllerContext;

            var controllerResult = (RedirectToActionResult)_sut.Forward(new FCANumberViewModel { HasFCANumber = "No" });

            Assert.That(controllerResult.ControllerName == "Ineligible");
            Assert.That(controllerResult.ActionName == "FCA");
        }

        [Test]
        public async Task CallForwardWithNoRedirectToFCAControllerWithActionNameAsVoucherAlreadyApplied()
        {
            _applicationStatus = ApplicationStatus.CancelledCannotReApply;

            var fcaSociety = new FCASociety();

            _fcaSocietyService
                .Setup(_ => _.GetSociety(It.IsAny<string>()))
                .Returns(Task.FromResult(fcaSociety));

            var enterprise = new enterprise();

            _enterpriseRepository
                .Setup(_ => _.GetEnterpriseByFCANumber(It.IsAny<string>()))
                .Returns(Task.FromResult(enterprise));

            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new FCAController(
                _mockSessionService.Object,
                _enterpriseRepository.Object,
                _fcaSocietyRepository.Object,
                _fcaSocietyService.Object,
                _mockApplicationStatusService.Object);

            _sut.ControllerContext = _controllerContext;

            var model = new FCAViewModel { FCAFullRegistrationNumber = "123456" };
            var result = await _sut.GetFCANumber(model);

            Assert.That(result is RedirectToActionResult);
            Assert.That((result as RedirectToActionResult).ControllerName == "FCA");
            Assert.That((result as RedirectToActionResult).ActionName == "VoucherAlreadyApplied");
        }

        [Test]
        public async Task CallGetFCANumberWithFCANumberRedirectToCompaniesHouseControllerWithActionNameAsCheckCompanyDetails()
        {
            var expectedModel = await SetupSelection(_productRepository, 1, 1);

            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext)).Returns(expectedModel);

            _sut = new FCAController(
                _mockSessionService.Object,
                _enterpriseRepository.Object,
                _fcaSocietyRepository.Object,
                _fcaSocietyService.Object,
                _mockApplicationStatusService.Object);
            
            _sut.ControllerContext = _controllerContext;

            var model = new FCAViewModel { FCAFullRegistrationNumber = "123456" };
            var result = await _sut.GetFCANumber(model);

            Assert.That(((RedirectToActionResult)result).ControllerName == "FCA");
            Assert.That(((RedirectToActionResult)result).ActionName == "FCANumberNotFound");
        }  
    }
}