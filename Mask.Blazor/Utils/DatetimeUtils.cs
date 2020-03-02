using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TimeZoneConverter;

namespace Mask.Blazor.Utils
{
    public class DatetimeUtils
    {
        public static DateTime GetChinaDatetimeNow() => DateTime.UtcNow + ChinaTimeZone.BaseUtcOffset;
        public static TimeZoneInfo ChinaTimeZone { get; } = TZConvert.GetTimeZoneInfo("China Standard Time");
    }
}
