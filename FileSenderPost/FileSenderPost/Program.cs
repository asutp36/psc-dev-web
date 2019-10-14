using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace FileSenderPost
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.InitLogger();

            if(args.Length == 0)
            {
                Logger.Log.Error("No arguments");
            }
            else
            {
                foreach(string filename in args)
                {
                    PostRequestFile(ConfigurationSettings.AppSettings["filesDirectory"] + filename);
                    Logger.Log.Debug("Sent file: " + ConfigurationSettings.AppSettings["filesDirectory"] + filename);
                }
            }

            string result = PostRequestFile(@"D:\Programs\Работа\FileSenderPost\m8-08.10.2019.xml");
            Console.WriteLine(result);
            Console.ReadKey();
        }

        static string PostRequestFile(string path)
        {           
            string result;

            WebClient client = new WebClient();
            var response = client.UploadFile(ConfigurationSettings.AppSettings["serverUrl"], path);

            result = System.Text.Encoding.ASCII.GetString(response);

            return result;
        }
    }
}
