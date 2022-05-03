using Beis.HelpToGrow.Core.Repositories.Interface;
using Beis.Htg.VendorSme.Database.Models;
using BEIS.HelpToGrow.Core.Enums;
using BEIS.HelpToGrow.Voucher.Web.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.ApplyForDiscount.Services
{
    [TestFixture]
    public class VoucherCancellationServiceTests
    {
        private VoucherCancellationService _sut;
        private Mock<ILogger<VoucherCancellationService>> _mockLogger;
        private Mock<IEnterpriseRepository> _mockEnterpriseRepo;
        private Mock<ITokenRepository> _mockTokenRepo;
        private Mock<IProductPriceRepository> _mockProductPriceRepo;

        private enterprise enterprise;
        private token token;
        private product_price productPrice;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<VoucherCancellationService>>();
            _mockEnterpriseRepo = new Mock<IEnterpriseRepository>();
            _mockEnterpriseRepo.Setup(x => x.GetEnterprise(It.IsAny<long>()))
                .ReturnsAsync((long id) => enterprise);
            _mockTokenRepo = new Mock<ITokenRepository>();
            _mockTokenRepo.Setup(x => x.GetToken(It.IsAny<long>()))
                .ReturnsAsync((long id) => token);
            _mockProductPriceRepo = new Mock<IProductPriceRepository>();
            _mockProductPriceRepo.Setup(x => x.GetByProductId(It.IsAny<long>()))
                .ReturnsAsync((long id) => productPrice);
            _sut = new VoucherCancellationService(_mockLogger.Object, _mockEnterpriseRepo.Object, _mockTokenRepo.Object, _mockProductPriceRepo.Object);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithValidTokenInTrialPeriod()
        {
            enterprise = new enterprise { applicant_email_address = "test@test.com" };
            token = new token { redemption_date = DateTime.Now.AddDays(-5), redemption_status_id = 1 };
            productPrice = new product_price { free_trial_term_no = 12, free_trial_term_unit = "days"};

            var result = await _sut.CancelVoucherFromEmailLink(1, "test@test.com");           
            Assert.AreEqual(CancellationResponse.SuccessfullyCancelled, result);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithValidTokenAfterTrialPeriod()
        {
            enterprise = new enterprise { applicant_email_address = "test@test.com" };
            token = new token { redemption_date = DateTime.Now.AddDays(-25), redemption_status_id = 1 };
            productPrice = new product_price { free_trial_term_no = 12, free_trial_term_unit = "days" };

            var result = await _sut.CancelVoucherFromEmailLink(1, "test@test.com");
            Assert.AreEqual(CancellationResponse.FreeTrialExpired, result);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithNoToken()
        {
            enterprise = new enterprise { applicant_email_address = "test@test.com" };
            token = null;
            productPrice = new product_price { free_trial_term_no = 12, free_trial_term_unit = "days" };

            var result = await _sut.CancelVoucherFromEmailLink(1, "test@test.com");
            Assert.AreEqual(CancellationResponse.TokenNotFound, result);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithValidTokenAlreadyCancelled()
        {          
            enterprise = new enterprise { applicant_email_address = "test@test.com" };
            token = new token { redemption_date = DateTime.Now.AddDays(-25), redemption_status_id = 1, cancellation_status_id = 2 };
            productPrice = new product_price { free_trial_term_no = 12, free_trial_term_unit = "days" };

            var result = await _sut.CancelVoucherFromEmailLink(1, "test@test.com");
            Assert.AreEqual(CancellationResponse.AlreadyCancelled, result);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithValidTokenAlreadyCancelledAndCannotReapply()
        {
            enterprise = new enterprise { applicant_email_address = "test@test.com" };
            token = new token { redemption_date = DateTime.Now.AddDays(-25), redemption_status_id = 1, cancellation_status_id = 1 };
            productPrice = new product_price { free_trial_term_no = 12, free_trial_term_unit = "days" };

            var result = await _sut.CancelVoucherFromEmailLink(1, "test@test.com");
            Assert.AreEqual(CancellationResponse.FreeTrialExpired, result);
        }

        [Test]
        public async Task CancelVoucherFromLinkWithValidTokenAlreadyExpired()
        {
            enterprise = new enterprise { applicant_email_address = "test@test.com" };
            token = new token { redemption_date = DateTime.Now.AddDays(-25), redemption_status_id = 1, token_expiry = DateTime.Now.AddDays(-3) };
            productPrice = new product_price { free_trial_term_no = 12, free_trial_term_unit = "days" };

            var result = await _sut.CancelVoucherFromEmailLink(1, "test@test.com");
            Assert.AreEqual(CancellationResponse.FreeTrialExpired, result);
        }

        [Test]
        public async Task CancelVoucherFromWithInvalidEntityId()
        {
            enterprise = null;
      
            var result = await _sut.CancelVoucherFromEmailLink(1, "test@test.com");
            Assert.AreEqual(CancellationResponse.EnterpriseNotFound, result);
        }

        [Test]
        public async Task CancelVoucherFromWithInvalidEmail()
        {

            enterprise = new enterprise { applicant_email_address = "test@testing.com" };
            token = new token { redemption_date = DateTime.Now.AddDays(-25), redemption_status_id = 1, token_expiry = DateTime.Now.AddDays(-3) };
            productPrice = new product_price { free_trial_term_no = 12, free_trial_term_unit = "days" };

            var result = await _sut.CancelVoucherFromEmailLink(1, "test@test.com");
            Assert.AreEqual(CancellationResponse.InvalidEmail, result);
        }
    }
}
