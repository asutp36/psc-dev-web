using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace MobileAppWasteSender
{
    class Notification
    {
        private class Message
        {
            public string phone { get; set; }
            public string body { get; set; }

            public string Create(string phone, string body)
            {
                this.phone = phone;
                this.body = body;

                return JsonConvert.SerializeObject(this);
            }
        }
        public static async void SendCritical(Exception e)
        {
            Message m = new Message();
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://cwmon.ru/notify/");
            httpClient.DefaultRequestHeaders.Add(
                HeaderNames.Accept, "application/json");
            var data = new StringContent(m.Create("79507744415", $"Сервис отправки окончания мойки в мобильное приложение упал. {e.Message}"), Encoding.UTF8, Application.Json);

            await httpClient.PostAsync("api/notify/message-dev", data);

            //return httpResponseMessage.EnsureSuccessStatusCode();
        }
    }
}
