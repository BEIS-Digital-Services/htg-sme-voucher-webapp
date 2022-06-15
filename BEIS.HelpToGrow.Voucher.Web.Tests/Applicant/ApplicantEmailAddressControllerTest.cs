


namespace Beis.HelpToGrow.Voucher.Web.Tests.Applicant
{
    [TestFixture]
    public class ApplicantEmailAddressControllerTest : BaseControllerTest
    {
        private ApplicantEmailAddressController _sut;
        private Mock<ISessionService> _mockSessionService;
        private ControllerContext _controllerContext;
        private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
        private Mock<IEmailVerificationService> _mockEmailVerificationService;
        private Mock<IApplicationStatusService> _mockApplicationStatusService;
        private ApplicationStatus _applicationStatus = ApplicationStatus.NewApplication;
        private IOptions<UrlOptions> _options;

        [SetUp]
        public void Setup()
        {
            _options = Options.Create(new UrlOptions { EmailVerificationUrl = "https://localhost:44326/VerifyEmailAddress" });
            _mockSessionService = new Mock<ISessionService>();
            _controllerContext = SetupControllerContext(_controllerContext);
            
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns((string key, HttpContext _) => _memoryCache.Get<UserVoucherDto>(key));
            
            _mockSessionService
                .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), _controllerContext.HttpContext))
                .Callback((string s, object o, HttpContext _) => _memoryCache.Set(s, o as UserVoucherDto));
            
            _mockEmailVerificationService = new Mock<IEmailVerificationService>();
            _mockEmailVerificationService.Setup(x => x.SendVerifyEmailNotificationAsync(It.IsAny<ApplicantDto>(), null))
                .ReturnsAsync(Result.Ok());
            _mockApplicationStatusService = new Mock<IApplicationStatusService>();
            _mockApplicationStatusService.Setup(x => x.GetApplicationStatus(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string s1, string s2) =>  _applicationStatus);
        }

        [Test]
        public void IndexMissingSession()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            Assert.Throws<Exception>(() => _sut.Index());
        }

        [Test]
        public void IndexMissingEmailAddress()
        {
            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);
                                                                                                                                                             
            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            var viewResult = (ViewResult)_sut.Index();

            Assert.False(viewResult.ViewData.Any());
        }

        [Test]
        public void Index()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto { EmailAddress = "fake.email@address.org" }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            var viewResult = (ViewResult)_sut.Index();

            Assert.That(viewResult.Model is EmailAddressViewModel);
            Assert.AreEqual("fake.email@address.org", ((EmailAddressViewModel)viewResult.Model).EmailAddress);
        }

        [Test]
        public void InvalidModel()
        {
            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            var model = new EmailAddressViewModel { EmailAddress = string.Empty };
            var viewResult = (ViewResult)_sut.Index(model);

            Assert.AreEqual("Index", viewResult.ViewName);
            Assert.That(viewResult.Model is EmailAddressViewModel);
        }

        [Test]
        public void IndexException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            var model = new EmailAddressViewModel { EmailAddress = "fake.email@address.org" };

            Assert.Throws<Exception>(() => _sut.Index(model));
        }

        [Test]
        public void CheckEmailAddressMissingSessionException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            Assert.ThrowsAsync<Exception>(() => _sut.CheckEmailAddress());
        }

        [Test]
        public async Task CheckEmailAddressMissingEmailAddress()
        {
            var userVoucherDto = new UserVoucherDto();

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            var result = await _sut.CheckEmailAddress();
            var viewResult = (ViewResult) result;
            
            Assert.Null(viewResult.ViewName);
            Assert.That(viewResult.Model is EmailAddressViewModel);
            Assert.Null(((EmailAddressViewModel)viewResult.Model).EmailAddress);
        }

        [Test]
        public async Task CheckEmailAddressApplicationAlreadyExists()
        {
            _applicationStatus = ApplicationStatus.EmailNotVerified;
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto =
                {
                    EmailAddress = "fake.email@address.org",
                    IsVerified = false,
                    EnterpriseId = 0
                }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _mockEmailVerificationService
                .Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            var result = await _sut.CheckEmailAddress();
            var viewResult = (ViewResult)result;

            Assert.AreEqual("CompanyAlreadyExists", viewResult.ViewName);
        }

        [Test]
        public async Task CheckApplicationCancelledCannotReapply()
        {
            _applicationStatus = ApplicationStatus.CancelledCannotReApply;
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto =
                {
                    EmailAddress = "fake.email@address.org",
                    IsVerified = false,
                    EnterpriseId = 0
                }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _mockEmailVerificationService
                .Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(false));

            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            var result = await _sut.CheckEmailAddress();
            var viewResult = (ViewResult)result;

            Assert.AreEqual("CompanyAlreadyExists", viewResult.ViewName);
        }



        [Test]
        public async Task CheckEmailAddressFailedSavingEnterprise()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto =
                {
                    EmailAddress = "fake.email@address.org",
                    IsVerified = false,
                    EnterpriseId = 0
                }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _mockEmailVerificationService
                .Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var updateEnterpriseResult = Result.Fail("fake error message");

            _mockEmailVerificationService
                .Setup(_ => _.CreateOrUpdateEnterpriseDetailsAsync(It.IsAny<UserVoucherDto>()))
                .Returns(Task.FromResult(updateEnterpriseResult));

            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            var result = await _sut.CheckEmailAddress();

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("error", actionResult.ActionName);
        }

        [Test]
        public async Task CheckEmailAddress()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto =
                {
                    EmailAddress = "fake.email@address.org",
                    IsVerified = false,
                    EnterpriseId = 0
                }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _mockEmailVerificationService
                .Setup(_ => _.CompanyNumberIsUnique(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            var updateEnterpriseResult = Result.Ok();

            _mockEmailVerificationService
                .Setup(_ => _.CreateOrUpdateEnterpriseDetailsAsync(It.IsAny<UserVoucherDto>()))
                .Returns(Task.FromResult(updateEnterpriseResult));

            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);

            var result = await _sut.CheckEmailAddress();

            var viewResult = (ViewResult) result;

            Assert.That(viewResult.Model is EmailAddressViewModel);
        }

        [Test]
        [TestCase("test.email@test.gov.uk")]
        public void GetIndexSetsUserSessionDtoForAValidModel(string name)
        {
            //Arrange
            _sut = new ApplicantEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object, _mockApplicationStatusService.Object, _options);
            _sut.ControllerContext = _controllerContext;

            //Act
            _sut.Index(new EmailAddressViewModel { EmailAddress = name });
            var dtoResult = _mockSessionService.Object.Get<UserVoucherDto>("userVoucherDto", _controllerContext.HttpContext);

            //Assert
            Assert.IsNotNull(dtoResult);
            Assert.IsNotNull(dtoResult.ApplicantDto);
            Assert.IsNotNull(dtoResult.ApplicantDto.EmailAddress);
        }

        [TestCase("test.email@test.gov.uk")]
        public void ValidationPasses(string emailAddress)
        {
            // we can assume this has already been tested by the .net 5 team

            // Arrange
            var sut = new EmailAddressViewModel { EmailAddress = emailAddress };
            // Set some properties here
            var context = new ValidationContext(sut, null, null);
            var results = new List<ValidationResult>();
            //Act
            var modelIsValid = Validator.TryValidateObject(sut, context, results, true);

            // Assert 
            Assert.IsTrue(modelIsValid);
        }

        [TestCase(null, Description = "Null value")]
        [TestCase("", Description = "Empty string")]
        [TestCase("  ", Description = "White Space")]     
        [TestCase("/paulc.")]
        [TestCase("@paulc", Description = "no user or domain")]
        [TestCase("paul_test.beis.gov.uk", Description = "underscore")]
        [TestCase("paul_test.beis")]
        [TestCase("paul_test@beis@gov")]
        [TestCase("paul_test@@beis.gov")]
        [TestCase("@paul_test.beis.gov")]

        public void ValidationFails(string emailAddress)
        {
            // we can assume this has already been tested by the .net 5 team

            //Arrange
            var sut = new EmailAddressViewModel { EmailAddress = emailAddress };
            // Set some properties here
            var context = new ValidationContext(sut, null, null);
            var results = new List<ValidationResult>();
            //Act
            var modelIsValid = Validator.TryValidateObject(sut, context, results, true);

            // Assert 
            Assert.IsFalse(modelIsValid);
        }
    }
}