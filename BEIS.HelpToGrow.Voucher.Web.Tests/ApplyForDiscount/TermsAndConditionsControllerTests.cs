
namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class TermsAndConditionsControllerTests
    {
        private TermsAndConditionsController _sut;
        private Mock<ISessionService> _mockSessionService;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _sut = new TermsAndConditionsController(_mockSessionService.Object, Options.Create(new UrlOptions { LearningPlatformUrl = "https://fake.url.com/" }));
        }

        [Test]
        public void HandlesMissingSession()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns((UserVoucherDto) null);

            var viewResult = (ViewResult) _sut.Index();

            Assert.That(viewResult.Model is TermsConditionsViewModel);
            Assert.Null(((TermsConditionsViewModel) viewResult.Model).SelectedProduct);
        }

        [Test]
        public void HandlesMissingTermsAndConditions()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ConsentTermsConditions = null,
                SelectedProduct = new product
                {
                    product_name = "fake product name"
                }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewResult = (ViewResult) _sut.Index();

            Assert.That(viewResult.Model is TermsConditionsViewModel);
            Assert.AreEqual("fake product name", ((TermsConditionsViewModel) viewResult.Model).SelectedProduct);
            Assert.IsFalse(((TermsConditionsViewModel) viewResult.Model).TermsAndConditionsAccepted);
            Assert.IsFalse(((TermsConditionsViewModel) viewResult.Model).PrivacyPolicyAccepted);
            Assert.IsFalse(((TermsConditionsViewModel) viewResult.Model).SubsidyControlAccepted);
        }

        [Test]
        public void GetIndex()
        {
            var userVoucherDto = new UserVoucherDto
            {
                ConsentTermsConditions = "fake positive value",
                SelectedProduct = new product
                {
                    product_name = "fake product name"
                }
            };

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewResult = (ViewResult) _sut.Index();

            Assert.That(viewResult.Model is TermsConditionsViewModel);
            Assert.AreEqual("fake product name", ((TermsConditionsViewModel) viewResult.Model).SelectedProduct);
            Assert.IsTrue(((TermsConditionsViewModel) viewResult.Model).TermsAndConditionsAccepted);
            Assert.IsTrue(((TermsConditionsViewModel) viewResult.Model).PrivacyPolicyAccepted);
            Assert.IsTrue(((TermsConditionsViewModel) viewResult.Model).SubsidyControlAccepted);
        }

        [Test]
        public void PostIndexInvalidModel()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var viewModel = new TermsConditionsViewModel();

            var viewResult = (ViewResult)_sut.Index(viewModel);

            Assert.That(viewResult.Model is TermsConditionsViewModel);
            Assert.Null(viewResult.ViewName);
        }

        [Test]
        public void PostIndex()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var viewModel = new TermsConditionsViewModel
            {
                PrivacyPolicyAccepted = true,
                SubsidyControlAccepted = true,
                TermsAndConditionsAccepted = true,
            };

            var actionResult = (RedirectToActionResult)_sut.Index(viewModel);

            Assert.AreEqual("ConfirmApplicant", actionResult.ControllerName);
            Assert.AreEqual("Index", actionResult.ActionName);
        }

        [Test]
        public void Terms()
        {
            var viewResult = (ViewResult)_sut.Terms();
            var viewModel = viewResult.Model as TermsConditionsViewModel;

            Assert.IsEmpty(viewResult.ViewData);
            Assert.NotNull(viewModel);
            Assert.AreEqual("https://fake.url.com/eligibility",  $"{viewModel.LearningPlatformUrl}eligibility");
        }

        [Test]
        public void Privacy()
        {
            var viewResult = (ViewResult)_sut.Privacy();

            Assert.AreEqual(0, viewResult.ViewData.Count);
        }

        [Test]
        public void Subsidy()
        {
            var viewResult = (ViewResult)_sut.Subsidy();

            Assert.AreEqual(0, viewResult.ViewData.Count);
        }
    }
}