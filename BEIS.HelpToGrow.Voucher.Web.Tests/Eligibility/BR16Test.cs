using System.IO;
using System.Text.Json;
using Beis.HelpToGrow.Voucher.Web.Models.Voucher;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied;

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [TestFixture]
    public class BR16Test
    {
        private Mock<ILogger<BR16>> _mockLogger;
        private BR16 _sut;
        private IndesserCompanyResponse _indesserCompanyResponse;
        private UserVoucherDto _userVoucherDto;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<BR16>>();
            _sut = new BR16(_mockLogger.Object, new MortgagePresent());
            _indesserCompanyResponse = new IndesserCompanyResponse();
            _indesserCompanyResponse.Characteristics = new System.Collections.Generic.List<Services.Connectors.Domain.Characteristic>();
            _userVoucherDto = new UserVoucherDto();
        }

        private static string GetFakeIndesserCompanyResponse() => File.OpenText("indessercompanycheckresponse.json").ReadToEnd();

        [Test]
        public void MortgagePresent()
        {
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.MortgagePresent, Value = "1" });
            
            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);
            Assert.That(result.IsSuccess);
        }

        [Test]
        public void MortgageNotPresent()
        {
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.MortgagePresent, Value = "2" });
            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);
            Assert.That(result.IsFailed, $"Mortgage present value is {_indesserCompanyResponse.Characteristic(Characteristics.MortgagePresent).Value}");
        }

        [Test]
        public void NamedDescribed()
        {
            Assert.That(!string.IsNullOrWhiteSpace(_sut.Name));
            Assert.That(!string.IsNullOrWhiteSpace(_sut.Description));
        }
    }
}