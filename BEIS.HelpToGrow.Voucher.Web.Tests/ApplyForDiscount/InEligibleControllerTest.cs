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
    public class InEligibleControllerTest : BaseControllerTest
    {
        private InEligibleController _sut;

        [SetUp]
        public void Setup()
        {
            var mockSessionService = new Mock<ISessionService>();

            mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(new UserVoucherDto());

            _sut = new InEligibleController(mockSessionService.Object);
        }

        [Test]
        public void MajorUpgrade()
        {
            var viewResult = (ViewResult)_sut.MajorUpgrade();
            Assert.AreEqual(0, viewResult.ViewData.Count);
        }

        [Test]
        public void NotFirstTime()
        {
            var viewResult = (ViewResult)_sut.NotFirstTime();
            Assert.AreEqual(0, viewResult.ViewData.Count);
        }

        [Test]
        public void Deregistered()
        {
            var viewResult = (ViewResult)_sut.Deregistered();
            Assert.AreEqual(0, viewResult.ViewData.Count);
        }

        [Test]
        public void CompanySize()
        {
            var viewResult = (ViewResult)_sut.CompanySize();
            Assert.AreEqual(0, viewResult.ViewData.Count);
        }

        [Test]
        public void FCA()
        {
            var viewResult = (ViewResult)_sut.FCA();
            Assert.AreEqual(0, viewResult.ViewData.Count);
        }

        [Test]
        public void Vendor()
        {
            var viewResult = (ViewResult)_sut.Vendor();
            Assert.AreEqual(0, viewResult.ViewData.Count);
        }
    }
}