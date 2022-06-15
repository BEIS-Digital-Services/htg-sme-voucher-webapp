
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