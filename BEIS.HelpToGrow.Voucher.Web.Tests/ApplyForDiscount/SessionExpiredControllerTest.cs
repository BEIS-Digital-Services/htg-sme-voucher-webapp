using System;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]

    public class SessionExpiredControllerTest
    {
        private SessionExpiredController _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SessionExpiredController();

            Environment.SetEnvironmentVariable("LEARNING_PLATFORM_URL", "https://fake.url.com/");
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult) _sut.Index();
            var viewModel = viewResult.Model as SessionExpiredViewModel;

            Assert.NotNull(viewModel);
            Assert.AreEqual(new Uri("https://fake.url.com/comparison-tool"), viewModel.ComparisonToolURL);
        }
    }
}