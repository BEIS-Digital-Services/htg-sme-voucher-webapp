

namespace Beis.HelpToGrow.Voucher.Web.Tests.Eligibility
{
    [TestFixture]
    public class CoreEligibilityChecksTests
    {
        private ICheckEligibility _eligibility;
        private List<ICheckEligibilityRule> _eligibilityRulesChecks;
        private IndesserCompanyResponse _indesserCompanyResponse;
        private UserVoucherDto _userVoucherDto;
        private Mock<ILogger<EligibilityCheckService>> _mockLogger;
        private static EligibilityRuleSetting Enabled => new()
        {
            Enabled = true,
            ContributesToFailCount = true
        };
        private static EligibilityRuleSetting Disabled => new()
        {
            Enabled = false,
            ContributesToFailCount = true
        };

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<EligibilityCheckService>>();
            _indesserCompanyResponse = new IndesserCompanyResponse();
            _userVoucherDto = new UserVoucherDto();
        }

        [Test]
        public void RunsEnabledCheck()
        {
            var ruleIds = new[] { "BR01" };

            var eligibilityRules = new EligibilityRules
            {
                Core = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting> { { ruleIds[0], Enabled } }
                },
                Additional = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting>()
                }
            };

            _eligibilityRulesChecks = new List<ICheckEligibilityRule>();

            var rules = ruleIds.Select(GetPassingRule).ToList();

            rules.ForEach(rule => _eligibilityRulesChecks.Add(rule.Object));

            _eligibility = new EligibilityCheckService(_mockLogger.Object, Options.Create(eligibilityRules), _eligibilityRulesChecks);

            _eligibility.Check(_userVoucherDto, _indesserCompanyResponse);

            rules[0].Verify(_ => _.Check(_indesserCompanyResponse, _userVoucherDto), Times.Once);
        }

        [Test]
        public void IgnoresDisabledCheck()
        {
            var ruleIds = new[] { "BR01" };

            var eligibilityRules = new EligibilityRules
            {
                Core = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting>  { { ruleIds[0], Disabled } }
                },
                Additional = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting> ()
                }
            };

            _eligibilityRulesChecks = new List<ICheckEligibilityRule>();

            var rules = ruleIds.Select(GetPassingRule).ToList();

            rules.ForEach(rule => _eligibilityRulesChecks.Add(rule.Object));

            _eligibility = new EligibilityCheckService(_mockLogger.Object, Options.Create(eligibilityRules), _eligibilityRulesChecks);

            _eligibility.Check(_userVoucherDto, _indesserCompanyResponse);

            rules[0].Verify(_ => _.Check(_indesserCompanyResponse, _userVoucherDto), Times.Never);
        }

        [Test]
        public void RunsOnlyEnabledChecks()
        {
            var ruleIds = new[] {"BR01", "BR02", "BR03"};

            var eligibilityRules = new EligibilityRules
            {
                Core = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting> 
                    {
                        {ruleIds[0], Enabled},
                        {ruleIds[1], Disabled},
                        {ruleIds[2], Enabled}
                    }
                },
                Additional = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting> ()
                }
            };

            _eligibilityRulesChecks = new List<ICheckEligibilityRule>();

            var rules = ruleIds.Select(GetPassingRule).ToList();

            rules.ForEach(rule => _eligibilityRulesChecks.Add(rule.Object));

            _eligibility = new EligibilityCheckService(_mockLogger.Object, Options.Create(eligibilityRules), _eligibilityRulesChecks);
            
            _eligibility.Check(_userVoucherDto, _indesserCompanyResponse);

            rules[0].Verify(_ => _.Check(_indesserCompanyResponse, _userVoucherDto), Times.Once);
            rules[1].Verify(_ => _.Check(_indesserCompanyResponse, _userVoucherDto), Times.Never);
            rules[2].Verify(_ => _.Check(_indesserCompanyResponse, _userVoucherDto), Times.Once);
        }

        [Test]
        public void NoFailedChecksAllowed()
        {
            var ruleIds = new[] { "BR01" };

            var eligibilityRules = new EligibilityRules
            {
                Core = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting>  { { ruleIds[0], Enabled } }
                },
                Additional = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting> ()
                }
            };

            _eligibilityRulesChecks = new List<ICheckEligibilityRule>();

            var rules = ruleIds.Select(GetPassingRule).ToList();

            rules.ForEach(rule => _eligibilityRulesChecks.Add(rule.Object));

            _eligibility = new EligibilityCheckService(_mockLogger.Object, Options.Create(eligibilityRules), _eligibilityRulesChecks);

            var result = _eligibility.Check(_userVoucherDto, _indesserCompanyResponse);

            Assert.That(result.Value.IsEligible);
        }

        [Test]
        public void SingleFailedCheckAllowed()
        {
            var ruleIds = new[] { "BR01" };

            var eligibilityRules = new EligibilityRules
            {
                Core = new EligibilityRulesSection
                {
                    MaxFailCount = 1,
                    Rules = new Dictionary<string, EligibilityRuleSetting>  { { ruleIds[0], Enabled } }
                },
                Additional = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting> ()
                }
            };

            _eligibilityRulesChecks = new List<ICheckEligibilityRule>();

            var rules = ruleIds.Select(GetFailingRule).ToList();

            rules.ForEach(rule => _eligibilityRulesChecks.Add(rule.Object));

            _eligibility = new EligibilityCheckService(_mockLogger.Object, Options.Create(eligibilityRules), _eligibilityRulesChecks);

            var result = _eligibility.Check(_userVoucherDto, _indesserCompanyResponse);

            Assert.That(result.Value.IsEligible);
        }

        [Test]
        public void ExceedsMaxFailCount()
        {
            var ruleIds = new[] { "BR01", "BR02" };

            var eligibilityRules = new EligibilityRules
            {
                Core = new EligibilityRulesSection
                {
                    MaxFailCount = 1,
                    Rules = new Dictionary<string, EligibilityRuleSetting> 
                    {
                        {ruleIds[0], Enabled},
                        {ruleIds[1], Enabled}
                    }
                },
                Additional = new EligibilityRulesSection
                {
                    MaxFailCount = 0,
                    Rules = new Dictionary<string, EligibilityRuleSetting> ()
                }
            };

            _eligibilityRulesChecks = new List<ICheckEligibilityRule>();

            var rules = ruleIds.Select(GetFailingRule).ToList();

            rules.ForEach(rule => _eligibilityRulesChecks.Add(rule.Object));

            _eligibility = new EligibilityCheckService(_mockLogger.Object, Options.Create(eligibilityRules), _eligibilityRulesChecks);

            var result = _eligibility.Check(_userVoucherDto, _indesserCompanyResponse);

            Assert.That(!result.Value.IsEligible);
        }

        private Mock<ICheckEligibilityRule> GetPassingRule(string id) => GetRule(id, true);

        private Mock<ICheckEligibilityRule> GetFailingRule(string id) => GetRule(id, false);

        private Mock<ICheckEligibilityRule> GetRule(string id, bool ok)
        {
            Result FakeResult() => ok ? Result.Ok() : Result.Fail(id);

            var mockRule = new Mock<ICheckEligibilityRule>();
            mockRule.SetupGet(_ => _.Id).Returns(id);
            mockRule.Setup(_ => _.Check(_indesserCompanyResponse, _userVoucherDto)).Returns(FakeResult());

            return mockRule;
        }
    }
}