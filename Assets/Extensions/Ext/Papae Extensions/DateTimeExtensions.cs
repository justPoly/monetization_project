using System;
using System.Globalization;

namespace Papae.UnitySDK.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Gets the actual age from the specified dateOfBirth.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth.</param>
        public static int Age(this DateTime dateOfBirth)
        {
            if (DateTime.Today.Month < dateOfBirth.Month 
                || DateTime.Today.Month == dateOfBirth.Month 
                && DateTime.Today.Day < dateOfBirth.Day)
            {
                return DateTime.Today.Year - dateOfBirth.Year - 1;
            }

            return DateTime.Today.Year - dateOfBirth.Year;
        }

        /// <summary>
        /// Beginning the of the month.
        /// </summary>
        /// <returns>The of the month.</returns>
        /// <param name="date">Date.</param>
        public static DateTime BeginningOfTheMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        /// <summary>
        /// Ends the of the month.
        /// </summary>
        /// <returns>The of the month.</returns>
        /// <param name="date">Date.</param>
        public static DateTime EndOfTheMonth(this DateTime date)
        {
            var endOfTheMonth = new DateTime(date.Year, date.Month, 1)
                .AddMonths(1)
                .AddDays(-1);

            return endOfTheMonth;
        }

        public static double GetCurrentTime()
        {
            TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            return span.TotalSeconds;
        }

        public static double GetCurrentTimeInDays()
        {
            TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            return span.TotalDays;
        }

        public static double GetCurrentTimeInMills()
        {
            TimeSpan span = DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
            return span.TotalMilliseconds;
        }

        /// <summary>
        /// Gets the Sunday DateTime from the week of DateTime
        /// </summary>
        /// <returns>The sunday.</returns>
        /// <param name="dateTime">Date time.</param>
        public static DateTime GetSunday(this DateTime dateTime)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            return new GregorianCalendar().AddDays(date, -((int)date.DayOfWeek));
        }

        /// <summary>
        /// Gets the Monday DateTime from the week of DateTime
        /// </summary>
        /// <returns>The monday.</returns>
        /// <param name="dateTime">Date time.</param>
        public static DateTime GetMonday(this DateTime dateTime)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            return new GregorianCalendar().AddDays(date, -((int)date.DayOfWeek) + 1);
        }

        /// <summary>
        /// Gets the Tuesday DateTime from the week of DateTime
        /// </summary>
        /// <returns>The tuesday.</returns>
        /// <param name="dateTime">Date time.</param>
        public static DateTime GetTuesday(this DateTime dateTime)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            return new GregorianCalendar().AddDays(date, -((int)date.DayOfWeek) + 2);
        }

        /// <summary>
        /// Gets the Wednesday DateTime from the week of DateTime
        /// </summary>
        /// <returns>The wednesday.</returns>
        /// <param name="dateTime">Date time.</param>
        public static DateTime GetWednesday(this DateTime dateTime)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            return new GregorianCalendar().AddDays(date, -((int)date.DayOfWeek) + 3);
        }

        /// <summary>
        /// Gets the Thursday DateTime from the week of DateTime
        /// </summary>
        /// <returns>The thursday.</returns>
        /// <param name="dateTime">Date time.</param>
        public static DateTime GetThursday(this DateTime dateTime)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            return new GregorianCalendar().AddDays(date, -((int)date.DayOfWeek) + 4);
        }

        /// <summary>
        /// Gets the Friday DateTime from the week of DateTime.
        /// </summary>
        /// <returns>The friday.</returns>
        /// <param name="dateTime">Date time.</param>
        public static DateTime GetFriday(this DateTime dateTime)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            return new GregorianCalendar().AddDays(date, -((int)date.DayOfWeek) + 5);
        }

        /// <summary>
        /// Gets the Saturday DateTime from the week of DateTime.
        /// </summary>
        /// <returns>The saturday.</returns>
        /// <param name="dateTime">Date time.</param>
        public static DateTime GetSaturday(this DateTime dateTime)
        {
            DateTime date = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
            return new GregorianCalendar().AddDays(date, -((int)date.DayOfWeek) + 6);
        }

        /// <summary>
        /// Intersects the specified startDate, endDate, intersectingStartDate and intersectingEndDate.
        /// </summary>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <param name="intersectingStartDate">Intersecting start date.</param>
        /// <param name="intersectingEndDate">Intersecting end date.</param>
        public static bool Intersects(this DateTime startDateTime, DateTime endDateTime, DateTime intersectingStartDateTime, DateTime intersectingEndDateTime)
        {
            return (intersectingEndDateTime >= startDateTime && intersectingStartDateTime <= endDateTime);
        }

        /// <summary>
        /// Determines if if the date is between the two provided dates (startDate and endDate)
        /// </summary>
        /// <returns><c>true</c> if is between the specified dt startDate endDate compareTime; otherwise, <c>false</c>.</returns>
        /// <param name="dt">Dt.</param>
        /// <param name="startDate">Start date.</param>
        /// <param name="endDate">End date.</param>
        /// <param name="compareTime">If set to <c>true</c> compare time.</param>
        public static Boolean IsBetween(this DateTime dt, DateTime startDate, DateTime endDate, bool compareTime = false)
        {
            return compareTime ?
                dt >= startDate && dt <= endDate :
                dt.Date >= startDate.Date && dt.Date <= endDate.Date;
        }

        /// <summary>
        /// Ises the date in range.
        /// </summary>
        /// <returns><c>true</c>, if date in range was ised, <c>false</c> otherwise.</returns>
        /// <param name="toCheck">To check.</param>
        /// <param name="lowerBound">Lower bound.</param>
        /// <param name="upperBound">Upper bound.</param>
        public static bool IsDateInRange(this DateTime toCheck, DateTime lowerBound, DateTime upperBound)
        {
            if (lowerBound < toCheck && toCheck < upperBound)
                return true;
            return false;
        }

        /// <summary>
        /// Determines if is last day of the month the specified dateTime.
        /// </summary>
        /// <returns><c>true</c> if is last day of the month the specified dateTime; otherwise, <c>false</c>.</returns>
        /// <param name="dateTime">Date time.</param>
        public static bool IsLastDayOfTheMonth(this DateTime dateTime)
        {
            return dateTime == new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Ises the on same day as.
        /// </summary>
        /// <returns><c>true</c>, if on same day as was ised, <c>false</c> otherwise.</returns>
        /// <param name="dateTime">Date time.</param>
        /// <param name="other">Other.</param>
        public static bool IsOnSameDayAs(this DateTime dateTime, DateTime other)
        {
            if (dateTime.Day == other.Day && dateTime.Month == other.Month && dateTime.Year == other.Year)
                return true;
            return false;
        }

        /// <summary>
        /// Ises the on same day as.
        /// </summary>
        /// <returns><c>true</c>, if on same day as was ised, <c>false</c> otherwise.</returns>
        /// <param name="this">This.</param>
        /// <param name="day">Day.</param>
        /// <param name="month">Month.</param>
        /// <param name="year">Year.</param>
        public static bool IsOnSameDayAs(this DateTime @this, int day, int month, int year)
        {
            return @this.Day == day && @this.Month == month && @this.Year == year;
        }

        /// <summary>
        /// Determines if is weekend the specified value.
        /// </summary>
        /// <returns><c>true</c> if is weekend the specified value; otherwise, <c>false</c>.</returns>
        /// <param name="value">Value.</param>
        public static bool IsWeekend(this DateTime dateTime)
        {
            return (dateTime.DayOfWeek == DayOfWeek.Sunday || dateTime.DayOfWeek == DayOfWeek.Saturday);
        }

        /// <summary>
        /// Nexts the day of week.
        /// </summary>
        /// <returns>The day of week.</returns>
        /// <param name="dateTime">Dt.</param>
        /// <param name="day">Day.</param>
        public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek day)
        {
            var d = new GregorianCalendar().AddDays(dateTime, -((int)dateTime.DayOfWeek) + (int)day);
            return (d.Day < dateTime.Day) ? d.AddDays(7) : d;
        }

        /// <summary>
        /// Tos the friendly date string.
        /// </summary>
        /// <returns>The friendly date string.</returns>
        /// <param name="Date">Date.</param>
        public static string ToFriendlyDateString(this DateTime Date)
        {
            string FormattedDate = "";
            if (Date.Date == DateTime.Today)
            {
                FormattedDate = "Today";
            }
            else if (Date.Date == DateTime.Today.AddDays(-1))
            {
                FormattedDate = "Yesterday";
            }
            else if (Date.Date > DateTime.Today.AddDays(-6))
            {
                // *** Show the Day of the week
                FormattedDate = Date.ToString("dddd").ToString();
            }
            else
            {
                FormattedDate = Date.ToString("MMMM dd, yyyy");
            }

            //append the time portion to the output
            FormattedDate += " @ " + Date.ToString("t").ToLower();
            return FormattedDate;
        }

        /// <summary>
        /// Tos the java time from date time.
        /// </summary>
        /// <returns>The java time from date time.</returns>
        /// <param name="dateTime">Date time.</param>
        public static long ToJavaTimeFromDateTime(this DateTime dateTime)
        {
            DateTime startDate = new System.DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            long totalMillisecs = (long)(dateTime.ToUniversalTime().Subtract(startDate)).TotalMilliseconds;
            return totalMillisecs;
        }

        /// <summary>
        /// Tos the unix time.
        /// </summary>
        /// <returns>The unix time.</returns>
        /// <param name="date">Date.</param>
        public static long ToUnixTime(this DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)((date - epoch).TotalMilliseconds);
        }
    }
}

