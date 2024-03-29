﻿using System.Text.Json.Serialization;

namespace Beis.HelpToGrow.Voucher.Web.Config
{
    public class EligibilityRulesSection
    {
        [JsonPropertyName("maxFailCount")]
        public int MaxFailCount { get; set; }

        [JsonPropertyName("rules")] 
        public Dictionary<string, EligibilityRuleSetting> Rules { get; set; }
    }
}