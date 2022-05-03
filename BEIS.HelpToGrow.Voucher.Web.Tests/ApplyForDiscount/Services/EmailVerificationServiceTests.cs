using System;
using System.Linq;
using System.Threading.Tasks;
using BEIS.HelpToGrow.Voucher.Web.Common;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using Beis.Htg.VendorSme.Database.Models;
using FluentResults;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class EmailVerificationServiceTests
    {
        private EmailVerificationService _sut;
        private Mock<ILogger<EmailVerificationService>> _mockLogger;
        private Mock<INotifyService> _mockNotifyService;
        private Mock<IEnterpriseService> _mockEnterpriseService;
        private Mock<IEncryptionService> _mockEncryptionService;
        private Mock<IConfiguration> _mockConfiguration;
        private string _salt;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<EmailVerificationService>>();
            _mockNotifyService = new Mock<INotifyService>();
            _mockEnterpriseService = new Mock<IEnterpriseService>();
            _mockEncryptionService = new Mock<IEncryptionService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _salt = "exampleemailvericationsalt";


            var inMemorySettings = new Dictionary<string, string>
            {
                {"EMAIL_VERIFICATION_SALT", "exampleemailvericationsalt"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _sut = new EmailVerificationService(
                _mockLogger.Object,
                _mockNotifyService.Object,
                _mockEnterpriseService.Object,
                _mockEncryptionService.Object,
                configuration);
        }

        [Test]
        public void GetVerificationCode()
        {
            const string expected = "some text";

            _mockEncryptionService
                .Setup(_ => _.Encrypt(It.IsAny<string>(), _salt))
                .Returns(expected);

            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { product_id = 11 }
            };

            var verificationCode = _sut.GetVerificationCode(userVoucherDto);

            Assert.AreEqual(expected, verificationCode);
        }

        [Test]
        public async Task CreateOrUpdateEnterpriseDetailsAsync()
        {
            await _sut.CreateOrUpdateEnterpriseDetailsAsync(new UserVoucherDto());

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Executing") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("completed") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SendVerifyEmailNotificationAsync()
        {
            await _sut.SendVerifyEmailNotificationAsync(new ApplicantDto());

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Executing") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("completed") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task VerifyEnterpriseFromCodeAsyncMissingEnterprise()
        {
            const string verificationCode = "fake-verification-code";

            _mockEncryptionService
                .Setup(_ => _.Decrypt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("{\"EnterpriseId\": 2, \"EmailAddress\": \"fake@address.org\", \"ProductId\": 3}");

            var result = await _sut.VerifyEnterpriseFromCodeAsync(verificationCode);

            Assert.That(result.IsFailed);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("The enterprise could not be found.", result.Errors.Single().Message);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains($"Attempting to verify email address for {verificationCode}") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task VerifyEnterpriseFromCodeAsyncInvalidVoucher()
        {
            const string verificationCode = "fake-verification-code";

            _mockEncryptionService
                .Setup(_ => _.Decrypt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("{\"EnterpriseId\": 0, \"EmailAddress\": \"fake@address.org\", \"ProductId\": 3}");

            var result = await _sut.VerifyEnterpriseFromCodeAsync(verificationCode);

            Assert.That(result.IsFailed);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("The voucher does not contain a valid Id.", result.Errors.Single().Message);
        }

        [Test]
        public async Task VerifyEnterpriseFromCodeAsyncDifferingEmailAddress()
        {
            const string verificationCode = "fake-verification-code";

            _mockEncryptionService
                .Setup(_ => _.Decrypt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("{\"EnterpriseId\": 1, \"EmailAddress\": \"fake@address.org\", \"ProductId\": 3}");

            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto { EmailAddress = "different@address.org" }
            };

            _mockEnterpriseService
                .Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(userVoucherDto));

            var result = await _sut.VerifyEnterpriseFromCodeAsync(verificationCode);

            Assert.That(result.IsFailed);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("The email address does not match.", result.Errors.Single().Message);
        }

        [Test]
        public async Task VerifyEnterpriseFromCodeAsyncDifferingEnterpriseId()
        {
            const string verificationCode = "fake-verification-code";

            _mockEncryptionService
                .Setup(_ => _.Decrypt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("{\"EnterpriseId\": 11, \"EmailAddress\": \"fake@address.org\", \"ProductId\": 3}");

            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto { EmailAddress = "fake@address.org" }
            };

            _mockEnterpriseService
                .Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(userVoucherDto));

            var result = await _sut.VerifyEnterpriseFromCodeAsync(verificationCode);

            Assert.That(result.IsFailed);
            Assert.AreEqual(1, result.Errors.Count);
            Assert.AreEqual("the Id does not match.", result.Errors.Single().Message);
        }

        [Test]
        public async Task VerifyEnterpriseFromCodeAsyncThrowsException()
        {
            const string verificationCode = "fake-verification-code";

            _mockEncryptionService
                .Setup(_ => _.Decrypt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("{\"EnterpriseId\": 12, \"EmailAddress\": \"fake@address.org\", \"ProductId\": 3}");

            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto
                {
                    EmailAddress = "fake@address.org",
                    EnterpriseId = 12
                }
            };

            _mockEnterpriseService
                .Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(userVoucherDto));

            _mockEnterpriseService
                .Setup(_ => _.SetEnterpriseAsVerified(It.IsAny<long>()))
                .Throws(new Exception("fake error message"));

            var result = await _sut.VerifyEnterpriseFromCodeAsync(verificationCode);

            Assert.That(result.IsFailed);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains($"There was a problem verifying the applicant address for code {verificationCode}") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task VerifyEnterpriseFromCodeAsync()
        {
            const string verificationCode = "fake-verification-code";

            _mockEncryptionService
                .Setup(_ => _.Decrypt(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("{\"EnterpriseId\": 12, \"EmailAddress\": \"fake@address.org\", \"ProductId\": 3}");

            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto
                {
                    EmailAddress = "fake@address.org",
                    EnterpriseId = 12
                }
            };

            _mockEnterpriseService
                .Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(userVoucherDto));

            _mockEnterpriseService
                .Setup(_ => _.SetEnterpriseAsVerified(It.IsAny<long>()))
                .Returns(Task.FromResult(Result.Ok()));

            var result = await _sut.VerifyEnterpriseFromCodeAsync(verificationCode);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public async Task FCANumberIsUnique()
        {
            _mockEnterpriseService
                .Setup(_ => _.FcaNumberIsUnique("fake FCA number"))
                .Returns(Task.FromResult(false));

            var isUnique = await _sut.CompanyNumberIsUnique(It.IsAny<string>(), "fake FCA number");

            Assert.False(isUnique);
        }

        [Test]
        public async Task CompanyNumberIsUnique()
        {
            _mockEnterpriseService
                .Setup(_ => _.CompanyNumberIsUnique("fake companies house number"))
                .Returns(Task.FromResult(false));

            var isUnique = await _sut.CompanyNumberIsUnique("fake companies house number", It.IsAny<string>());

            Assert.False(isUnique);
        }
    }
}