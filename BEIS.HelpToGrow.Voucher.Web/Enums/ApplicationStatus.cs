using System;
using System.Collections.Generic;
using System.Text;

namespace Beis.HelpToGrow.Voucher.Web.Enums
{
    public enum ApplicationStatus
    {
        NewApplication,
        EmailNotVerified,
        EmailVerified,
        Ineligible, // failed indesser
        Eligible,
        CancelledCannotReApply,
        CancelledInFreeTrialCanReApply,
        CancelledNotRedeemedCanReApply,
        ActiveTokenNotRedeemed,
        ActiveTokenRedeemed,
        TokenReconciled,
        TokenExpired

    }
}
