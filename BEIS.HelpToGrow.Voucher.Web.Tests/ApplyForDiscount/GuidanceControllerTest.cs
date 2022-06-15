
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class GuidanceControllerTest : BaseControllerTest
    {
        private GuidanceController _sut;
        private static string LearningPlatformUrl => "https://fake-test-webapp.azurewebsites.net/";

        [SetUp]
        public void Setup()
        {
            _sut = new GuidanceController(Options.Create(new UrlOptions { LearningPlatformUrl = LearningPlatformUrl }));
        }

        [Test]
        public void Index()
        {
            var viewResult = (ViewResult)_sut.Index();
            var viewModel = viewResult.Model as GuidanceViewModel;

            Assert.NotNull(viewModel);
            Assert.AreEqual(LearningPlatformUrl , viewModel.LearningPlatformUrl);
        }
    }
}