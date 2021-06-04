using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Pluralsight_Download.Entity;
using THttpWebRequest.Utility;

namespace PluralSight_Download
{
    static class Program
    {
        private static readonly string commandCharacter = "--";
        private static readonly string defaultFolderDownload = "Download";

        static void Main(string[] args)
        {
            if (DisplayConsole(Environment.GetCommandLineArgs().ToList(), out DownloadParameter downloadParameter) == 0)
            {
                var pluralSightClient = new PluralSightClient();
                var hasLogin = pluralSightClient.Login(downloadParameter.UserName, downloadParameter.Password);
                if (!hasLogin)
                    Console.WriteLine("The User Name Or Password Invalid!");
                var jsonCourse = pluralSightClient.GetJson(downloadParameter.Link);
                Course deserializeJsonAs = jsonCourse.DeserializeJsonAs<Course>();

                pluralSightClient.DownloadAll(deserializeJsonAs, downloadParameter.DownloadFolder);
                Console.WriteLine("Download Complete!");
                Console.Beep();
            }
        }

        public static DownloadParameter ParseDownloadParameter(List<string> strings)
        {
            if (strings.Any(x => x.StartsWith(commandCharacter)))
            {
                Dictionary<string, string> commands = new Dictionary<string, string>();
                for (int i = 0; i < strings.Count; i++)
                {
                    if (strings[i].StartsWith(commandCharacter) && !string.IsNullOrWhiteSpace(strings[i + 1]) && !strings[i + 1].StartsWith(commandCharacter))
                    {
                        commands.Add(strings[i].Replace(commandCharacter, ""), strings[i + 1]);
                    }
                }
                return ParseDownloadParameterCommandList(commands);
            }
            else
                return ParseDownloadParameterFromNominalize(strings);
        }

        public static DownloadParameter ParseDownloadParameterFromNominalize(List<string> strings)
        {
            var result = new DownloadParameter()
            {
                DownloadFolder = defaultFolderDownload
            };
            if (strings.Count >= 4)
            {
                result.UserName = strings[1];
                result.Password = strings[2];
                result.Link = strings[3];
            }
            if (strings.Count >= 5)
            {
                result.DownloadFolder = strings[4];
            }
            return result;
        }

        public static DownloadParameter ParseDownloadParameterCommandList(Dictionary<string, string> command)
        {
            var result = new DownloadParameter()
            {
                DownloadFolder = defaultFolderDownload
            };

            foreach (var commandKey in command.Keys)
            {
                var commandProperty = typeof(DownloadParameter).GetProperties().FirstOrDefault(p => p.Name.Equals(commandKey, StringComparison.OrdinalIgnoreCase));
                if (commandProperty != null)
                {
                    commandProperty.SetValue(result, command[commandKey]);
                }
            }

            return result;
        }

        public static int DisplayConsole(List<string> commandLinesStrings, out DownloadParameter parameter)
        {
            if (commandLinesStrings.Count == 1 || commandLinesStrings.Any(x => x.StartsWith(commandCharacter + "help")))
            {
                parameter = new DownloadParameter();
                DisplayHelpConsole();
                return -1;
            }
            parameter = ParseDownloadParameter(commandLinesStrings);
            if (string.IsNullOrWhiteSpace(parameter.UserName))
            {
                parameter = new DownloadParameter();
                Console.WriteLine("The UserName can not be empty!");
                return -1;
            }
            if (string.IsNullOrWhiteSpace(parameter.Password))
            {
                parameter = new DownloadParameter();
                Console.WriteLine("The PassWord can not be empty!");
                return -1;
            }
            if (string.IsNullOrWhiteSpace(parameter.Link))
            {
                parameter = new DownloadParameter();
                Console.WriteLine("The Link Download can not be empty!");
                return -1;
            }
            return 0;
        }

        public static void DisplayHelpConsole()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            string name = Path.GetFileName(codeBase);
            Console.WriteLine($"you can used download by order {name} [Username] [Password] [Link] [DownloadFolder]");
            Console.WriteLine("--UserName [UserName] for set user name param [required]");
            Console.WriteLine("--PassWord [PassWord] for set user name param [required]");
            Console.WriteLine("--Link [Link] for set user name param [required]");
            Console.WriteLine("--DownloadFolder [DownloadFolder] for set user name param [optional]");
            Console.WriteLine("--Help");
        }
    }
    public class DownloadParameter
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Link { get; set; }
        public string DownloadFolder { get; set; }
    }
}
