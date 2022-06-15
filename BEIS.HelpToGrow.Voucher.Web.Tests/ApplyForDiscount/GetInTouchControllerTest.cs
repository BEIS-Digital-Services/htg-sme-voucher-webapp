using Beis.HelpToGrow.Voucher.Web.Config;
using Beis.HelpToGrow.Voucher.Web.Controllers;
using Beis.HelpToGrow.Voucher.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class GetInTouchControllerTest : BaseControllerTest
    {
        private GetInTouchController _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new GetInTouchController(Options.Create(new UrlOptions { LearningPlatformUrl = "https://test-webapp.azurewebsites.net/" }));
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult) _sut.Index();
            var viewModel = viewResult.Model as UsefulLinksViewModel;

            Assert.NotNull(viewModel);
            Assert.AreEqual("https://test-webapp.azurewebsites.net/", viewModel.LearningPlatformUrl);
        }
    }
}