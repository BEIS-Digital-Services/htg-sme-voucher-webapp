using System;
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Models;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class GuidanceControllerTest : BaseControllerTest
    {
        private GuidanceController _sut;
        private static string LearningPlatformUrl => "https://fake-test-webapp.azurewebsites.net/";

        [SetUp]
        public void Setup()
        {
            _sut = new GuidanceController();

            Environment.SetEnvironmentVariable("LEARNING_PLATFORM_URL", LearningPlatformUrl);
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult)_sut.Index();
            var viewModel = viewResult.Model as GuidanceViewModel;

            Assert.NotNull(viewModel);
            Assert.AreEqual(new Uri(LearningPlatformUrl) , viewModel.LearningPlatformURL);
            Assert.AreEqual(new Uri($"{LearningPlatformUrl}comparison-tool"), viewModel.ComparisonToolURL);
        }
    }
}