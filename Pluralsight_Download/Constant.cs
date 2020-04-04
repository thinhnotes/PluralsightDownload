using System;

namespace Pluralsight_Download
{
    static class Constant
    {
        public static string Host = "pluralsight.com";
        public static string RequestVerificationToken = "__RequestVerificationToken";
        public static Uri SiteApp = new Uri($"https://app.{Host}");
    }
}
