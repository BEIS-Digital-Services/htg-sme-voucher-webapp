
namespace Beis.HelpToGrow.Voucher.Web.Tests.Common
{
    [TestFixture]
    public class SharedResultFilterTest
    {
        private SharedResultFilter _sut;
        private Mock<ISessionService> _mockSessionService;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _sut = new SharedResultFilter(_mockSessionService.Object, Options.Create(new UrlOptions { LearningPlatformUrl = "https://localhost" }));
        }

        [Test]
        public void OnResultExecutingNotController()
        {
            HttpContext http = new DefaultHttpContext();
            var contactUsController = new ContactUsController();
            var routeData = new RouteData();
            routeData.Values.Add("Action", "FakeAction");
            routeData.Values.Add("Controller", "FakeController");
            contactUsController.ControllerContext = new ControllerContext {RouteData = routeData};
            var executing = CreateResultExecutingContext(http, contactUsController);

            _sut.OnResultExecuting(executing);

            Assert.NotNull(contactUsController.ViewData["CookieBannerViewModel"]);
        }

        private static ActionContext CreateActionContext(HttpContext context) => new(context, new(), new());

        private static ResultExecutingContext CreateResultExecutingContext(HttpContext context, Controller controller) =>
            new(CreateActionContext(context), Array.Empty<IFilterMetadata>(), new NoOpResult(), controller);
    }
}