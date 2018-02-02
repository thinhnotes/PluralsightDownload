﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pluralsight_Download.Entity;
using THttpWebRequest;
using THttpWebRequest.Utility;
using System.Threading;
using System.Threading.Tasks;

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

            return CookieCollection["PsJwt-production"] != null;
        }


        public string DownLoad(string urldownload)
        {
            var uri = new Uri(urldownload);
            string url = "https://app.pluralsight.com/video/clips/viewclip";
            var moduleName = HttpUtility.ParseQueryString(uri.Query).Get("name");
            object data = new
            {
                author = HttpUtility.ParseQueryString(uri.Query).Get("author"),
                moduleName = moduleName,
                courseName = HttpUtility.ParseQueryString(uri.Query).Get("course"),
                clipIndex = int.Parse(HttpUtility.ParseQueryString(uri.Query).Get("clip") ?? "0"),
                mediaType = ConfigValue.FileType,
                quality = ConfigValue.Quality,
                includeCaptions = ConfigValue.Cap,
                locale = ConfigValue.Localize
            };
            Referer = urldownload;
            var downLoad = Post(url, data.ToJsonString(), TypeRequest.Json);
            var jObject = JsonConvert.DeserializeObject<JObject>(downLoad);
            if(jObject!=null && jObject["urls"]!=null)
            {
                var urls = JsonConvert.DeserializeObject<List<LinkDownload>>(jObject["urls"].ToString());
                return urls.FirstOrDefault()?.Url;
            }
            throw new Exception($"Not find any url download for module {moduleName}");
        }

        public void DownLoadFile(string url, string path, string fileName)
        {
            using (var webClient = new WebClient())
            {
                var fileSizeFromUrl = GetFileSizeFromUrl(webClient, url);

                CreateIfNotExitDirectory(path);
                var combine = PathCombine(path, CleanFileName(fileName));

                if (!CheckFileIsDonloaded(combine, fileSizeFromUrl, url))
                {
                    var progress = new ProgressBar();

                    webClient.DownloadProgressChanged += (sender, args) =>
                    {
                        progress.Report((double)args.BytesReceived / args.TotalBytesToReceive);
                    };

                    webClient.DownloadFileCompleted += (sender, args) =>
                    {
                        progress.Dispose();
                    };

                    webClient.DownloadFileTaskAsync(new Uri(url), combine).Wait();
                }
            }
        }

        public bool CheckFileIsDonloaded(string fileLocation, Int64 bytesTotal, string urlFile)
        {
            if (!File.Exists(fileLocation)) return false;
            
            if (new FileInfo(fileLocation).Length == bytesTotal)
                return true;
            return false;
        }

        public Int64 GetFileSizeFromUrl(WebClient wc, string urlFile)
        {
            wc.OpenRead(urlFile);
            return Convert.ToInt64(wc.ResponseHeaders["Content-Length"]);
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