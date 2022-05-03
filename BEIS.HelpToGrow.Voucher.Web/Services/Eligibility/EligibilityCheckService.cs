using System;
using System.Collections.Generic;
using System.Linq;
using BEIS.HelpToGrow.Voucher.Web.Config;
using BEIS.HelpToGrow.Voucher.Web.Models.Voucher;
using BEIS.HelpToGrow.Voucher.Web.Services.Connectors.Domain;
using BEIS.HelpToGrow.Voucher.Web.Services.Eligibility.Rules;
using FluentResults;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BEIS.HelpToGrow.Voucher.Web.Services.Eligibility
{
    public class EligibilityCheckService : ICheckEligibility
    {
        private readonly ILogger<EligibilityCheckService> _logger;
        private readonly IOptions<EligibilityRules> _eligibilityRulesOptions;
        private readonly IEnumerable<ICheckEligibilityRule> _eligibilityRuleChecks;

        public EligibilityCheckService(
            ILogger<EligibilityCheckService> logger,
            IOptions<EligibilityRules> eligibilityRulesOptions,
            IEnumerable<ICheckEligibilityRule> eligibilityRuleChecks)
        {
            _logger = logger;
            _eligibilityRulesOptions = eligibilityRulesOptions;
            _eligibilityRuleChecks = eligibilityRuleChecks;
        }

        public Result<Check> Check(UserVoucherDto userVoucherDto, IndesserCompanyResponse indesserCompanyResponse)
        {
            try
            {
                Check CheckSection(EligibilityRulesSection section) => GetResult(indesserCompanyResponse, userVoucherDto, section);

                var check = CheckSection(_eligibilityRulesOptions.Value.Core).Append(CheckSection(_eligibilityRulesOptions.Value.Additional));

                return Result.Ok(check);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking eligibility of indesser company response: {JsonConvert.SerializeObject(indesserCompanyResponse)}");

                return Result.Fail(ex.Message);
            }
        }

        private Check GetResult(
            IndesserCompanyResponse indesserCompanyResponse,
            UserVoucherDto userVoucherDto,
            EligibilityRulesSection eligibilityRules)
        {
            var result = Result.Ok();
            var requiringReview = Result.Ok();
            var recordedOnly = Result.Ok();

            foreach (var enabledEligibilityRule in Enabled(eligibilityRules))
            {
                var eligibilityRule = _eligibilityRuleChecks.FirstOrDefault(Matches(enabledEligibilityRule));

                if (eligibilityRule == null)
                {
                    continue;
                }

                foreach (var error in eligibilityRule.Check(indesserCompanyResponse, userVoucherDto).Errors)
                {
                    if (enabledEligibilityRule.Value.ContributesToFailCount)
                    {
                        result.WithError(error);
                    }

                    if (enabledEligibilityRule.Value.ContributesToReviewCount)
                    {
                        requiringReview.WithError(error);
                    }

                    if (!enabledEligibilityRule.Value.ContributesToFailCount &&
                        !enabledEligibilityRule.Value.ContributesToReviewCount)
                    {
                        recordedOnly.WithError(error);
                    }
                }
            }

            return new Check(
                result.Errors.Count <= eligibilityRules.MaxFailCount,
                result.Errors,
                requiringReview.Errors,
                recordedOnly.Errors);
        }

        private static Func<ICheckEligibilityRule, bool> Matches(KeyValuePair<string, EligibilityRuleSetting> enabledCoreEligibilityRule) =>
            _ => _.Id.Equals(enabledCoreEligibilityRule.Key);

        private static IEnumerable<KeyValuePair<string, EligibilityRuleSetting>> Enabled(EligibilityRulesSection coreEligibilityRules) =>
            coreEligibilityRules.Rules.Where(_ => _.Value.Enabled);
    }
}