using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Web.Controllers;

namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class AccessibilityControllerTest : BaseControllerTest
    {
        private AccessibilityController _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new AccessibilityController();
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult)_sut.Index();

            Assert.AreEqual(0, viewResult.ViewData.Count);
        }
    }
}