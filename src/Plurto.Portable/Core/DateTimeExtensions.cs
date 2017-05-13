using System;

namespace Plurto.Core
{
    public static class DateTimeExtensions
    {
        public static DateTime Min(DateTime d1, DateTime d2)
        {
            return d1 < d2 ? d1 : d2;
        }
        public static long ToTimestamp(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime().Ticks - 621355968000000000L;
        }
        public static DateTime ToDateTimeLocal(this long timestamp)
        {
            return new DateTime(timestamp + 621355968000000000L).ToLocalTime();
        }
        public static DateTime ToDateTimeUtc(this long timestamp)
        {
            return new DateTime(timestamp + 621355968000000000L, DateTimeKind.Utc);
        }
        public static string ToMySqlDateTime(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd hh-mm-ss");
        }
        public static string ToMySqlDate(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd");
        }
    }
}
