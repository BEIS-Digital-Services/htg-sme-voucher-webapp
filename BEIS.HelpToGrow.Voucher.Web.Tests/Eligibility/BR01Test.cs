

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [TestFixture]
    public class BR01Test
    {
        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<BR01>>();
            _verification = new PostcodePattern();
            _sut = new BR01(_mockLogger.Object, _verification);
            _indesserCompanyResponse = new IndesserCompanyResponse();
            _indesserCompanyResponse.Characteristics = new System.Collections.Generic.List<Services.Connectors.Domain.Characteristic>();
            _indesserCompanyResponse.Identification = new Identification();
            _indesserCompanyResponse.Identification.RegisteredOffice = new RegisteredOffice();
            _indesserCompanyResponse.Identification.RegisteredOffice.postcode = "SE16 7TU";
            _userVoucherDto = new UserVoucherDto();
        }

        private Mock<ILogger<BR01>> _mockLogger;
        private BR01 _sut;
        private PostcodePattern _verification;
        private IndesserCompanyResponse _indesserCompanyResponse;
        private UserVoucherDto _userVoucherDto;

        private static string GetFakeIndesserCompanyResponse()
        {
            
            return File.OpenText("indessercompanycheckresponse.json").ReadToEnd();
        }

        [Test]
        public void ValidPostcode()
        {
            
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.Postcode, Value = "SE16 7TU" });
            

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void WillUseRegisteredOfficePostcodeIfCharacteristicPostcodeFails()
        {
            
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.Postcode, Value = "X16X X7X" });

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void WillUseRegisteredOfficePostcodeIfCharacteristicIsMissing()
        {
            
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.Postcode, Value = null });
            _indesserCompanyResponse.Identification.RegisteredOffice.postcode = "SE16 7TU";
            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void WillUseRegisteredOfficePostcodeIfCharacteristicPostcodeIsMissing()
        {
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.Postcode, Value = null });
            _indesserCompanyResponse.Identification.RegisteredOffice.postcode = "SE16 7TU";

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void InvalidPostcode()
        {
            _indesserCompanyResponse.Identification.RegisteredOffice.postcode = "X16X X7X";
            _indesserCompanyResponse.Characteristics.Add(new Services.Connectors.Domain.Characteristic { Name = Characteristics.Postcode, Value = "X16X X7X" });
            

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