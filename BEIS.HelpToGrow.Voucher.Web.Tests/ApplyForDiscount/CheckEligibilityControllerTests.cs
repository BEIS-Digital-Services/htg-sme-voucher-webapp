using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class CheckEligibilityControllerTests : BaseControllerTest
    {
        private CheckEligibilityController _sut;

        private Mock<IIndesserHttpConnection<IndesserCompanyResponse>> _mockIndesserHttpConnection;
        private Mock<IIndesserResponseService> _mockIndesserResponseService;
        private Mock<ICheckEligibility> _mockEligibility;
        private Mock<IEligibilityCheckResultService> _mockEligibilityCheckResultService;
        private Mock<IEnterpriseService> _mockEnterpriseService;
        private Mock<ILogger<CheckEligibilityController>> _mockLogger;
        private Mock<ISessionService> _mockSessionService;
        private Mock<IApplicationStatusService> _mockApplicationStatusService;
        private ApplicationStatus _applicationStatus = ApplicationStatus.EmailVerified;

        [SetUp]
        public void Setup()
        {
            _mockIndesserHttpConnection = new Mock<IIndesserHttpConnection<IndesserCompanyResponse>>();
            _mockIndesserResponseService = new Mock<IIndesserResponseService>();
            _mockEligibility = new Mock<ICheckEligibility>();
            _mockEligibilityCheckResultService = new Mock<IEligibilityCheckResultService>();
            _mockEnterpriseService = new Mock<IEnterpriseService>();
            _mockLogger = new Mock<ILogger<CheckEligibilityController>>();
            _mockSessionService = new Mock<ISessionService>();
            _mockApplicationStatusService = new Mock<IApplicationStatusService>();
            _mockApplicationStatusService.Setup(x => x.GetApplicationStatus(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((string s1, string s2) => _applicationStatus);
            _sut = new CheckEligibilityController(
                _mockIndesserHttpConnection.Object,
                _mockIndesserResponseService.Object,
                _mockEligibility.Object,
                _mockEligibilityCheckResultService.Object,
                _mockEnterpriseService.Object,
                _mockLogger.Object,
                _mockSessionService.Object, _mockApplicationStatusService.Object);

        }

        [Test]
        public void SessionError()
        {
            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Throws(new Exception("fake error message"));

            Assert.ThrowsAsync<Exception>(() => _sut.Index());
        }

        [Test]
        public void MissingRedemptionUrl()
        {
            var missingSelectedProduct = new UserVoucherDto
            {
                SelectedProduct = null
            };
            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(missingSelectedProduct);

            Assert.ThrowsAsync<NullReferenceException>(() => _sut.Index());
        }

        [Test]
        public void MissingUserVerification()
        {
            var voucherDto = new UserVoucherDto
            {
                SelectedProduct = new product{ redemption_url = "https://fake.url.org/" },
                ApplicantDto = null
            };
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(voucherDto);

            Assert.ThrowsAsync<NullReferenceException>(() => _sut.Index());
        }

        [Test]
        public async Task ExistingEligibilityPassed()
        {
          
            var voucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url.org/" },
                ApplicantDto = new ApplicantDto { IsVerified = true }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(voucherDto);

            _mockEnterpriseService
                .Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(voucherDto));

            _mockEnterpriseService
                .Setup(_ => _.GetEligibilityStatusAsync())
                .Returns(Task.FromResult(EligibilityStatus.Eligible));

            var result = await _sut.Index();

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("Index", actionResult.ActionName);
            Assert.AreEqual("TokenIssued", actionResult.ControllerName);
        }

        [Test]
        public async Task ExistingEligibilityFailed()
        {
            var voucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url.org/" },
                ApplicantDto = new ApplicantDto { IsVerified = true }
            };
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(voucherDto);

            _mockEnterpriseService
                .Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(voucherDto));

            _mockEnterpriseService
                .Setup(_ => _.GetEligibilityStatusAsync())
                .Returns(Task.FromResult(EligibilityStatus.Failed));

            var result = await _sut.Index();

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("Index", actionResult.ActionName);
            Assert.AreEqual("TokenNotIssued", actionResult.ControllerName);
        }

        [Test]
        public async Task VoucherCancelledCannotReApply()
        {
            _applicationStatus = ApplicationStatus.CancelledCannotReApply;
            var voucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url.org/" },
                ApplicantDto = new ApplicantDto { IsVerified = true }
            };
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(voucherDto);

            _mockEnterpriseService
                .Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(voucherDto));

            _mockEnterpriseService
                .Setup(_ => _.GetEligibilityStatusAsync())
                .Returns(Task.FromResult(EligibilityStatus.Eligible));

            var result = await _sut.Index();

            var actionResult = (RedirectToActionResult)result;

            Assert.AreEqual("Index", actionResult.ActionName);
            Assert.AreEqual("TokenNotIssued", actionResult.ControllerName);
        }

        [Test]
        public async Task FcaApplication()
        {
            var missingSelectedProduct = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url.org/" },
                FCANumber = "fake fca number",
                ApplicantDto =
                {
                    IsVerified = true,
                    EnterpriseId = 1
                }
            };
            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(missingSelectedProduct);
            _mockEnterpriseService.Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(missingSelectedProduct);

            var result = await _sut.Index();

            Assert.AreEqual("TokenIssued", ((RedirectToActionResult)result).ControllerName);
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task IndesserFailure()
        {
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url.org/" },
                CompanyHouseResponse = new CompanyHouseResponse { CompanyNumber = "fake companies house number" },
                ApplicantDto =
                {
                    IsVerified = true,
                    EnterpriseId = 1
                }
            };
            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);
            _mockEnterpriseService.Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(userVoucherDto);

            var failed = new Result<IndesserCompanyResponse>();
            failed.WithError("fake failure message");

            _mockIndesserHttpConnection
                .Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(failed);

            var result = await _sut.Index();

            Assert.AreEqual("IndesserUnavailable", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task IndesserResponsePersistenceFailure()
        {
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url.org/" },
                CompanyHouseResponse = new CompanyHouseResponse
                {
                    CompanyNumber = "fake companies house number"
                },
                ApplicantDto =
                {
                    IsVerified = true
                }
            };
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _mockEnterpriseService
                .Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(userVoucherDto));

            var success = new Result<IndesserCompanyResponse>();

            _mockIndesserHttpConnection
                .Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(success);

            Result<long> fakeId = Result.Fail("fake error message");

            _mockIndesserResponseService
                .Setup(_ => _.SaveAsync(It.IsAny<IndesserCompanyResponse>(), It.IsAny<long>()))
                .Returns(Task.FromResult(fakeId));

            var result = await _sut.Index();

            Assert.AreEqual("Error", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task ErrorsDuringEligibilityCheck()
        {
            
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url.org/" },
                CompanyHouseResponse = new CompanyHouseResponse { CompanyNumber = "fake companies house number" },
                ApplicantDto = { IsVerified = true }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            _mockEnterpriseService
                .Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>()))
                .Returns(Task.FromResult(userVoucherDto));

            var success = new Result<IndesserCompanyResponse>();

            _mockIndesserHttpConnection
                .Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(success);

            var fakeId = Result.Ok(1L);
            _mockIndesserResponseService
                .Setup(_ => _.SaveAsync(It.IsAny<IndesserCompanyResponse>(), It.IsAny<long>()))
                .Returns(Task.FromResult(fakeId));

            _mockEligibility
                .Setup(_ => _.Check(It.IsAny<UserVoucherDto>(), It.IsAny<IndesserCompanyResponse>()))
                .Returns(Result.Fail("fake error message"));

            var result = await _sut.Index();

            var actionResult = (RedirectToActionResult) result;

            Assert.AreEqual("Error", actionResult.ActionName);
            Assert.AreEqual("Home", actionResult.ControllerName);
        }

        [Test]
        public async Task FailsEligibilityCheck()
        {
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url.org/" },
                CompanyHouseResponse = new CompanyHouseResponse { CompanyNumber = "fake companies house number" },
                ApplicantDto =
                {
                    IsVerified = true,
                    EnterpriseId = 1
                }
            };
            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);
            _mockEnterpriseService.Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(userVoucherDto);

            var success = new Result<IndesserCompanyResponse>();

            _mockIndesserHttpConnection
                .Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(success);

            var fakeId = Result.Ok(1L);
            _mockIndesserResponseService
                .Setup(_ => _.SaveAsync(It.IsAny<IndesserCompanyResponse>(), It.IsAny<long>()))
                .Returns(Task.FromResult(fakeId));

            var errors = new List<IError>();
            var reviewItems = new List<IError>();
            var recordedItems = new List<IError>();
            var check = new Check(false, errors, reviewItems, recordedItems);

            _mockEligibility
                .Setup(_ => _.Check(It.IsAny<UserVoucherDto>(), It.IsAny<IndesserCompanyResponse>()))
                .Returns(Result.Ok(check));

            var result = await _sut.Index();

            Assert.AreEqual("TokenNotIssued", ((RedirectToActionResult)result).ControllerName);
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public async Task PassesEligibilityCheck()
        {
            var userVoucherDto = new UserVoucherDto
            {
                SelectedProduct = new product { redemption_url = "https://fake.url.org/" },
                CompanyHouseResponse = new CompanyHouseResponse { CompanyNumber = "fake companies house number" },
                ApplicantDto =
                {
                    IsVerified = true,
                    EnterpriseId = 1
                }
            };
            _mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(userVoucherDto);
            _mockEnterpriseService.Setup(_ => _.GetUserVoucherFromEnterpriseAsync(It.IsAny<long>(), It.IsAny<long>())).ReturnsAsync(userVoucherDto);

            var success = new Result<IndesserCompanyResponse>();

            _mockIndesserHttpConnection
                .Setup(_ => _.ProcessRequest(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(success);

            var fakeId = Result.Ok(1L);
            _mockIndesserResponseService
                .Setup(_ => _.SaveAsync(It.IsAny<IndesserCompanyResponse>(), It.IsAny<long>()))
                .Returns(Task.FromResult(fakeId));

            var errors = new List<IError>();
            var reviewItems = new List<IError>();
            var recordedItems = new List<IError>();
            var check = new Check(true, errors, reviewItems, recordedItems);

            _mockEligibility
                .Setup(_ => _.Check(It.IsAny<UserVoucherDto>(), It.IsAny<IndesserCompanyResponse>()))
                .Returns(Result.Ok(check));

            var result = await _sut.Index();

            Assert.AreEqual("TokenIssued", ((RedirectToActionResult)result).ControllerName);
            Assert.AreEqual("Index", ((RedirectToActionResult)result).ActionName);
        }

        [Test]
        public void IndesserUnavailable()
        {
            var viewResult = (ViewResult)_sut.IndesserUnavailable();

            Assert.Null(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }
    }
}