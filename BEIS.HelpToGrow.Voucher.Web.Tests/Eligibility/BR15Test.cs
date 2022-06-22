
namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [TestFixture]
    public class BR15Test
    {
        private Mock<ILogger<BR15>> _mockLogger;
        private BR15 _sut;
        private IndesserCompanyResponse _indesserCompanyResponse;
        private UserVoucherDto _userVoucherDto;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<BR15>>();
            _sut = new BR15(_mockLogger.Object, new ScoreCheck());
            _indesserCompanyResponse = new IndesserCompanyResponse();
            _indesserCompanyResponse.ScoresAndLimits = new ScoresAndLimits();
            _userVoucherDto = new UserVoucherDto();
        }

        private static string GetFakeIndesserCompanyResponse() => File.OpenText("indessercompanycheckresponse.json").ReadToEnd();

        [Test]
        public void EligibleScore()
        {
            _indesserCompanyResponse.ScoresAndLimits.ScoreGrade = "X";

            var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

            Assert.That(result.IsSuccess);
        }

        [Test]
        public void IneligibleScores()
        {
            var badScoreGrades = new[] { "G", "I", "O", "N", "NA", "NR", "NT" };

            foreach (var badScoreGrade in badScoreGrades)
            {
                _indesserCompanyResponse.ScoresAndLimits.ScoreGrade = badScoreGrade;

                var result = _sut.Check(_indesserCompanyResponse, _userVoucherDto);

                Assert.That(result.IsFailed, "Score grade '" + badScoreGrade + "' should have failed check");
            }
        }

        [Test]
        public void NamedDescribed()
        {
            Assert.That(!string.IsNullOrWhiteSpace(_sut.Name));
            Assert.That(!string.IsNullOrWhiteSpace(_sut.Description));
        }
    }
}