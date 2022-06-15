using System;
using System.Threading.Tasks;

using Beis.HelpToGrow.Voucher.Web.Models.Voucher;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Web.Services;
using Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using Beis.HelpToGrow.Voucher.Web.Services.FCAServices;

namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class EnterpriseServiceTests
    {
        private EnterpriseService _sut;
        private Mock<IEnterpriseRepository> _mockEnterpriseRepo;
        private Mock<IEnterpriseSizeRepository> _mockEnterpriseSizeRepo;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<ILogger<EnterpriseService>> _mockLogger;
        private Mock<IProductRepository> _mockProductRepo;
        private Mock<IFCASocietyService> _mockFcaService;

        [SetUp]
        public void Setup()
        {
            _mockEnterpriseRepo = new Mock<IEnterpriseRepository>();
            _mockEnterpriseSizeRepo = new Mock<IEnterpriseSizeRepository>();
            _mockSessionService = new Mock<ISessionService>();
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _mockLogger = new Mock<ILogger<EnterpriseService>>();
            _mockProductRepo = new Mock<IProductRepository>();
            _mockFcaService = new Mock<IFCASocietyService>();

            _sut = new EnterpriseService(
                _mockEnterpriseRepo.Object,
                _mockEnterpriseSizeRepo.Object,
                _mockSessionService.Object,
                _mockHttpContextAccessor.Object,
                _mockLogger.Object,
                _mockProductRepo.Object,
                _mockFcaService.Object);
        }

        [Test]
        public async Task SetEligibilityStatusAsyncHandlesException()
        {
            var enterprise = new enterprise { enterprise_name = "fake company name" };

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            _mockEnterpriseRepo
                .Setup(_ => _.UpdateEnterprise(enterprise))
                .Throws(new Exception("fake error message"));

            var result = await _sut.SetEligibilityStatusAsync(EligibilityStatus.ReviewRequired);

            Assert.That(result.IsFailed);
        }

        [Test]
        public async Task SetEligibilityStatusAsync()
        {
            var enterprise = new enterprise { enterprise_name = "fake company name" };

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var result = await _sut.SetEligibilityStatusAsync(EligibilityStatus.ReviewRequired);

            _mockEnterpriseRepo.Verify(_ => _.UpdateEnterprise(enterprise), Times.Once);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public async Task GetUserVoucherFromEnterpriseAsyncWithProductId()
        {
            var enterprise = new enterprise { enterprise_name = "fake company name" };

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var voucher = await _sut.GetUserVoucherFromEnterpriseAsync(8, 16);

            Assert.AreEqual(8, voucher.ApplicantDto.EnterpriseId);
            Assert.AreEqual("fake company name", voucher.CompanyHouseResponse.CompanyName);
        }

        [Test]
        public async Task SetEnterpriseAsVerifiedHandlesException()
        {
            var enterprise = new enterprise { enterprise_name = "fake company name" };

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            var result = await _sut.SetEnterpriseAsVerified(11);

            Assert.That(result.IsFailed);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Error verifying the enterprise") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task SetEnterpriseAsVerified()
        {
            var enterprise = new enterprise { enterprise_name = "fake company name" };

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var result = await _sut.SetEnterpriseAsVerified(11);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public async Task CompanyNumberIsUnique()
        {
            var enterprise = new enterprise();

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterpriseByCompanyNumber(It.IsAny<string>()))
                .Returns(Task.FromResult(enterprise));

            var result = await _sut.CompanyNumberIsUnique("fake companies house number");

            Assert.False(result);
        }

        [Test]
        public async Task FcaNumberIsUnique()
        {
            var enterprise = new enterprise();

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterpriseByFCANumber(It.IsAny<string>()))
                .Returns(Task.FromResult(enterprise));

            var result = await _sut.FcaNumberIsUnique("fake companies house number");

            Assert.False(result);
        }

        [Test]
        public async Task GetEligibilityStatusAsyncMissingVoucher()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns((UserVoucherDto)null);

            var result = await _sut.GetEligibilityStatusAsync();

            Assert.AreEqual(EligibilityStatus.Unknown, result);
        }

        [Test]
        public async Task GetEligibilityStatusAsyncBadEnterpriseId()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto { ApplicantDto = new ApplicantDto { EnterpriseId = -1 }});

            var result = await _sut.GetEligibilityStatusAsync();

            Assert.AreEqual(EligibilityStatus.Unknown, result);
        }

        [Test]
        public void GetEligibilityStatusAsyncHandlesException()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto { EnterpriseId = 3 }
            };

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterprise(3))
                .Throws(new Exception("fake error message"));

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            Assert.ThrowsAsync<Exception>(() => _sut.GetEligibilityStatusAsync());
            
            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Error getting eligibility status for enterprise id") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetEligibilityStatusAsync()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto { EnterpriseId = 3 }
            };

            var enterprise = new enterprise
            {
                eligibility_status_id = (long) EligibilityStatus.ReviewRequired
            };

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterprise(3))
                .Returns(Task.FromResult(enterprise));

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var result = await _sut.GetEligibilityStatusAsync();

            Assert.AreEqual(EligibilityStatus.ReviewRequired, result);
        }

        [Test]
        public async Task CreateOrUpdateEnterpriseDetailsAsyncWithMissingEnterprise()
        {
            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult((enterprise)null));

            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto { EnterpriseId = 1 }
            };

            var result = await _sut.CreateOrUpdateEnterpriseDetailsAsync(userVoucherDto);

            Assert.That(result.IsFailed);
        }

        [Test]
        public async Task CreateOrUpdateEnterpriseDetailsAsyncWithEnterpriseId()
        {
            var enterprise = new enterprise();

            _mockEnterpriseRepo
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockEnterpriseSizeRepo
                .Setup(_ => _.GetEnterpriseSizeRecord(It.IsAny<string>()))
                .Returns(Task.FromResult(new enterprise_size { enterprise_size_id = 8 }));

            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = new ApplicantDto { EnterpriseId = 1 },
                HasCompanyHouseNumber = "yes",
                CompanyHouseResponse = new CompanyHouseResponse { CompanyName = "fake company name" }
            };

            var result = await _sut.CreateOrUpdateEnterpriseDetailsAsync(userVoucherDto);

            Assert.That(result.IsSuccess);
        }
    }
}