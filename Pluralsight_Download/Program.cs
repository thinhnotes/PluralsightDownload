using System;
using Pluralsight_Download.Entity;
using THttpWebRequest.Utility;

namespace PluralSight_Download
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 3 || args.Length == 4)
            {
                ConfigValue.UserName = args[0];
                ConfigValue.Password = args[1];
                var pluralSightClient = new PluralSightClient();
                var hasLogin = pluralSightClient.Login(ConfigValue.UserName, ConfigValue.Password);
                if (!hasLogin)
                    throw new Exception("The User Name Or Password Invalid!");
                var s = pluralSightClient.GetJson(args[2]);
                Course deserializeJsonAs = s.DeserializeJsonAs<Course>();

                string downloadFolder = "Download";
                if (args.Length == 4)
                {
                    downloadFolder = args[3];
                }
                pluralSightClient.DownloadAll(deserializeJsonAs, downloadFolder);
                Console.Beep();
            }
        }
    }
}
