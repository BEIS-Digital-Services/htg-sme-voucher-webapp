using System;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class GetInTouchControllerTest : BaseControllerTest
    {
        private GetInTouchController _sut;
        private static string LearningPlatformUrl => "https://test-webapp.azurewebsites.net/";

        [SetUp]
        public void Setup()
        {
            _sut = new GetInTouchController();

            Environment.SetEnvironmentVariable("LEARNING_PLATFORM_URL", LearningPlatformUrl);
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult) _sut.Index();
            var viewModel = viewResult.Model as UsefulLinksViewModel;

            Assert.NotNull(viewModel);
            Assert.AreEqual(LearningPlatformUrl, viewModel.LearningPlatformURL.ToString());
        }
    }
}