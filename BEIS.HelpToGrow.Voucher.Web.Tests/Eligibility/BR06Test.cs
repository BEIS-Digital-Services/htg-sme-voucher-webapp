using System.IO;
using System.Text.Json;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility;

namespace BEIS.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [TestFixture]
    public class BR06Test
    {
        private Mock<ILogger<BR06>> _mockLogger;
        private BR06 _sut;
        private IndesserCompanyResponse _indesserCompanyResponse;
        private UserVoucherDto _userVoucherDto;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<BR06>>();
            _sut = new BR06(_mockLogger.Object, new FinancialAgreementProviders());
            _indesserCompanyResponse = new IndesserCompanyResponse();
            _indesserCompanyResponse.Characteristics = new System.Collections.Generic.List<Services.Connectors.Domain.Characteristic>();
            _userVoucherDto = new UserVoucherDto();
        }

        private static string GetFakeIndesserCompanyResponse() => File.OpenText("indessercompanycheckresponse.json").ReadToEnd();

        [Test]
        public void WithFinancialProviderAgreement()
        {
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.FinancialAgreementProviders, Value = "1" });            

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void WithoutFinancialProviderAgreement()
        {
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.FinancialAgreementProviders, Value = "0" });            

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsFailed);
        }

        [Test]
        public void InvalidFinancialProviderAgreement()
        {
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.FinancialAgreementProviders, Value = "X" });

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