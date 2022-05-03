﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BEIS.HelpToGrow.Voucher.Web.Models
{
    public class TermsConditionsViewModel : UsefulLinksViewModel
    {
        public string SelectedProduct { get; set; }

        [Required]
        public string TermsConditions { get; set; }
        [Required]
        public string PrivacyPolicy { get; set; }
        [Required]
        public string SubsidyControl { get; set; }

        public string MarketingConsent { get; set; }

        public bool IsIncomplete =>
            string.IsNullOrWhiteSpace(TermsConditions) ||
            string.IsNullOrWhiteSpace(PrivacyPolicy) ||
            string.IsNullOrWhiteSpace(SubsidyControl);

        public static Uri BusinessEligibilityUrl => Urls.BusinessEligibilityUrl;
    }
}