using System.IO;
using System.Text.Json;
using Beis.HelpToGrow.Voucher.Web.Models.Voucher;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Beis.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Rules;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility.Verification.Applied;
using Beis.HelpToGrow.Voucher.Web.Services.Eligibility;

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [TestFixture]
    public class BR10Test
    {
        private Mock<ILogger<BR10>> _mockLogger;
        private BR10 _sut;
        private IndesserCompanyResponse _indesserCompanyResponse;
        private UserVoucherDto _userVoucherDto;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<BR10>>();
            _sut = new BR10(_mockLogger.Object, new HoldingCompanyRegistration());
            _indesserCompanyResponse = new IndesserCompanyResponse();
            _indesserCompanyResponse.Characteristics = new System.Collections.Generic.List<Services.Connectors.Domain.Characteristic>();
            _userVoucherDto = new UserVoucherDto();
        }

        private static string GetFakeIndesserCompanyResponse() => File.OpenText("indessercompanycheckresponse.json").ReadToEnd();

        [Test]
        public void NoAbnormalAccountFiling()
        {            
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.HoldingCompanyRegistration, Value = "0" });
            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void AbnormalAccountFiling()
        {
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.HoldingCompanyRegistration, Value = "1" });

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