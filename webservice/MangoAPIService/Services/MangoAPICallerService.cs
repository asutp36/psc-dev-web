using MangoAPIService.Helpers;
using MangoAPIService.Models;
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
        private HttpSender _httpSender;
        private readonly string _apiKey;
        private readonly string _salt;
         
        public MangoAPICallerService(string baseUrl, string apiKey, string salt)
        {
            _httpSender = new HttpSender(baseUrl);
            _apiKey = apiKey;
            _salt = salt;
        }

        public async void CallHangup(string call_id)
        {
            MangoAPICallHangup hangup = new MangoAPICallHangup() 
            {
                call_id = call_id,
                command_id = DateTime.Now.ToString("yyyyMMddHHmmss")
            };

            string json = JsonConvert.SerializeObject(hangup);
            string signature = Hasher.GetHash(HashAlgorithm.Create("SHA256"), _apiKey + json + _salt);

            Dictionary<string, string> toSend = new Dictionary<string, string>();
            toSend.Add("vpbx_api_key", _apiKey);
            toSend.Add("json", json);
            toSend.Add("sign", signature);

            HttpResponseMessage response = await _httpSender.PostAsync("commands/call/hangup", toSend);
            response.EnsureSuccessStatusCode();
        }
    }
}
