using MangoAPIService.Helpers;
using MangoAPIService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MangoAPIService.Services
{
    public class MangoAPICallerService : IMangoAPICaller
    {
        private readonly ILogger _logger;
        private HttpSender _httpSender;
        private readonly string _apiKey;
        private readonly string _salt;
         
        public MangoAPICallerService(ILogger logger, string baseUrl, string apiKey, string salt)
        {
            _logger = logger;
            _httpSender = new HttpSender(baseUrl);
            _apiKey = apiKey;
            _salt = salt;
        }

        public async void CallHangupAsync(string call_id)
        {
            // манго апи нужно отправить идентификатор звонка (call_id) и любой идентификатор команды (им неважно какой он)
            MangoAPICallHangup hangup = new MangoAPICallHangup() 
            {
                call_id = call_id,
                command_id = DateTime.Now.ToString("yyyyMMddHHmmss")
            };

            string json = JsonConvert.SerializeObject(hangup);

            // генерируется подпись
            string signature = Hasher.GetHash(HashAlgorithm.Create("SHA256"), _apiKey + json + _salt + "qwerty");

            // данные нужно отправлять в виде application/x-www-form-urlencoded, то их нужно предстваить в виде словаря
            Dictionary<string, string> toSend = new Dictionary<string, string>();
            toSend.Add("vpbx_api_key", _apiKey);
            toSend.Add("json", json);
            toSend.Add("sign", signature);

            HttpResponseMessage response = await _httpSender.PostFormUrlEncodedAsync("commands/call/hangup", toSend);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException e)
            {
                string content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"| MangoAPICallerService.CallHangupAsync | От MangoAPI ответ неудачный. StatusCode: {response.StatusCode}, Content: {content}");
            }
        }
    }
}
