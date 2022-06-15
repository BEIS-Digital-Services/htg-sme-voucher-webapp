
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]

    public class SessionExpiredControllerTest
    {
        private SessionExpiredController _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new SessionExpiredController(Options.Create(new UrlOptions { LearningPlatformUrl = "https://test-webapp.azurewebsites.net/" }));
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult) _sut.Index();
            var viewModel = viewResult.Model as SessionExpiredViewModel;

            Assert.NotNull(viewModel);
            Assert.AreEqual(new Uri("https://test-webapp.azurewebsites.net/comparison-tool"), $"{viewModel.LearningPlatformUrl}comparison-tool");
        }
    }
}