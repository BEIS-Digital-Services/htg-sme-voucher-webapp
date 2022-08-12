namespace Beis.HelpToGrow.Voucher.Web.Tests.Applicant
{
    [TestFixture]
    public class ApplicantPhoneNumberControllerTest : BaseControllerTest
    {
        private ApplicantPhoneNumberController _sut;
        private Mock<ISessionService> _mockSessionService;
        private ControllerContext _controllerContext;
        private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _controllerContext = SetupControllerContext(_controllerContext);
            _mockSessionService.Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns((string key, HttpContext _) => _memoryCache.Get<UserVoucherDto>(key));
            _mockSessionService.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), _controllerContext.HttpContext))
                .Callback((string s, object o, HttpContext _) => _memoryCache.Set(s, o as UserVoucherDto));
            _sut = new ApplicantPhoneNumberController(_mockSessionService.Object);
            _sut.ControllerContext = _controllerContext;
        }

        [Test]
        public void GetIndexHandlesSessionException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            Assert.Throws<Exception>(() => _sut.Index());
        }

        [Test]
        public void GetIndexHandlesMissingPhoneNumber()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var viewResult = (ViewResult)_sut.Index();

            Assert.Null(viewResult.ViewName);
        }

        [Test]
        public void GetIndex()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto { ApplicantDto = new ApplicantDto { PhoneNumber = "fake phone number" } });

            var viewResult = (ViewResult)_sut.Index();

            Assert.That(viewResult.Model is PhoneNumberViewModel);
        }

        [Test]
        public void PostIndexHandlesSessionException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));


            Assert.Throws<Exception>(() => _sut.Index(new PhoneNumberViewModel { PhoneNumber = "fake phone number" }));
        }

        [Test]
        public void PostIndexHandlesInvalidPhoneNumber()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var viewResult = (ViewResult)_sut.Index(new PhoneNumberViewModel { PhoneNumber = default });

            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [Test]
        [TestCase("07525252525")]       
        public void GetIndexSetsUserSessionDtoForAValidModel(string number)
        {
            _sut.Index(new PhoneNumberViewModel { PhoneNumber = number });
            var dtoResult = _mockSessionService.Object.Get<UserVoucherDto>("userVoucherDto", _controllerContext.HttpContext);

            Assert.IsNotNull(dtoResult);
            Assert.IsNotNull(dtoResult.ApplicantDto);
            Assert.IsNotNull(dtoResult.ApplicantDto.PhoneNumber);
        }

        [TestCase("07525252525")]
        [TestCase("+447525252525")]
        [TestCase("00447525252525")]
        [TestCase("01173027736")]
        [TestCase("00441173027736")]
        [TestCase("+441173027736")]        
        public void ValidationPasses(string number)
        {
            // Arrange
            var sut = new PhoneNumberViewModel { PhoneNumber = number };

            // Set some properties here
            var context = new ValidationContext(sut, null, null);
            var results = new List<ValidationResult>();

            // Act
            var modelIsValid = Validator.TryValidateObject(sut, context, results, true);

            // Assert 
            Assert.IsTrue(modelIsValid);
        }

        [TestCase(null, Description = "Null value")]
        [TestCase("", Description = "Empty string")]
        [TestCase("  ", Description = "White Space")]        
        public void ValidationFails(string number)
        {
            // Arrange
            var sut = new PhoneNumberViewModel { PhoneNumber = number };

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
