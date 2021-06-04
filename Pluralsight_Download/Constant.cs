using System;

namespace Pluralsight_Download
{
    static class Constant
    {
        public static readonly string Host = "pluralsight.com";
        public static readonly string RequestVerificationToken = "__RequestVerificationToken";
        public static readonly Uri SiteApp = new Uri($"https://app.{Host}");
    }
}
