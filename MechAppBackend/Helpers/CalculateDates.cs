﻿using Microsoft.Extensions.Logging;
using Nager.Holiday;

namespace MechAppBackend.Helpers
{
    public class CalculateDates
    {
        public static List<DateTime> holidayDays()
        {
            List<DateTime> days = new List<DateTime>();

            using (HolidayClient hc = new HolidayClient())
            {
                try
                {
                    var datesGet = hc.GetHolidaysAsync(Convert.ToInt32(DateTime.Now.ToString("yyyy")), "pl");

                    for (var i = 0; i < datesGet.Result.Length; i++)
                    {
                        DateTime hDay = Convert.ToDateTime(datesGet.Result[i].Date);
                        days.Add(hDay);
                    }
                }
                catch (Exception ex)
                {
                    Logger.SendNormalException("Mechapp", "CalculateDates", "holidaysDates", ex);
                }
            }

            return days;
        }

        public static int BusinessDaysUntil(DateTime firstDay, DateTime lastDay, List<DateTime> bankHolidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday
                    ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday
                    ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }

        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

            return firstDayOfMonth;
        }

        public static DateTime GetLastDayOfMonth(DateTime firstDayOfMonth)
        {
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);

            return lastDayOfMonth;
        }
    }
}
