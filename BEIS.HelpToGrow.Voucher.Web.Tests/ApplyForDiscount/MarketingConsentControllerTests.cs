using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    internal class MarketingConsentControllerTests
    {
        private MarketingConsentController _sut;
        private Mock<ISessionService> _mockSessionService;
        private Mock<ILogger<MarketingConsentController>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockSessionService = new Mock<ISessionService>();
            _mockLogger = new Mock<ILogger<MarketingConsentController>>();
            _sut = new MarketingConsentController(_mockSessionService.Object, _mockLogger.Object);
        }

        [Test]
        public async Task HandlesMissingSession()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns((UserVoucherDto)null);

            var viewResult = (await _sut.Index() as RedirectToActionResult);

            Assert.AreEqual(string.Empty, viewResult.ActionName);
            Assert.AreEqual("SessionExpired", viewResult.ControllerName);
        }

        [Test]
        public async Task HandlesMissingViewData()
        {
            var userVoucherDto = new UserVoucherDto();
           

            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewResult = (await _sut.Index() as ViewResult);

            Assert.That(viewResult.Model is MarketingConsentViewModel);
       
            Assert.IsFalse(((MarketingConsentViewModel)viewResult.Model).AcceptMarketingByEmail);
            Assert.IsFalse(((MarketingConsentViewModel)viewResult.Model).AcceptMarketingByPhone);
         
        }

        [Test]
        public async Task GetIndex()
        {
            var userVoucherDto = new UserVoucherDto 
            {
                ApplicantDto = new ApplicantDto
                {
                    HasProvidedMarketingConsentByPhone = true,
                    HasProvidedMarketingConsent = true
                }
            };


            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(userVoucherDto);

            var viewResult = (await _sut.Index() as ViewResult) ;

            Assert.That(viewResult.Model is MarketingConsentViewModel);
          
            Assert.IsTrue(((MarketingConsentViewModel)viewResult.Model).AcceptMarketingByPhone);
            Assert.IsTrue(((MarketingConsentViewModel)viewResult.Model).AcceptMarketingByEmail);

        }

        [Test]
        public async Task PostIndexInvalidModel()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var viewModel = new MarketingConsentViewModel();

            var viewResult = (await _sut.Index(viewModel) as RedirectToActionResult);

      
            Assert.AreEqual("Index", viewResult.ActionName);
            Assert.AreEqual("ConfirmApplicant", viewResult.ControllerName);
        }

        [Test]
        public async Task PostIndex()
        {
            _mockSessionService
                .Setup(_ => _.Get<UserVoucherDto>(It.IsAny<string>(), It.IsAny<HttpContext>()))
                .Returns(new UserVoucherDto());

            var viewModel = new MarketingConsentViewModel
            {
               AcceptMarketingByEmail = true,
               AcceptMarketingByPhone = true,
            };

            var actionResult = (await _sut.Index(viewModel) as RedirectToActionResult);

            Assert.AreEqual("ConfirmApplicant", actionResult.ControllerName);
            Assert.AreEqual("Index", actionResult.ActionName);
        }
    }
}

