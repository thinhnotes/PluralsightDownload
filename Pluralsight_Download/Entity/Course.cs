using System;
using System.Collections.Generic;

namespace Pluralsight_Download.Entity
{
    public class ConfigValue
    {
        public static string PathDirectory = "D:\\test";
        public static string FileType = "mp4";
        public static string Quality = "1024x768";
        public static bool Cap = false;
        public static string Localize = "en";
        public static string UserName { get; set; }
        public static string Password { get; set; }
    }

    public class Course
    {
        public string Id { get; set; }
        public DateTime PublishedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public Level Level { get; set; }
        public string Duration { get; set; }
        public string PlayerUrl { get; set; }
        public int PopularityScore { get; set; }
        public List<Authors> Authors { get; set; }
        public List<Module> Modules { get; set; }
    }

    public class Authors
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class Module
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public string PlayerUrl { get; set; }
        public List<Clip> Clips { get; set; }
    }

    public class Clip
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public string PlayerUrl { get; set; }
    }

    public enum Level
    {
        Intermediate,
        Beginner,
        Advanced
    }

    public class LinkDownload
    {
        public string CDN { get; set; }
        public string Source { get; set; }
        public int Rank { get; set; }
        public string Url { get; set; }
    }
}
