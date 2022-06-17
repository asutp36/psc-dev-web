using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MangoAPIService.Helpers
{
    public class HttpSender
    {
        private HttpClient _httpClient;

        public HttpSender(string baseAddress)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseAddress);
        }

        public async Task<HttpResponseMessage> Post(string url, string content)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            if(content != null)
            {
                requestMessage.Content = new StringContent(content, Encoding.UTF8, Application.Json);
            }

            HttpResponseMessage result = await _httpClient.SendAsync(requestMessage);

            return result;
        }
    }
}
