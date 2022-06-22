using System.Text.Json;

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [Ignore("Not yet able to run on server")]
    [TestFixture]
    public class ConfigTests
    {
        private WebApplicationFactory<Startup> _factory;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Startup>();
        }

        [TearDown]
        public void TearDown()
        {
            _factory?.Dispose();
        }

        /// <summary>
        /// https://bit.ly/Business-Rules-for-SME-Eligibility-Check-WIP
        /// </summary>
        [Test]
        public async Task ConfigurationMatchesSpecification()
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/api/{nameof(EligibilityRules)}");
            var responseContent = await response.Content.ReadAsStringAsync();
            var eligibilityRules = JsonSerializer.Deserialize<EligibilityRules>(responseContent);

            Assert.NotNull(eligibilityRules);

            Assert.AreEqual(0, eligibilityRules.Core.MaxFailCount);
            Assert.AreEqual(4, eligibilityRules.Core.Rules.Count);
            Assert.True(eligibilityRules.Core.Rules.All(_ => _.Value.Enabled));

            Assert.Contains("BR01", eligibilityRules.Core.Rules.Keys);
            Assert.Contains("BR02", eligibilityRules.Core.Rules.Keys);
            Assert.Contains("BR03", eligibilityRules.Core.Rules.Keys);
            Assert.Contains("BR14", eligibilityRules.Core.Rules.Keys);

            Assert.AreEqual(12, eligibilityRules.Additional.MaxFailCount);
            Assert.AreEqual(12, eligibilityRules.Additional.Rules.Count);
            Assert.True(eligibilityRules.Additional.Rules.All(_ => _.Value.Enabled));

            Assert.Contains("BR04", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR05", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR06", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR07", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR08", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR09", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR10", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR11", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR12", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR13", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR15", eligibilityRules.Additional.Rules.Keys);
            Assert.Contains("BR16", eligibilityRules.Additional.Rules.Keys);
        }
    }
}
