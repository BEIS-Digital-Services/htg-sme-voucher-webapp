using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Web.Controllers;
using Beis.HelpToGrow.Voucher.Web.Models.Voucher;
using Beis.HelpToGrow.Voucher.Web.Services;

namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class ConfirmWorkEmailControllerTest : BaseControllerTest
    {
        private ConfirmWorkEmailController _sut;

        [SetUp]
        public void Setup()
        {
            var mockSessionService = new Mock<ISessionService>();

            mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>())).Returns(new UserVoucherDto());

            _sut = new ConfirmWorkEmailController(mockSessionService.Object);
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult)_sut.Index("fakeWorkEmail");

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