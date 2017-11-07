using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Pluralsight_Download.Entity;
using THttpWebRequest;
using THttpWebRequest.Utility;

namespace PluralSight_Download
{
    public class PluralSightClient : TWebRequest
    {
        public string GetJson(string url)
        {
            return Get(url.Replace("/library/", "/learner/content/").Replace("/table-of-contents", ""));
        }

        public bool Login(string user, string pass)
        {
            var url = "https://app.pluralsight.com/id/";
            var data = $"Username={HttpUtility.HtmlEncode(user)}&Password={HttpUtility.HtmlEncode(pass)}";
            Referer = url;
            Post(url, data);
            Get(
                "https://app.pluralsight.com/data/signin/token?redirectTo=https%3a%2f%2fapp.pluralsight.com%2flibrary%2f");

            return CookieCollection["PsJwt-production"] != null;
        }


        public string DownLoad(string urldownload)
        {
            var uri = new Uri(urldownload);
            string url = "http://app.pluralsight.com/training/Player/ViewClip";
            object data = new
            {
                a = HttpUtility.ParseQueryString(uri.Query).Get("author"),
                m = HttpUtility.ParseQueryString(uri.Query).Get("name"),
                course = HttpUtility.ParseQueryString(uri.Query).Get("course"),
                cn = HttpUtility.ParseQueryString(uri.Query).Get("clip"),
                mt = ConfigValue.FileType,
                q = ConfigValue.Quality,
                cap = ConfigValue.Cap,
                lc = ConfigValue.Localize
            };
            return Post(url, data.ToJsonString(), TypeRequest.Json);
        }

        public void DownLoadFile(string url, string path, string fileName)
        {
            using (var webClient = new WebClient())
            {
                CreateIfNotExitDirectory(path);
                var combine = PathCombine(path, CleanFileName(fileName));
                webClient.DownloadFile(new Uri(url), combine);
            }
        }

        public void DownloadAll(Course course, string path)
        {
            path = PathCombine(path, CleanFileName(course.Title));
            CreateIfNotExitDirectory(path);
            int i = 0;
            foreach (var module in course.Modules)
            {
                i++;
                var modulePath = PathCombine(path, $"{i}. {CleanFileName(module.Title)}");
                CreateIfNotExitDirectory(modulePath);
                int j = 0;
                foreach (var clip in module.Clips)
                {
                    j++;
                    Console.WriteLine(clip.Title);
                    if (!clip.PlayerUrl.StartsWith("http"))
                    {
                        clip.PlayerUrl = "https://app.pluralsight.com" + clip.PlayerUrl;
                    }
                    var downLoadLink = DownLoad(clip.PlayerUrl);
                    DownLoadFile(downLoadLink, modulePath, $"{j}. {clip.Title}.{ConfigValue.FileType}");
                }
            }
        }

        string PathCombine(string path1, string path2)
        {
            return Path.Combine(CleanFilePath(path1), CleanFilePath(path2));
        }

        void CreateIfNotExitDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(CleanFilePath(path));
            }
        }

        string CleanFilePath(string filePath)
        {
            return Path.GetInvalidPathChars().Aggregate(filePath, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

        string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}