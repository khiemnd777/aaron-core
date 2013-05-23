using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Aaron.Core;
using Aaron.Core.Domain.Accounts;
using Aaron.Core.Services.Accounts;
using Aaron.Core.Services.Configuration;

namespace Aaron.Core.Services.Helpers
{
    /// <summary>
    /// Represents a datetime helper
    /// </summary>
    public partial class DateTimeHelper : IDateTimeHelper
    {
        private readonly ICurrentActivity _currentActivity;
        private readonly IAccountService _accountService;
        private readonly ISettingService _settingService;

        public DateTimeHelper(ICurrentActivity currentActivity,
            IAccountService accountService,
            ISettingService settingService)
        {
            this._currentActivity = currentActivity;
            this._accountService = accountService;
            this._settingService = settingService;
        }

        /// <summary>
        /// Retrieves a System.TimeZoneInfo object from the registry based on its identifier.
        /// </summary>
        /// <param name="id">The time zone identifier, which corresponds to the System.TimeZoneInfo.Id property.</param>
        /// <returns>A System.TimeZoneInfo object whose identifier is the value of the id parameter.</returns>
        public virtual TimeZoneInfo FindTimeZoneById(string id)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }

        /// <summary>
        /// Returns a sorted collection of all the time zones
        /// </summary>
        /// <returns>A read-only collection of System.TimeZoneInfo objects.</returns>
        public virtual ReadOnlyCollection<TimeZoneInfo> GetSystemTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones();
        }

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in account time zone.</returns>
        public virtual DateTime ConvertToUserTime(DateTime dt)
        {
            return ConvertToUserTime(dt, dt.Kind);
        }

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <param name="sourceDateTimeKind">The source datetimekind</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in account time zone.</returns>
        public virtual DateTime ConvertToUserTime(DateTime dt, DateTimeKind sourceDateTimeKind)
        {
            dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
            var currentUserTimeZoneInfo = this.CurrentTimeZone;
            return TimeZoneInfo.ConvertTime(dt, currentUserTimeZoneInfo);
        }

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in account time zone.</returns>
        public virtual DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone)
        {
            var currentUserTimeZoneInfo = this.CurrentTimeZone;
            return ConvertToUserTime(dt, sourceTimeZone, currentUserTimeZoneInfo);
        }

        /// <summary>
        /// Converts the date and time to current user date and time
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <param name="destinationTimeZone">The time zone to convert dateTime to.</param>
        /// <returns>A DateTime value that represents time that corresponds to the dateTime parameter in account time zone.</returns>
        public virtual DateTime ConvertToUserTime(DateTime dt, TimeZoneInfo sourceTimeZone, TimeZoneInfo destinationTimeZone)
        {
            return TimeZoneInfo.ConvertTime(dt, sourceTimeZone, destinationTimeZone);
        }

        /// <summary>
        /// Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
        public virtual DateTime ConvertToUtcTime(DateTime dt)
        {
            return ConvertToUtcTime(dt, dt.Kind);
        }

        /// <summary>
        /// Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        /// <param name="dt">The date and time (respesents local system time or UTC time) to convert.</param>
        /// <param name="sourceDateTimeKind">The source datetimekind</param>
        /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
        public virtual DateTime ConvertToUtcTime(DateTime dt, DateTimeKind sourceDateTimeKind)
        {
            dt = DateTime.SpecifyKind(dt, sourceDateTimeKind);
            return TimeZoneInfo.ConvertTimeToUtc(dt);
        }

        /// <summary>
        /// Converts the date and time to Coordinated Universal Time (UTC)
        /// </summary>
        /// <param name="dt">The date and time to convert.</param>
        /// <param name="sourceTimeZone">The time zone of dateTime.</param>
        /// <returns>A DateTime value that represents the Coordinated Universal Time (UTC) that corresponds to the dateTime parameter. The DateTime value's Kind property is always set to DateTimeKind.Utc.</returns>
        public virtual DateTime ConvertToUtcTime(DateTime dt, TimeZoneInfo sourceTimeZone)
        {
            if (sourceTimeZone.IsInvalidTime(dt))
            {
                //could not convert
                return dt;
            }
            else
            {
                return TimeZoneInfo.ConvertTimeToUtc(dt, sourceTimeZone);
            }
        }

        /// <summary>
        /// Gets a account time zone
        /// </summary>
        /// <param name="account">Account</param>
        /// <returns>Account time zone; if account is null, then default store time zone</returns>
        public virtual TimeZoneInfo GetAccountTimeZone(Account account)
        {
            //registered user
            TimeZoneInfo timeZoneInfo = this.DefaultTimeZone;

            return timeZoneInfo;
        }

        /// <summary>
        /// Gets or sets a default store time zone
        /// </summary>
        public virtual TimeZoneInfo DefaultTimeZone
        {
            get
            {
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
                return timeZoneInfo;
            }
        }

        /// <summary>
        /// Gets or sets the current user time zone
        /// </summary>
        public virtual TimeZoneInfo CurrentTimeZone
        {
            get
            {
                return GetAccountTimeZone(_currentActivity.CurrentAccount);
            }
        }

    }
}