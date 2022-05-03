using System;
using System.Collections.Generic;
using System.Text;

namespace BEIS.HelpToGrow.Core.Enums
{
    public enum CancellationResponse
    {
        SuccessfullyCancelled,
        EnterpriseNotFound,
        InvalidEmail,
        TokenNotFound,
        FreeTrialExpired,
        AlreadyCancelled,
        TokenExpired,
        UnknownError
    }
}
