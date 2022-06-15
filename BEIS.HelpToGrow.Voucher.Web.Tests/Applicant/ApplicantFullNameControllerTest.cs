using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Web.Controllers;
using Beis.HelpToGrow.Voucher.Web.Models.Applicant;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Beis.HelpToGrow.Voucher.Web.Models.Voucher;
using Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount;
using Beis.HelpToGrow.Voucher.Web.Services;

namespace Beis.HelpToGrow.Voucher.Web.Tests.Applicant
{
    [TestFixture]
    public class ApplicantFullNameControllerTest : BaseControllerTest
    {
        private ApplicantFullNameController _sut;
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
            _sut = new ApplicantFullNameController(_mockSessionService.Object);
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
        public void GetIndexHandlesMissingName()
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
                .Returns(new UserVoucherDto { ApplicantDto = new ApplicantDto { FullName = "fake full name" }});

            var viewResult = (ViewResult)_sut.Index();

            Assert.That(viewResult.Model is FullNameViewModel);
        }

        [Test]
        public void PostIndexHandlesSessionException()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Throws(new Exception("fake error message"));


            Assert.Throws<Exception>(() => _sut.Index(new FullNameViewModel { Name = "fake full name" }));
        }

        [Test]
        public void PostIndexHandlesInvalidName()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var viewResult = (ViewResult)_sut.Index(new FullNameViewModel { Name = default });

            Assert.AreEqual("Index", viewResult.ViewName);
        }

        [Test]
        [TestCase("Paul Cripps")]
        public void GetIndexSetsUserSessionDtoForAValidModel(string name)
        {
            _sut.Index(new FullNameViewModel { Name = name });
            var dtoResult = _mockSessionService.Object.Get<UserVoucherDto>("userVoucherDto", _controllerContext.HttpContext);

            Assert.IsNotNull(dtoResult);
            Assert.IsNotNull(dtoResult.ApplicantDto);
            Assert.IsNotNull(dtoResult.ApplicantDto.FullName);
        }

        [Test]
        [TestCase("Paul Cripps")]
        [TestCase("Renée")]
        [TestCase("Noël Cole")]
        [TestCase("Sørina")]
        [TestCase("Adrián")]
        [TestCase("Zoë Smith")]
        [TestCase("François")]
        [TestCase("Mary-Jo")]
        [TestCase("Mónica")]
        [TestCase("Seán Jones")]
        [TestCase("Mathéo")]
        [TestCase("Ruairí")]
        [TestCase("Mátyás")]
        [TestCase("Jokūbas")]
        [TestCase("John-Paul")]
        [TestCase("Siân Smith")]
        [TestCase("Agnès Jones")]
        [TestCase("Maël McDonald")]
        [TestCase("János")]
        [TestCase("KŠthe")]
        [TestCase("Chloë")]
        [TestCase("Øyvind")]
        [TestCase("Asbjørn")]
        [TestCase("Fañch")]
        [TestCase("José Sanchez")]
        [TestCase("Nuñez")]
        public void ValidationPasses(string name)
        {
            // Arrange
            var sut = new FullNameViewModel { Name = name };
            
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
        [TestCase("        ", Description = "White space larger than min length")]
        public void ValidationFails(string name)
        {
            // Arrange
            var sut = new FullNameViewModel { Name = name };
            
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