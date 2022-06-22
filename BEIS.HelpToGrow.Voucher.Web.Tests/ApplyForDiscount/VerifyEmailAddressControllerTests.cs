
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class VerifyEmailAddressControllerTests : BaseControllerTest
    {
        private VerifyEmailAddressController _sut;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IEmailVerificationService> _mockEmailVerificationService;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _mockEmailVerificationService = new Mock<IEmailVerificationService>();
            _sut = new VerifyEmailAddressController(_mockSessionService.Object, _mockEmailVerificationService.Object);
        }

        [Test]
        public async Task IndexSuccess()
        {
            const string fakeVerificationCode = nameof(fakeVerificationCode);

            _mockEmailVerificationService.Setup(_ => _.VerifyEnterpriseFromCodeAsync(fakeVerificationCode)).Returns(Task.FromResult(Result.Ok()));

            var result = await _sut.Index(fakeVerificationCode);

            var viewResult = (ViewResult) result;

            Assert.AreEqual("Success", viewResult.ViewName);
        }

        [Test]
        public async Task IndexInvalidCode()
        {
            const string fakeVerificationCode = nameof(fakeVerificationCode);

            _mockEmailVerificationService.Setup(_ => _.VerifyEnterpriseFromCodeAsync(fakeVerificationCode)).Returns(Task.FromResult(Result.Fail("fake error message")));

            var result = await _sut.Index(fakeVerificationCode);

            var viewResult = (ViewResult) result;

            Assert.AreEqual("InvalidCode", viewResult.ViewName);
        }

        [Test]
        public void IndexHandlesException()
        {
            const string fakeVerificationCode = nameof(fakeVerificationCode);

            _mockEmailVerificationService
                .Setup(_ => _.VerifyEnterpriseFromCodeAsync(fakeVerificationCode))
                .Throws(new Exception("fake error message"));

            Assert.ThrowsAsync<Exception>(() => _sut.Index(fakeVerificationCode));
        }

        [Test]
        public void Success()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto
                {
                    IsVerified = true
                }
            };

            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);

            var viewResult = (ViewResult) _sut.Success().Result;

            Assert.Null(viewResult.ViewName);
        }

        [Test]
        public void Error()
        {
            var userVoucherDto = new UserVoucherDto();

            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);

            var viewResult = (ViewResult) _sut.Success().Result;

            Assert.AreEqual("Error", viewResult.ViewName);
        }

        [Test]
        public void ConfirmVerified()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto
                {
                    IsVerified = true
                }
            };

            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);

            var viewResult = (ViewResult) _sut.ConfirmVerified().Result;

            Assert.AreEqual("Success", viewResult.ViewName);
        }

        [Test]
        public void NotVerified()
        {
            var userVoucherDto = new UserVoucherDto();

            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);

            var viewResult = (ViewResult)_sut.ConfirmVerified().Result;

            Assert.AreEqual("NotVerified", viewResult.ViewName);
        }
    }
}