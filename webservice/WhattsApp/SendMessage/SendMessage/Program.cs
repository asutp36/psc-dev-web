using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SendMessage
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.InitLogger();

            try
            {
                string toLog = "";
                for (int i = 0; i < args.Length; i++)
                {
                    toLog += ' ' + args[i];
                }
                  

                Logger.Log.Debug("Запуск с параметрами:" + toLog + ". Количество: " + args.Length);

                // номер списка получателей
                String recipientsGroup = "";
                // номер мойки
                Int16 washNumber = -1;
                // номер поста
                Int16 postNumber = -1;
                // сообщение
                String mtext = "";

                if (args.Length >= 3)
                {
                    recipientsGroup = args[0];
                    Int16.TryParse(args[1], out washNumber);
                    Int16.TryParse(args[2], out postNumber);

                    for (int i = 3; i < args.Length; i++)
                        mtext += String.Format("{0}", args[i].Replace("\\n", Environment.NewLine));

                    mtext = mtext.TrimStart(' ');
                    mtext = mtext.TrimEnd(' ');
                }

                // Если определены все аргументы
                if (!String.IsNullOrEmpty(recipientsGroup) && postNumber > -1 && !String.IsNullOrEmpty(mtext))
                {
                    // Определить получателей
                    String smsRecipients = ConfigurationManager.AppSettings["smsRecipients" + recipientsGroup];
                    if (String.IsNullOrEmpty(smsRecipients))
                    {
                        // не найден список получателей
                        Logger.Log.Error(String.Format("{0} Не определены получатели group = {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), recipientsGroup));
                    }

                    String[] recipients = smsRecipients.Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

                    // Определить пост
                    String msgPost = "";
                    // Определить мойку
                    String msgWash = "";

                    if (washNumber > 0)
                    {
                        msgWash = String.Format("М{0}", washNumber);
                    }

                    if (postNumber > 0)
                    {
                        msgPost = String.Format("Пост {0}:", postNumber);
                    }

                    // Сформировать SMS сообщение
                    String text = "";
                    text = String.Format("{0} {1}\n{2}", msgWash, msgPost, mtext);
                    text = text.TrimStart(' ');
                    text = text.TrimEnd(' ');

                    String sender = ConfigurationManager.AppSettings["smsSender" + recipientsGroup];

                    foreach (String recipient in recipients)
                    {
                        Message message = new Message(recipient, text);

                        string result = PostRequest(JsonConvert.SerializeObject(message));

                        ResponseSendMessage response = JsonConvert.DeserializeObject<ResponseSendMessage>(result);

                        if (!response.sent)
                        {
                            Logger.Log.Error(response.message + Environment.NewLine);
                        }
                        else
                        {
                            Logger.Log.Debug(String.Format("{0} : {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), response.message) + Environment.NewLine);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log.Error(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
            }

        }

        static string PostRequest(string json)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://eu33.chat-api.com/instance27633/sendMessage?token=0qgid5wjmhb8vw7d");
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://ptsv2.com/t/rq63q-1572107969/post");
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:56981/api/notify/sendmessage");
            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";

            byte[] postBytes = Encoding.UTF8.GetBytes(json);

            request.ContentType = "application/json; charset=UTF-8";
            request.Accept = "application/json";
            request.ContentLength = postBytes.Length;

            Stream requestStream = request.GetRequestStream();

            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result;
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }

            return result;
        }

        static void CheckAccountStatus()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.chat-api.com/instance27633/status?token=0qgid5wjmhb8vw7d");
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "GET";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string result;
            using (StreamReader rdr = new StreamReader(response.GetResponseStream()))
            {
                result = rdr.ReadToEnd();
            }
        }
    }
}
