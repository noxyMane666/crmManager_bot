using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using EventsHomeWork.Models;
using EventsHomeWork.Services;
using EventsHomeWork.Abstrctions;
using EventsHomeWork.Exceptions;
using EventsHomeWork.StaticServices;
using EventsHomeWork.Handlers;
using Microsoft.Extensions.Logging;

namespace EventsHomeWork
{

    public class MessageHandler(
        ILogger<MessageHandler> logger,
        ICommandHandler commandHandler,
        IErrorHandler errorHandler,
        IWelcomeMenuService welcomeMenu) : IMessageHandler
    {
        private readonly ILogger<MessageHandler> _logger = logger;
        private readonly ICommandHandler _commandHandler = commandHandler;
        private readonly IErrorHandler _errorHandler = errorHandler;
        private readonly IWelcomeMenuService _welcomeMenuService = welcomeMenu;

        public async Task HandleMessageAsync(ITelegramBotClient botClient, User currentUser, Message message, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested || string.IsNullOrWhiteSpace(message.Text))
                return;

            if (_commandHandler.CanHandle(message.Text.Split(' ')[0]))
            {
                await _commandHandler.HandleCommandAsync(botClient, currentUser, message.Text.Split(' ')[0], cancellationToken);
                return;
            }

            var worker = currentUser.CurrentWorker ?? _welcomeMenuService.GetWorker(message.Text);
            if (worker != null)
            {
                await worker.HandleCommand(botClient, currentUser, message.Text, cancellationToken);
                return;
            }

            var ex = new WorkerNotFoundException("Worker not found");
            _logger.LogError(ex, "Worker not found");
            await _errorHandler.HandleErrorAsync(botClient, currentUser, message.Chat.Id, cancellationToken);
        }
    }
}
