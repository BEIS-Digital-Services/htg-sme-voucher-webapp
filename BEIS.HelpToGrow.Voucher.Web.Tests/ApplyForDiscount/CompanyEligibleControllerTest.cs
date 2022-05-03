using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class CompanyEligibleControllerTest : BaseControllerTest
    {
        private CompanyEligibleController _sut;

        [SetUp]
        public void Setup()
        {
            var controllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var mockSessionService = new Mock<ISessionService>();
            mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), controllerContext.HttpContext));

            _sut = new CompanyEligibleController(mockSessionService.Object);
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult)_sut.Index();

            Assert.AreEqual(0, viewResult.ViewData.Count);
        }

        [Test]
        public void Error()
        {
            var viewResult = (ViewResult)_sut.Error();

            Assert.AreEqual(0, viewResult.ViewData.Count);
        }
    }
}