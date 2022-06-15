
using BEIS.HelpToGrow.Voucher.Web.Controllers;
using BEIS.HelpToGrow.Voucher.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

using System.Threading.Tasks;
 using BEIS.HelpToGrow.Voucher.Web.Config;
 using Microsoft.Extensions.Options;

 namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount
{
    [TestFixture]
    public class CancelVoucherControllerTests
    {
        private Mock<IVoucherCancellationService> _mockVoucherCancellationService;
        private Mock<ILogger<CancelVoucherController>> _mockLogger;
        private CancellationResponse _cancellationResponse = CancellationResponse.SuccessfullyCancelled;
        private CancelVoucherController _sut;

        [SetUp]
        public void Setup()
        {
            _mockVoucherCancellationService = new Mock<IVoucherCancellationService>();
            _mockVoucherCancellationService.Setup(x => x.CancelVoucherFromEmailLink(It.IsAny<long>(), It.IsAny<string>()))            
            .ReturnsAsync((long eid, string email) => _cancellationResponse);

            _mockLogger = new Mock<ILogger<CancelVoucherController>>();
            _sut = new CancelVoucherController(_mockVoucherCancellationService.Object, _mockLogger.Object, Options.Create(new UrlOptions { LearningPlatformUrl = "https://localhost:44326" }));
        }

        [Test]
        public async Task CancelVoucherFromLinkWithValidTokenInTrialPeriod()
        {
            _cancellationResponse = CancellationResponse.SuccessfullyCancelled;

            var result = await _sut.Index(1, "test@test.com");
            var viewResult = (ViewResult)result;
            Assert.AreEqual(null, viewResult.ViewName);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithValidTokenAlreadyExpired()
        {
            _cancellationResponse = CancellationResponse.TokenExpired;

            var result = await _sut.Index(1, "test@test.com");
            var viewResult = (ViewResult)result;
            Assert.AreEqual(null, viewResult.ViewName);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithValidTokenAfterTrialPeriod()
        {
            _cancellationResponse = CancellationResponse.FreeTrialExpired;

            var result = await _sut.Index(1, "test@test.com");
            var viewResult = (ViewResult)result;
            Assert.AreEqual("CantCancel", viewResult.ViewName);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithNoToken()
        {
            _cancellationResponse = CancellationResponse.TokenNotFound;

            var result = await _sut.Index(1, "test@test.com");
            var viewResult = (ViewResult)result;
            Assert.AreEqual("CantCancel", viewResult.ViewName);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithValidTokenAlreadyCancelled()
        {
            _cancellationResponse = CancellationResponse.AlreadyCancelled;

            var result = await _sut.Index(1, "test@test.com");
            var viewResult = (ViewResult)result;
            Assert.AreEqual("CantCancel", viewResult.ViewName);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithUnknownError()
        {
            _cancellationResponse = CancellationResponse.UnknownError;

            var result = await _sut.Index(1, "test@test.com");
            var viewResult = (ViewResult)result;
            Assert.AreEqual("CantCancel", viewResult.ViewName);
        }

        [Test]
        public async Task CancelVoucherFromWithInvalidEntityId()
        {
            _cancellationResponse = CancellationResponse.EnterpriseNotFound;

            var result = await _sut.Index(1, "test@test.com");
            var viewResult = (ViewResult)result;
            Assert.AreEqual("CantCancel", viewResult.ViewName);
        }

        [Test]
        public async Task CancelVoucherFromWithInvalidEmail()
        {
            _cancellationResponse = CancellationResponse.InvalidEmail;

            var result = await _sut.Index(1, "test@test.com");
            var viewResult = (ViewResult)result;
            Assert.AreEqual("CantCancel", viewResult.ViewName);
        }
    }
}
