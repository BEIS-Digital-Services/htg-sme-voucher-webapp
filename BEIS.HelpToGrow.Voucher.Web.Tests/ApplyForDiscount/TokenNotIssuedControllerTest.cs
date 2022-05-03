using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class TokenNotIssuedControllerTest : BaseControllerTest
    {
        private TokenNotIssuedController _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new TokenNotIssuedController();
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult)_sut.Index();

            Assert.AreEqual(0, viewResult.ViewData.Count);
        }
    }
}