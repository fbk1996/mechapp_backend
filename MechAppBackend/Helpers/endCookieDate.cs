namespace MechAppBackend.Helpers
{
    public class endCookieDate
    {
        public static DateTime GetEndCookieDate()
        {
            // Get the timezone information for Europe/Warsaw
            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"); // Warsaw time zone

            // Get the current time in Warsaw timezone
            DateTime timeInWarsaw = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, tz);

            timeInWarsaw = timeInWarsaw.AddHours(4);

            return timeInWarsaw;
        }
    }
}
