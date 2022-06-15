
namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [TestFixture]
    public class BR04Test
    {
        private Mock<ILogger<BR04>> _mockLogger;
        private BR04 _sut;
        private IndesserCompanyResponse _indesserCompanyResponse;
        private IVerifyEmployeeCount _verification;
        private UserVoucherDto _userVoucherDto;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<BR04>>();
            _verification = new EmployeeCount();
            _sut = new BR04(_mockLogger.Object, _verification);
            _indesserCompanyResponse = new IndesserCompanyResponse();
            _userVoucherDto = new UserVoucherDto();
        }

        [Test]
        public void ValidEmployeeCount()
        {
            _userVoucherDto.EmployeeNumbers = 5;

            _indesserCompanyResponse.Financials = new List<Financial> { new()
                {
                    FinancialData = new FinancialData
                    {
                        NumberofEmployees = 5.0
                    }
                }
            };

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void MissingFinancial()
        {
            _userVoucherDto.EmployeeNumbers = 5;

            _indesserCompanyResponse.Financials = new List<Financial>();

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void MissingFinancialData()
        {
            _userVoucherDto.EmployeeNumbers = 5;

            _indesserCompanyResponse.Financials = new List<Financial> { new() };

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void InvalidEmployeeCountPerUserResponse()
        {
            _userVoucherDto.EmployeeNumbers = 4;

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsFailed);
        }

        [Test]
        public void InvalidEmployeeCountTooSmallPerIndesserResponse()
        {
            _userVoucherDto.EmployeeNumbers = 5;

            _indesserCompanyResponse.Financials = new List<Financial> { new()
                {
                    FinancialData = new FinancialData
                    {
                        NumberofEmployees = 4.0
                    }
                }
            };
            
            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsFailed);
        }

        [Test]
        public void InvalidEmployeeCountTooLargePerIndesserResponse()
        {
            _userVoucherDto.CompanySize = "Yes";

            _indesserCompanyResponse.Financials = new List<Financial> { new()
                {
                    FinancialData = new FinancialData
                    {
                        NumberofEmployees = 250.0
                    }
                }
            };

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
