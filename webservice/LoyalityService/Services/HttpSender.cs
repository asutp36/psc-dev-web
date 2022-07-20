using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DiscountService.Services
{
    public class HttpSender
    {
        private HttpClient _httpClient;

        public HttpSender(string baseAddress)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseAddress);
        }

        public async Task<HttpResponseMessage> PostJsonAsync(string url, string json)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url);

            if(!string.IsNullOrEmpty(json))
            {
                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            HttpResponseMessage result = await _httpClient.SendAsync(requestMessage);

            return result;
        }

        
    }
}
