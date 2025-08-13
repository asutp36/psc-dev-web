using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BotNotificationService.Options
{
    public class TelegramBotOptionsSetup : IConfigureOptions<TelegramBotOptions>
    {
        private const string SectionName = "BotOptions";
        
        private readonly IConfiguration _configuration;

        public TelegramBotOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(TelegramBotOptions options)
        {
            _configuration.GetSection(SectionName).Bind(options);
        }
    }
}
