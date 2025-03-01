using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using EventsHomeWork.StaticServices;
using EventsHomeWork.Exceptions;
using System.Threading;
using Telegram.Bot.Types;
using EventsHomeWork.Abstrctions;
using Microsoft.Extensions.Logging;

namespace EventsHomeWork.Handlers
{
    public class CommandHandler : ICommandHandler
    {
        private readonly ILogger<CommandHandler> _logger;
        private readonly IErrorHandler _errorHandler;
        private readonly IWelcomeMenuService _welcomeMenuService;
        private readonly Dictionary<string, Func<ITelegramBotClient, long, CancellationToken, Task>> _handledCommands;

        public CommandHandler(
            ILogger<CommandHandler> logger,
            IErrorHandler errorHandler,
            IWelcomeMenuService welcomeMenuService)
        {
            _logger = logger;
            _errorHandler = errorHandler;
            _welcomeMenuService = welcomeMenuService;

            _handledCommands = new Dictionary<string, Func<ITelegramBotClient, long, CancellationToken, Task>>
            {
                {"/start", async (botClient, chatId, token) =>
                    await _welcomeMenuService.PrintWelcomeMenuAsync(botClient, chatId, token)}
            };
        }

        public async Task HandleCommandAsync(ITelegramBotClient botClient, User currentUser, string commandMessage, CancellationToken cancellationToken)
        {
            if (_handledCommands.TryGetValue(commandMessage, out var command))
            {
                await command(botClient, currentUser.ChatId, cancellationToken);
            }
            else
            {
                _logger.LogError(new CommandNotFoundException("Unknown command"), "Unknown command");
                await _errorHandler.HandleErrorAsync(botClient, currentUser, currentUser.ChatId, cancellationToken);
            }
        }

        public bool CanHandle(string commandMessage)
        {
            return _handledCommands.ContainsKey(commandMessage);
        }
    }
}
