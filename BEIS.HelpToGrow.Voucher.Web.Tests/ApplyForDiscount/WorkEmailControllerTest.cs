﻿
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class WorkEmailControllerTest : BaseControllerTest
    {
        private WorkEmailController _sut;

        [SetUp]
        public void Setup()
        {
            var controllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
            var mockSessionService = new Mock<ISessionService>();
            mockSessionService.Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), controllerContext.HttpContext));

            _sut = new WorkEmailController(mockSessionService.Object);
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult)_sut.Index();

            Assert.AreEqual(0, viewResult.ViewData.Count);
        }
    }
}