using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tmg.equinox.applicationservices.ReportingCenter
{
    public static class DateConverter
    {
        public static DateTime GetEstDateTime(this DateTime sourceDate)
        {
            if (sourceDate == null)
                sourceDate = DateTime.UtcNow;

            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(sourceDate, easternZone);
            return easternTime;
        }
    }
}
