using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReportNotificationWhattsapp.Models;
using ReportNotificationWhattsapp.Supplies;
using Newtonsoft.Json;

namespace ReportNotificationWhattsapp
{
    class Program
    {
        static void Main(string[] args)
        {
            ModelDb model = new ModelDb();

            SqlParameter wash = new SqlParameter("@wash", System.Data.SqlDbType.NVarChar);
            wash.Value = "М13";
            SqlParameter date = new SqlParameter("@date", System.Data.SqlDbType.DateTime);
            date.Value = DateTime.Now.AddDays(-1).Date;

            List<GetDayReportIncrease_Result> result = model.Database.SqlQuery<GetDayReportIncrease_Result>("GetDayReportIncrease @wash, @date", wash, date).ToList();

            List<string> phones = new List<string>();
            phones.Add("79050493285-1571938869@g.us");
            phones.Add("79202131085@c.us");

            foreach(string chat in phones)
            {
                MessageToSend message = new MessageToSend();
                message.body = result[0].msg;
                message.chatId = chat;

                string json = JsonConvert.SerializeObject(message);
                ResponseSendMessage response = WhattsAppSender.SendMessage(json);

                if (response.sent)
                {
                    Console.WriteLine("ok");
                    Logger.Log.Debug("Main: Сообщение отправлено: " + Environment.NewLine + json + Environment.NewLine);
                }
                else
                {
                    Console.WriteLine("!ok");
                    Logger.Log.Error("Main: Сообщение не отправлено: " + Environment.NewLine + response.message + Environment.NewLine + json + Environment.NewLine);
                }
            }

            Console.ReadLine();
        }
    }
}
