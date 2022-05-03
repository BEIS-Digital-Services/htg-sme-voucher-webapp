using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models.Applicant;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount;
using BEIS.HelpToGrow.Voucher.Web.Services;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.Applicant
{
    [TestFixture]
    public class ApplicantRoleControllerTest : BaseControllerTest
    {
        private ApplicantRoleController _sut;
        private Mock<ILogger<ApplicantRoleController>> _mockLogger;
        private Mock<ISessionService> _mockSessionService;
        private ControllerContext _controllerContext;
        private readonly MemoryCache _memoryCache = new(new MemoryCacheOptions());
        
        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<ApplicantRoleController>>();
            _mockSessionService = new Mock<ISessionService>();
            _controllerContext = SetupControllerContext(_controllerContext);
            _mockSessionService
                .Setup(x => x.Get<UserVoucherDto>(It.IsAny<string>(), _controllerContext.HttpContext))
                .Returns((string key, HttpContext _) => _memoryCache.Get<UserVoucherDto>(key));
            _mockSessionService
                .Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), _controllerContext.HttpContext))
                .Callback((string s, object o, HttpContext _) => _memoryCache.Set(s, o as UserVoucherDto));
            _sut = new ApplicantRoleController(_mockSessionService.Object, _mockLogger.Object);
            _sut.ControllerContext = _controllerContext;
        }

        [Test]
        public void GetIndex()
        {
            var userVoucherDto = new UserVoucherDto { ApplicantDto = new ApplicantDto { Role = "fake role" }};

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewResult = (ViewResult)_sut.Index();

            Assert.That(viewResult.Model is TitleOrRoleViewModel);
            Assert.AreEqual("fake role", ((TitleOrRoleViewModel)viewResult.Model).BusinessRole);
        }

        [Test]
        public void GetIndexMissingBusinessRole()
        {
            var userVoucherDto = new UserVoucherDto();

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewResult = (ViewResult)_sut.Index();

            Assert.Null(viewResult.Model);
            Assert.Null(viewResult.ViewName);
        }

        [Test]
        public void GetIndexHandlesSessionException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            var actionResult = (RedirectToActionResult)_sut.Index();

            Assert.AreEqual("Error", actionResult.ActionName);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Error serving applicant role page") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void PostIndexHandlesSessionException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));

            var viewModel = new TitleOrRoleViewModel { BusinessRole = "fake role" };

            var actionResult = (RedirectToActionResult)_sut.Index(viewModel);

            Assert.AreEqual("Error", actionResult.ActionName);

            _mockLogger.Verify(logger => logger.Log(
                    It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                    It.Is<EventId>(eventId => eventId.Id == 0),
                    It.Is<It.IsAnyType>((@object, type) => @object.ToString().Contains("Error progressing from applicant role page") && type.Name == "FormattedLogValues"),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void PostIndexMissingBusinessRole()
        {
            var userVoucherDto = new UserVoucherDto();

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewResult = (ViewResult)_sut.Index(new TitleOrRoleViewModel());

            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [Test]
        [TestCase("Company Director")]
        public void GetIndexSetsUserSessionDtoForAValidModel(string name)
        {
            _sut.Index(new TitleOrRoleViewModel { BusinessRole = name });
            var dtoResult = _mockSessionService.Object.Get<UserVoucherDto>("userVoucherDto", _controllerContext.HttpContext);

            Assert.IsNotNull(dtoResult);
            Assert.IsNotNull(dtoResult.ApplicantDto);
            Assert.IsNotNull(dtoResult.ApplicantDto.Role);
        }

        [TestCase("Company Director")]
        [TestCase("Company Secretary")]
        [TestCase("Company Administrator")]
        [TestCase("Company Accountant")]
        [TestCase("Director")]
        [TestCase("Accountant")]
        [TestCase("Boss")]
        [TestCase("Staff")]
        public void ValidationPasses(string role)
        {
            var sut = new TitleOrRoleViewModel { BusinessRole = role };
            var context = new ValidationContext(sut, null, null);
            var results = new List<ValidationResult>();
            var modelIsValid = Validator.TryValidateObject(sut, context, results, true);
            Assert.IsTrue(modelIsValid);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("      ")]
        public void ValidationFails(string role)
        {
            var sut = new TitleOrRoleViewModel { BusinessRole = role };
            var context = new ValidationContext(sut, null, null);
            var results = new List<ValidationResult>();
            var modelIsValid = Validator.TryValidateObject(sut, context, results, true);
            Assert.IsFalse(modelIsValid);
        }
    }
}