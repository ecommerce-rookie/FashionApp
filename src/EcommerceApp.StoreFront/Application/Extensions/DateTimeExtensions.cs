namespace StoreFront.Application.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
		/// Gets the current date and time in the Vietnam time zone.
		/// </summary>
		/// <returns>The current date and time in Vietnam.</returns>
		public static DateTime GetDateTimeRegion(this DateTime _)
        {
            // Get the current server time in UTC.
            DateTime serverTime = DateTime.UtcNow;

            // Find the time zone information for Vietnam.
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Convert the server time to Vietnam time.
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(serverTime, vietnamTimeZone);

            return vietnamTime;
        }

        public static string GetValueDateTimeRegion(this DateTime _)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);

            return timeNow.ToString("yyyyMMddHHmmss");
        }

        /// <summary>
        /// Gets the current date in the Vietnam time zone formatted as "dd/MM/yyyy".
        /// </summary>
        /// <returns>The current date in Vietnam as a string.</returns>
        public static string GetDateRegion(this DateTime _)
        {
            // Get the current server time in UTC.
            DateTime serverTime = DateTime.UtcNow;

            // Find the time zone information for Vietnam.
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            // Convert the server time to Vietnam time.
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(serverTime, vietnamTimeZone);

            // Format the date as "dd/MM/yyyy" and return it as a string.
            return vietnamTime.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Converts a DateTime object to a string formatted as "dd/MM/yyyy".
        /// </summary>
        /// <param name="dateTime">The DateTime object to format.</param>
        /// <returns>The date part of the DateTime object as a string.</returns>
        public static string ToOnlyDate(this DateTime dateTime)
        {
            return dateTime.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Gets a string representing the time range between two DateTime objects, formatted as "HH:mm:tt - HH:mm:tt".
        /// </summary>
        /// <param name="startDate">The start of the time range.</param>
        /// <param name="endDate">The end of the time range.</param>
        /// <returns>A string representing the time range.</returns>
        public static string GetTimeRange(DateTime startDate, DateTime endDate)
        {
            return startDate.ToString("HH:mm:tt") + " - " + endDate.ToString("HH:mm:tt");
        }

        /// <summary>
        /// Checks if two DateTime objects represent the same date (ignoring time).
        /// </summary>
        /// <param name="dateTime">The first DateTime object.</param>
        /// <param name="compare">The second DateTime object to compare with.</param>
        /// <returns>True if the dates are the same, otherwise false.</returns>
        public static bool IsTheSameDate(this DateTime dateTime, DateTime compare)
        {
            return dateTime.ToString("dd/MM/yyyy").Equals(compare.ToString("dd/MM/yyyy"));
        }

        /// <summary>
        /// Parse string to DateTime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime? ParseDateTime(this string date)
        {
            try
            {
                return DateTime.Parse(date);
            } catch (System.Exception)
            {
                Console.WriteLine("Date format is not valid");
                return null;
            }
        }

        /// <summary>
        /// Gets a string representing the time since a given DateTime object.      
        /// </summary>
        /// <param name="createdDate"></param>
        /// <returns></returns>
        public static string GetTimeSender(this DateTime createdDate)
        {
            var currentTime = DateTime.UtcNow.GetDateTimeRegion();

            var timeSpan = currentTime - createdDate;

            if (timeSpan.TotalDays > 1)
            {
                return createdDate.ToString("dd/MM/yyyy");
            }

            if (timeSpan.TotalHours > 1)
            {
                return ((int)(timeSpan.TotalHours)).ToString() + "h";
            }

            if (timeSpan.TotalMinutes > 1)
            {
                return ((int)(timeSpan.TotalMinutes)).ToString() + "m";
            }

            return ((int)(timeSpan.TotalSeconds)).ToString() + "s";
        }

    }
}
