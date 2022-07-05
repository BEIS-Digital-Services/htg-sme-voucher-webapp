
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class NotifyServiceTests
    {
        private NotifyService _sut;
        private Mock<ILogger<NotifyService>> _mockLogger;
        private IOptions<NotifyServiceSettings> _mockOptions;
        private Mock<IWebHostEnvironment> _mockHostEnvironment;
        private Mock<IEmailClientService> _mockEmailClient;

        [SetUp]
        public void Setup()
        {
            _mockOptions = Options.Create<NotifyServiceSettings>(new NotifyServiceSettings { IssueTokenTemplateId  = "fake template id" , EmailVerificationUrl = "https://localhost:44326/VerifyEmailAddress" });

            _mockLogger = new Mock<ILogger<NotifyService>>();
            _mockHostEnvironment = new Mock<IWebHostEnvironment>();
            _mockEmailClient = new Mock<IEmailClientService>();
            _sut = new NotifyService(
                _mockLogger.Object,
                _mockOptions,
                _mockEmailClient.Object,
                _mockHostEnvironment.Object);
        }

        [Test]
        public async Task SendVoucherToApplicantMissingTokenPurchaseLink()
        {
            var result = await _sut.SendVoucherToApplicant(new UserVoucherDto());

            Assert.That(result.IsFailed);
        }

        [Test]
        public async Task SendVoucherToApplicant()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto {EmailAddress = "fake@domain.org"},
                SelectedProduct = new product(),
                tokenPurchaseLink = "https://fake-link.org"
            };

            var result = await _sut.SendVoucherToApplicant(userVoucherDto);

            Assert.That(result.IsSuccess);

            _mockEmailClient.Verify(_ =>
                _.SendEmailAsync(
                    "fake@domain.org",
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()));
        }

        [Test]
        public async Task SendVoucherToApplicantHandlesException()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto {EmailAddress = "fake@domain.org"},
                SelectedProduct = new product(),
                tokenPurchaseLink = "https://fake-link.org"
            };

            _mockEmailClient
                .Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<Dictionary<string, object>>()))
                .Throws(new Exception("fake error message"));

            var result = await _sut.SendVoucherToApplicant(userVoucherDto);

            Assert.That(result.IsFailed);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) =>
                        @object.ToString().Contains("There was a problem sending the voucher email") &&
                        type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SendVerifyEmailNotification()
        {
            var applicant = new ApplicantDto();
            var templateId = "fake template Id";

            var result = await _sut.SendVerifyEmailNotification(applicant, templateId);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public async Task SendVerifyEmailNotificationHandlesProductionClientException()
        {
            var applicant = new ApplicantDto();
            var templateId = "fake template Id";

            _mockEmailClient
                .Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Throws(new NotifyClientException("teamid"));

            var result = await _sut.SendVerifyEmailNotification(applicant, templateId);

            Assert.That(result.IsFailed);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) =>
                        @object.ToString().Contains("There was a problem sending the Verify Email Notification") &&
                        type.Name == "FormattedLogValues"),
                    It.IsAny<NotifyClientException>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SendVerifyEmailNotificationHandlesProductionException()
        {
            var applicant = new ApplicantDto();
            var templateId = "fake template Id";

            _mockEmailClient
                .Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Throws(new Exception("fake error message"));

            var result = await _sut.SendVerifyEmailNotification(applicant, templateId);

            Assert.That(result.IsFailed);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) =>
                        @object.ToString().Contains("There was a problem sending the Verify Email Notification") &&
                        type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        [Ignore("unable to mock IsDevelopment()")]
        public async Task SendVerifyEmailNotificationHandlesDevelopmentClientException()
        {
            var applicant = new ApplicantDto();
            var templateId = "fake template Id";

            _mockHostEnvironment
                .Setup(_ => _.IsDevelopment())
                .Returns(true);

            _mockEmailClient
                .Setup(_ => _.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
                .Throws(new NotifyClientException("teamid"));

            var result = await _sut.SendVerifyEmailNotification(applicant, templateId);

            Assert.That(result.IsFailed);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) =>
                        @object.ToString().Contains("Exception ignored in development") &&
                        type.Name == "FormattedLogValues"),
                    It.IsAny<NotifyClientException>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}