using System;
using System.Threading.Tasks;
using BEIS.HelpToGrow.Core.Enums;
using Beis.HelpToGrow.Core.Repositories.Interface;
using BEIS.HelpToGrow.Voucher.Web.Common;
using Beis.Htg.VendorSme.Database.Models;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Interfaces;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class TokenIssuedControllerTests : BaseControllerTest
    {
        private TokenIssuedController _sut;
        private Mock<ILogger<TokenIssuedController>> _mockLogger;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IVoucherGenerationService> _mockVoucherGeneratorService;
        private Mock<IVendorCompanyRepository> _mockVendorCompanyRepository;
        private Mock<IEnterpriseRepository> _mockEnterpriseRepository;
        private Mock<INotifyService> _mockNotifyService;
        private Mock<IApplicationStatusService> _mockApplicationStatusService;
        private ApplicationStatus _applicationStatus = ApplicationStatus.EmailVerified;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<TokenIssuedController>>();
            _mockSessionService = new Mock<ISessionService>();
            _mockVoucherGeneratorService = new Mock<IVoucherGenerationService>();
            _mockVendorCompanyRepository = new Mock<IVendorCompanyRepository>();
            _mockEnterpriseRepository = new Mock<IEnterpriseRepository>();
            _mockNotifyService = new Mock<INotifyService>();

            _mockApplicationStatusService = new Mock<IApplicationStatusService>();
            _mockApplicationStatusService.Setup(x => x.GetApplicationStatus(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string s1, string s2) => _applicationStatus);
            _sut = new TokenIssuedController(
                _mockLogger.Object,
                _mockSessionService.Object,
                _mockVoucherGeneratorService.Object,
                _mockVendorCompanyRepository.Object,
                _mockEnterpriseRepository.Object,
                _mockNotifyService.Object,
                _mockApplicationStatusService.Object);
        }

        [Test]
        public void HandlesSessionError()
        {
            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Throws(new Exception("fake error message"));

            Assert.ThrowsAsync<Exception>(() => _sut.Index());
        }

        [Test]
        public void HandlesMissingSelectedProduct()
        {
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = null
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var vendorCompany = new vendor_company();

            _mockVendorCompanyRepository
                .Setup(_ => _.GetVendorCompanySingle(It.IsAny<long>()))
                .Returns(Task.FromResult(vendorCompany));

            Assert.ThrowsAsync<NullReferenceException>(() => _sut.Index());
        }

        [Test]
        public void HandlesMissingEnterpriseId()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ApplicantDto = null,
                SelectedProduct = new product { redemption_url = "fake url" }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var vendorCompany = new vendor_company();

            _mockVendorCompanyRepository
                .Setup(_ => _.GetVendorCompanySingle(It.IsAny<long>()))
                .Returns(Task.FromResult(vendorCompany));

            Assert.ThrowsAsync<NullReferenceException>(() => _sut.Index());
        }

        [Test]
        public async Task IndexUnverifiedEmail()
        {
            _applicationStatus = ApplicationStatus.EmailNotVerified;
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url/" }
            };
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var vendorCompany = new vendor_company();
            _mockVendorCompanyRepository
                .Setup(_ => _.GetVendorCompanySingle(It.IsAny<long>()))
                .Returns(Task.FromResult(vendorCompany));

            var enterprise = new enterprise { applicant_email_verified = false, eligibility_status_id = 2 };
            _mockEnterpriseRepository
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockVoucherGeneratorService
                .Setup(_ => _.GenerateVoucher(vendorCompany, enterprise, userVoucherDto.SelectedProduct))
                .Returns(Task.FromResult("fake voucher code"));

            var notificationResult = Result.Ok();
            _mockNotifyService
                .Setup(_ => _.SendVoucherToApplicant(userVoucherDto))
                .Returns(Task.FromResult(notificationResult));

            var result = await _sut.Index();

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("CheckEmailAddress", actionResult.ActionName);
            Assert.AreEqual("ApplicantEmailAddress", actionResult.ControllerName);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("There was an error issuing the token. The enterprise has not been verified.") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task IndexUnknownEligibility()
        {
            _applicationStatus = ApplicationStatus.EmailNotVerified;
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url/" }
            };
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var vendorCompany = new vendor_company();
            _mockVendorCompanyRepository
                .Setup(_ => _.GetVendorCompanySingle(It.IsAny<long>()))
                .Returns(Task.FromResult(vendorCompany));

            var enterprise = new enterprise { applicant_email_verified = false, eligibility_status_id = (long)EligibilityStatus.Unknown };
            _mockEnterpriseRepository
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockVoucherGeneratorService
                .Setup(_ => _.GenerateVoucher(vendorCompany, enterprise, userVoucherDto.SelectedProduct))
                .Returns(Task.FromResult("fake voucher code"));

            var notificationResult = Result.Ok();
            _mockNotifyService
                .Setup(_ => _.SendVoucherToApplicant(userVoucherDto))
                .Returns(Task.FromResult(notificationResult));

            var result = await _sut.Index();

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("CheckEmailAddress", actionResult.ActionName);
            Assert.AreEqual("ApplicantEmailAddress", actionResult.ControllerName);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("There was an error issuing the token. The enterprise has not been verified.") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task TokenIssued()
        {
            _applicationStatus = ApplicationStatus.Eligible;
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url/" },
                ApplicantDto = new ApplicantDto
                {
                    EmailAddress = "test@test.com"
                }
            };
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var vendorCompany = new vendor_company();
            _mockVendorCompanyRepository
                .Setup(_ => _.GetVendorCompanySingle(It.IsAny<long>()))
                .Returns(Task.FromResult(vendorCompany));

            var enterprise = new enterprise 
            {
                applicant_email_verified = true, 
                eligibility_status_id = 2,
                applicant_email_address = "test@test.com"
            };
            _mockEnterpriseRepository
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockVoucherGeneratorService
                .Setup(_ => _.GenerateVoucher(vendorCompany, enterprise, userVoucherDto.SelectedProduct))
                .Returns(Task.FromResult("fake voucher code"));

            var notificationResult = Result.Ok();
            _mockNotifyService
                .Setup(_ => _.SendVoucherToApplicant(userVoucherDto))
                .Returns(Task.FromResult(notificationResult));

            var result = await _sut.Index();

            var viewResult = (ViewResult)result;

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Getting token for enterprise") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            Assert.That(viewResult.Model is UserVoucherDto);
        }

        [Test]
        public async Task VoucherCancelledCannotReapply()
        {
            _applicationStatus = ApplicationStatus.CancelledCannotReApply;
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url/" },
                ApplicantDto = new ApplicantDto
                {
                    EmailAddress = "test@test.com"
                }
            };
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var vendorCompany = new vendor_company();
            _mockVendorCompanyRepository
                .Setup(_ => _.GetVendorCompanySingle(It.IsAny<long>()))
                .Returns(Task.FromResult(vendorCompany));

            var enterprise = new enterprise
            {
                applicant_email_verified = true,
                eligibility_status_id = 2,
                applicant_email_address = "test@test.com"
            };
            _mockEnterpriseRepository
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockVoucherGeneratorService
                .Setup(_ => _.GenerateVoucher(vendorCompany, enterprise, userVoucherDto.SelectedProduct))
                .Returns(Task.FromResult("fake voucher code"));

            var notificationResult = Result.Ok();
            _mockNotifyService
                .Setup(_ => _.SendVoucherToApplicant(userVoucherDto))
                .Returns(Task.FromResult(notificationResult));

            var result = await _sut.Index();

            var viewResult = (ViewResult)result;

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Getting token for enterprise") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            Assert.That(viewResult.ViewName == "CheckEligibility");
        }


        [Test]
        public void HandlesBadUrl()
        {
            _applicationStatus = ApplicationStatus.Eligible;
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "bad bad url" },
                ApplicantDto = new ApplicantDto
                {
                    EmailAddress = "test@test.com",
                    IsVerified = true
                }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            
            var vendorCompany = new vendor_company();
            _mockVendorCompanyRepository
                .Setup(_ => _.GetVendorCompanySingle(It.IsAny<long>()))
                .Returns(Task.FromResult(vendorCompany));

            var enterprise = new enterprise 
            { 
                applicant_email_verified = true, 
                eligibility_status_id = 2,
                applicant_email_address = "test@test.com"
            };
            _mockEnterpriseRepository
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockVoucherGeneratorService
                .Setup(_ => _.GenerateVoucher(vendorCompany, enterprise, userVoucherDto.SelectedProduct))
                .Returns(Task.FromResult("fake voucher code"));

            var notificationResult = Result.Ok();
            _mockNotifyService
                .Setup(_ => _.SendVoucherToApplicant(userVoucherDto))
                .Returns(Task.FromResult(notificationResult));

            Assert.ThrowsAsync<UriFormatException>(() => _sut.Index());
        }

        [Test]
        public async Task NotificationFailed()
        {
            _applicationStatus = ApplicationStatus.Eligible;
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url/" },
            };
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var vendorCompany = new vendor_company();
            _mockVendorCompanyRepository
                .Setup(_ => _.GetVendorCompanySingle(It.IsAny<long>()))
                .Returns(Task.FromResult(vendorCompany));

            var enterprise = new enterprise { applicant_email_verified = true, eligibility_status_id = 2 };
            _mockEnterpriseRepository
                .Setup(_ => _.GetEnterprise(It.IsAny<long>()))
                .Returns(Task.FromResult(enterprise));

            _mockVoucherGeneratorService
                .Setup(_ => _.GenerateVoucher(vendorCompany, enterprise, userVoucherDto.SelectedProduct))
                .Returns(Task.FromResult("fake voucher code"));

            var notificationResult = Result.Fail("fake error message");
            _mockNotifyService
                .Setup(_ => _.SendVoucherToApplicant(userVoucherDto))
                .Returns(Task.FromResult(notificationResult));

            var result = await _sut.Index();

            var actionResult = (RedirectToActionResult)result;

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Information),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Getting token for enterprise") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            Assert.AreEqual("Error", actionResult.ActionName);
        }

        [Test]
        public void Error()
        {
            var viewResult = (ViewResult)_sut.Error();

            Assert.That(viewResult.Model is ErrorViewModel);
        }
    }
}