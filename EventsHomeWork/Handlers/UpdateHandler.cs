using EventsHomeWork.Exceptions;
using EventsHomeWork.Handlers;
using EventsHomeWork.StaticServices;
using EventsHomeWork.Abstrctions;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static Telegram.Bot.TelegramBotClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualBasic;
using System.Threading;

namespace EventsHomeWork
{

    public class UpdateHandler(
        ILogger<UpdateHandler> logger,
        IAuthorizationService authorizationService,
        IActiveUsersService activeUsersService,
        IMessageHandler messageHandler,
        ICallbackHandler callbackHandler)
    {
        private readonly ILogger<UpdateHandler> _logger = logger;
        private readonly IAuthorizationService _authorizationService = authorizationService;
        private readonly IActiveUsersService _activeUsersService = activeUsersService;
        private readonly IMessageHandler _messageHandler = messageHandler;
        private readonly ICallbackHandler _callbackHandler = callbackHandler;

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var chatId = update.Message?.Chat.Id ?? update.CallbackQuery?.Message?.Chat.Id;
            var telegramId = update.Message?.From?.Id ?? update.CallbackQuery?.From?.Id;

            if (chatId is null) return;

            try
            {
                var (isAuth, user) = await AuthenticateUserAsync(telegramId, chatId, cancellationToken);

                if (isAuth && user is not null)
                {
                    user.UpdateActivity();
                    switch (update.Type)
                    {
                        case UpdateType.Message:
                            await HandleMessageAsync(botClient, update.Message, user, cancellationToken);
                            break;
                        case UpdateType.CallbackQuery:
                            await HandleCallbackQueryAsync(botClient, update.CallbackQuery, user, cancellationToken);
                            break;
                    }
                }
                else
                {
                    await botClient.SendMessage(chatId, "Доступ запрещен", cancellationToken: cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling update.");
                await botClient.SendMessage(chatId, "Доступ запрещен", cancellationToken: cancellationToken);
            }
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred in the bot.");
            await Task.CompletedTask;
        }

        private async Task HandleMessageAsync(ITelegramBotClient botClient, Message? message, User user, CancellationToken cancellationToken)
        {
            if (message?.From == null || string.IsNullOrWhiteSpace(message.Text))
                return;

            await _messageHandler.HandleMessageAsync(botClient, user, message, cancellationToken);
        }

        private async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery? callbackQuery, User user, CancellationToken cancellationToken)
        {
            if (callbackQuery?.Data == null || callbackQuery.From == null || callbackQuery.Message == null)
                return;

            await _callbackHandler.HandleCallbackAsync(botClient, user, callbackQuery, cancellationToken);
        }

        private async Task<(bool isAuth, User? user)> AuthenticateUserAsync(long? tgUserId, long? chatId, CancellationToken cancellationToken)
        {
            if (tgUserId is null || chatId is null)
                return (false, null);
            
            try
            {
                var userId = await _authorizationService.GetUserIdAsync((long)tgUserId, cancellationToken);
                if (userId == 0)
                {
                    return (false, null);
                }

                var user = _activeUsersService.GetOrCreateUser((long)tgUserId, userId, (long)chatId);
                return (true, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user authentication.");
                return (false, null);
            }
        }
    }
}