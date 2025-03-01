using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot;
using Microsoft.Extensions.Hosting;

namespace EventsHomeWork.StaticServices
{
    public class BotService : BackgroundService
    {
        private readonly TelegramBotClient _botClient;
        private readonly UpdateHandler _updateHandler;
        private readonly ILogger<BotService> _logger;

        public BotService(IConfiguration config, UpdateHandler updateHandler, ILogger<BotService> logger)
        {
            _logger = logger;
            _updateHandler = updateHandler;
            var apiToken = config["TelegramBot:ApiToken"] ?? throw new InvalidOperationException("No API-Keys");
            _botClient = new TelegramBotClient(apiToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Bot started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
                    DropPendingUpdates = true
                };

                _botClient.StartReceiving(
                    _updateHandler.HandleUpdateAsync,
                    _updateHandler.HandleErrorAsync,
                    receiverOptions,
                    CancellationToken.None
                );

                await Task.Delay(-1, stoppingToken);
            }
        }
    }
}
