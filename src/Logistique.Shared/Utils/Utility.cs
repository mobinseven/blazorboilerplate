﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BlazorBoilerplate.Shared
{
    public static class Utility
    {
        private static Dictionary<string, string> PersianLettersDictionary = new Dictionary<string, string>
        {
            ["۰"] = "0",
            ["۱"] = "1",
            ["۲"] = "2",
            ["۳"] = "3",
            ["۴"] = "4",
            ["۵"] = "5",
            ["۶"] = "6",
            ["۷"] = "7",
            ["۸"] = "8",
            ["۹"] = "9"
        };

        private static Dictionary<string, string> ArabicLettersDictionary = new Dictionary<string, string>
        {
            ["٠"] = "0",
            ["١"] = "1",
            ["٢"] = "2",
            ["٣"] = "3",
            ["٤"] = "4",
            ["٥"] = "5",
            ["٦"] = "6",
            ["٧"] = "7",
            ["٨"] = "8",
            ["٩"] = "9"
        };

        public static string PersianToEnglish(this string persianStr)
        {
            if (string.IsNullOrEmpty(persianStr)) return persianStr;
            return PersianLettersDictionary.Aggregate(persianStr.ArabicToEnglish(), (current, item) =>
                         current.Replace(item.Key, item.Value));
        }

        public static string ArabicToEnglish(this string persianStr)
        {
            if (string.IsNullOrEmpty(persianStr)) return persianStr;
            return ArabicLettersDictionary.Aggregate(persianStr, (current, item) =>
                         current.Replace(item.Key, item.Value));
        }

        public static bool IsPhoneNumber(this string PhoneNumber)
        {
            if (string.IsNullOrEmpty(PhoneNumber)) return false;
            var r = new System.Text.RegularExpressions.Regex(@"^09[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]$");
            return r.IsMatch(PhoneNumber);
        }

        private static readonly PersianCalendar pc = new PersianCalendar();

        public static string ConvertToFormattedPersianTime(DateTime dt)
        {
            StringBuilder sb = new StringBuilder();
            if (pc.MinSupportedDateTime < dt)
                return sb.AppendFormat("{0}:{1} | {2}/{3}/{4}",
                pc.GetHour(dt), pc.GetMinute(dt), pc.GetDayOfMonth(dt), pc.GetMonth(dt), pc.GetYear(dt)).ToString();
            else
                return sb.AppendFormat("{0}:{1} | {0}/{1}/{2}", dt.Hour, dt.Minute, dt.Day, dt.Month, dt.Year).ToString();
        }

        public static string ConvertToFormattedPersianDate(DateTime dt)
        {
            StringBuilder sb = new StringBuilder();
            if (pc.MinSupportedDateTime < dt)
                return sb.AppendFormat("{0}/{1}/{2}", pc.GetDayOfMonth(dt), pc.GetMonth(dt), pc.GetYear(dt)).ToString();
            else
                return sb.AppendFormat("{0}/{1}/{2}", dt.Day, dt.Month, dt.Year).ToString();
        }

        public static DateTime ConvertToPersian(DateTime dt)
        {
            return new DateTime(pc.GetYear(dt), pc.GetMonth(dt), pc.GetDayOfMonth(dt), pc.GetHour(dt), pc.GetMinute(dt), 0);
        }

        public static readonly DateTime UnixEpoch =
    new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long UnixTimestampMilliseconds(DateTime dt)
        {
            return (long)(dt - UnixEpoch).TotalMilliseconds;
        }

        public static long UnixTimestampSeconds(DateTime dt)
        {
            return (long)(dt - UnixEpoch).TotalSeconds;
        }

        public static DateTime FromUnixTime(long unixTime)
        {
            return UnixEpoch.AddSeconds(unixTime);
        }
    }
}