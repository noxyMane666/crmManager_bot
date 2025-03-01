using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;
using EventsHomeWork.Models;
using EventsHomeWork.Services;
using EventsHomeWork.Exceptions;
using EventsHomeWork.Abstrctions;
using Microsoft.Extensions.Logging;

namespace EventsHomeWork.Handlers
{
    internal class CallbackHandler(ILogger<CallbackHandler> logger, IErrorHandler errorHandler) : ICallbackHandler
    {
        private readonly ILogger<CallbackHandler> _logger = logger;
        private readonly IErrorHandler _errorHandler = errorHandler;

        public async Task HandleCallbackAsync(ITelegramBotClient botClient, User currentUser, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery?.Data == null || callbackQuery.Message == null)
                return;

            if (currentUser.UserItems == null || currentUser.CurrentWorker == null)
            {
                await HandleErrorAsync(botClient, currentUser, callbackQuery.Message.Chat.Id, new WorkerNotFoundException("Worker not found"), cancellationToken);
                return;
            }

            if (!int.TryParse(callbackQuery.Data, out int itemId))
            {
                await HandleErrorAsync(botClient, currentUser, callbackQuery.Message.Chat.Id, new ItemNotFoundException("Item not found"), cancellationToken);
                return;
            }

            var chosenItem = currentUser.UserItems.FirstOrDefault(u => u.ItemId == itemId);
            if (chosenItem == null)
            {
                await HandleErrorAsync(botClient, currentUser, callbackQuery.Message.Chat.Id, new ItemNotFoundException("Item not found"), cancellationToken);
                return;
            }

            currentUser.SetChoosenItem(chosenItem);
            await currentUser.CurrentWorker.HandleCallback(botClient, chosenItem, callbackQuery.Message.Chat.Id, cancellationToken);
        }

        private async Task HandleErrorAsync(ITelegramBotClient botClient, User user, long chatId, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, exception.Message);
            await _errorHandler.HandleErrorAsync(botClient, user, chatId, cancellationToken);
        }
    }
}
