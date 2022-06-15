using System.IO;
using System.Text.Json;
using Beis.HelpToGrow.Voucher.Web.Models.Voucher;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied;

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [TestFixture]
    public class BR14Test
    {
        private Mock<ILogger<BR14>> _mockLogger;
        private BR14 _sut;
        private IndesserCompanyResponse _indesserCompanyResponse;
        private UserVoucherDto _userVoucherDto;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<BR14>>();
            _sut = new BR14(_mockLogger.Object, new ProtectFraudScore());
            _indesserCompanyResponse = new IndesserCompanyResponse();
            _indesserCompanyResponse.ScoresAndLimits = new ScoresAndLimits();            
            _userVoucherDto = new UserVoucherDto();
        }

        private static string GetFakeIndesserCompanyResponse() => File.OpenText("indessercompanycheckresponse.json").ReadToEnd();

        [Test]
        public void PositiveProtectScore()
        {
            _indesserCompanyResponse.ScoresAndLimits.ProtectScore = 1;

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void LowestEligibleProtectScore()
        {
            _indesserCompanyResponse.ScoresAndLimits.ProtectScore = -199;

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void IneligibleProtectScore()
        {
            _indesserCompanyResponse.ScoresAndLimits.ProtectScore = -200;

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsFailed);
        }

        [Test]
        public void NamedDescribed()
        {
            Assert.That(!string.IsNullOrWhiteSpace(_sut.Name));
            Assert.That(!string.IsNullOrWhiteSpace(_sut.Description));
        }
    }
}