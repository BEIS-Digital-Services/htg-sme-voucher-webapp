using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Voucher.FunctionApp
{
    public class ScheduleInfo
    {
        // At this time TimerInfo is not supported in an isolated process. This needs to be used as a substitute
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
