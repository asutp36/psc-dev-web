using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using WashPing.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WashPing
{
    class Program
    {
        private static string _projectDirectory;
        private static Ping _pinger;

        private static ILogger _logger;

        private static List<Host> _hosts;
        private static int _retryCount;
        private static int _timeout;
        private static string _notifyUrl;
        private static string _notifyEndpoint;

        private static int _significantFailedPingCount;
        private static string _failedPingCountersFilename;
        private static List<PingCounter> _failedPingCounters;

        private static async Task ConfigureAsync()
        {
            _logger = LogManager.GetCurrentClassLogger();

            _projectDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _logger.Info($"Текущая директория: {_projectDirectory}");

            var builder = new ConfigurationBuilder().SetBasePath(_projectDirectory).AddJsonFile("appsettings.json", optional: false);
            
            IConfiguration config = builder.Build();

            _hosts = config.GetSection("Hosts").Get<List<Host>>();
            _retryCount = int.Parse(config.GetSection("RetryCount").Value);
            _timeout = int.Parse(config.GetSection("Timeout").Value);
            _notifyUrl = config.GetSection("NotifyUrl").Value;
            _notifyEndpoint = config.GetSection("NotifyEndpoint").Value;
            _significantFailedPingCount = int.Parse(config.GetSection("SignificantFailedPingCount").Value);
            _failedPingCountersFilename = config.GetSection("FailedPingCountersFilename").Value;

            await GetFailedPingCountersAsync();

            _pinger = new Ping();
        }

        static async Task Main(string[] args)
        {
            try
            {
                await ConfigureAsync();
                
                foreach (Host host in _hosts)
                {
                    if (host.IsActive)
                    {
                        if (Ping(host.Ip))
                        {
                            _logger.Info($"Мойка {host.Name} пингуется");
                            if(_failedPingCounters.Any(w => w.Name.Equals(host.Name)))
                            {
                                var counterToRemove = _failedPingCounters.Where(w => w.Name.Equals(host.Name)).First();
                                _failedPingCounters.Remove(counterToRemove);
                            }
                        }
                        else
                        {
                            if (_failedPingCounters.Any(w => w.Name.Equals(host.Name)))
                            {
                                var counter = _failedPingCounters.Where(w => w.Name.Equals(host.Name)).First();
                                counter.FailedPingCount++;

                                if(counter.FailedPingCount % _significantFailedPingCount == 0)
                                {
                                    _logger.Info($"По мойке {host.Name} отправляется оповещение");
                                    await SendNotification(host.FailedChatID, host.FailedMessage.Replace("{name}", host.Name));
                                }
                                
                                _logger.Error($"Мойка {host.Name} не пингуется");
                            }
                            else
                            {
                                PingCounter counter = new PingCounter { Name = host.Name, FailedPingCount = 1 };
                                _failedPingCounters.Add(counter);
                            }
                        }
                    }
                }

                UpdateFailedCountersFile();
            }
            catch(Exception e)
            {
                _logger.Error($"Произошла ошибка: {e.GetType()}: {e.Message}" + Environment.NewLine + e.StackTrace);
            }
        }

        private static bool Ping(string ip)
        {
            PingReply reply;
            for(int i = 0; i < _retryCount; i++)
            {
                try
                {
                    reply = _pinger.Send(ip, _timeout);
                    if (reply.Status == IPStatus.Success)
                    {
                        return true;
                    }
                }
                catch(PingException e)
                {
                    if (e.InnerException != null)
                    {
                        _logger.Error($"Ping exception: {e.Message}, inner: {e.InnerException.Message}");
                    }
                    else
                    {
                        _logger.Error($"Ping exception: {e.Message}");
                    }

                    return false;
                }
            }

            return false;
        }

        private static async Task SendNotification(string chatID, string message)
        {
            try
            {
                Message msg = new Message();
                msg.ChatID = chatID;
                msg.Body = message;

                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(_notifyUrl);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                var data = new StringContent(JsonConvert.SerializeObject(msg), Encoding.UTF8, "application/json");
                
                var result = await httpClient.PostAsync(_notifyEndpoint, data);
                result.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException e)
            {
                _logger.Error($"Ошибка при отправке сообщения: {e.Message}");
            }
        }

        private static void UpdateFailedCountersFile()
        {
            File.WriteAllText(_projectDirectory + "/" + _failedPingCountersFilename, JsonConvert.SerializeObject(_failedPingCounters));
        }

        private static async Task GetFailedPingCountersAsync()
        {
            using (StreamReader sr = new StreamReader(_projectDirectory + "/" + _failedPingCountersFilename))
            {
                string fileContent = await sr.ReadToEndAsync();
                _failedPingCounters = JsonConvert.DeserializeObject<List<PingCounter>>(fileContent);
            }

            if (_failedPingCounters == null)
            {
                _failedPingCounters = new List<PingCounter>();
            }
        }
    }
}
