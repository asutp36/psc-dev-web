using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportNotificationWhattsapp.Models;
using ReportNotificationWhattsapp.Supplies;
using Newtonsoft.Json;
using System.Configuration;

namespace ReportNotificationWhattsapp
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
                String recipientsGroup = args[0];
                // код мойки
                //String washCode = "М" + (int.Parse(args[0]) / 10).ToString();

                // Если определены все аргументы
                if (!String.IsNullOrEmpty(recipientsGroup))
                {
                    // Определить получателей
                    String smsRecipients = ConfigurationManager.AppSettings["waRecipients" + recipientsGroup];
                    // Определить код мойки 
                    String washCode = ConfigurationManager.AppSettings["washCode" + recipientsGroup];
                    // Определить хранимую процедуру
                    String storedProcedure = ConfigurationManager.AppSettings["storedProcedure" + recipientsGroup];

                    if (String.IsNullOrEmpty(smsRecipients))
                    {
                        // не найден список получателей
                        Logger.Log.Error(String.Format("{0} Не определены получатели group = {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), recipientsGroup));
                    }

                    if (String.IsNullOrEmpty(washCode))
                    {
                        // не найден список получателей
                        Logger.Log.Error(String.Format("{0} Не определена мойка = {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), recipientsGroup));
                    }

                    if (String.IsNullOrEmpty(storedProcedure))
                    {
                        // не найден список получателей
                        Logger.Log.Error(String.Format("{0} Не определена хранимая процедура = {1}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"), recipientsGroup));
                    }

                    String[] recipients = smsRecipients.Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<GetDayReportIncrease_Result> mtext = new List<GetDayReportIncrease_Result>();

                    using (ModelDb model = new ModelDb())
                    {
                        try
                        {
                            model.Database.Connection.Open();
                            SqlParameter wash = new SqlParameter("@wash", System.Data.SqlDbType.NVarChar);
                            wash.Value = washCode;
                            //wash.Value = "R48-M1";
                            SqlParameter date = new SqlParameter("@date", System.Data.SqlDbType.DateTime);
                            date.Value = DateTime.Now.AddDays(-1).Date;

                            mtext = model.Database.SqlQuery<GetDayReportIncrease_Result>($"{storedProcedure} @wash, @date", wash, date).ToList();
                            model.Database.Connection.Close();
                        }
                        catch (Exception e)
                        {
                            if (model.Database.Connection.State == System.Data.ConnectionState.Open)
                                model.Database.Connection.Close();
                            Logger.Log.Error("Ошибка при обращении к бд: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        }
                    }


                    string body = mtext[0].msg.Replace(" П", "\nП").Replace(": ", ":\n").Insert(mtext[0].msg.IndexOf("за ") + 14, "\n");

                    foreach (string chat in recipients)
                    {
                        MessageToSend message = new MessageToSend();
                        message.body = body;
                        message.chatId = chat;

                        string json = JsonConvert.SerializeObject(message);
                        TelegramResponse response = TelegramSender.SendMessage(json);

                        if (response.ok)
                        {
                            Console.WriteLine("ok");
                            Logger.Log.Debug("Main: Сообщение отправлено: " + json + Environment.NewLine);
                        }
                        else
                        {
                            Console.WriteLine("!ok");
                            Logger.Log.Error("Main: Сообщение не отправлено: " + json + Environment.NewLine);
                        }
                    }
                }
            }
            catch (Exception e)
            {

                Logger.Log.Error(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
            }
        }
    }
}
