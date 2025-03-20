using EventsHomeWork.Abstrctions;
using EventsHomeWork.Exceptions;
using EventsHomeWork.Models;
using EventsHomeWork.Workers;
using EventsHomeWork.StaticServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;

namespace EventsHomeWork.Services
{
    public class CommentWorker : IWorker
    {
        private readonly ILogger<CommentWorker> _logger;
        private readonly IErrorHandler _errorHandler;
        private readonly IItemService _itemService;
        private readonly AnswerWorker _answerService;
        private readonly Dictionary<string, Func<ITelegramBotClient, User, CancellationToken, Task>> _handledCommands;
        public CommentWorker(ILogger<CommentWorker> logger, IErrorHandler errorHandler, IItemService itemService, AnswerWorker answerWorker)
        {
            _logger = logger;
            _errorHandler = errorHandler;
            _itemService = itemService;
            _answerService = (AnswerWorker)answerWorker;

            _handledCommands = new()
            {
                {"Ответить на комментарий", async (botClient, user, token) =>
                    await _itemService.ReqeustItemAnswer(botClient, user, token)},

                {"Ко всем комментариям", async (botClient, user, token) =>
                    await _itemService.PrintUserItems(botClient, user, token)},

                {"Непрочитанные комментарии", async (botClient, user, token) =>
                    await _itemService.GetItems(botClient, user, token)},

                {"Главное меню",  async (botClient, user, token) =>
                    await _itemService.ProcessEndpoint(botClient, user, token)}
            };
        }

        public async Task HandleCommand(ITelegramBotClient botClient, User currentUser, string commandMessage, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            currentUser.SetUserWorker(this);

            if (currentUser.IsAnswerRequested)
            {
                currentUser.SetUserWorker(_answerService);

                if (currentUser.CurrentWorker is null)
                {
                    _logger.LogError(new WorkerNotFoundException("Woker is not found."), "Worker is not found");
                    await _errorHandler.HandleErrorAsync(
                        botClient,
                        currentUser,
                        currentUser.ChatId,
                        cancellationToken
                    );

                    return;
                }
                await currentUser.CurrentWorker.HandleCommand(botClient, currentUser, commandMessage, cancellationToken);

                return;
            }

            currentUser.SetUserEventType(EventServiceTypes.Comments);

            if (_handledCommands.TryGetValue(commandMessage, out var _commentCommandHandler))
            {
                if (_commentCommandHandler != null)
                    await _commentCommandHandler(botClient, currentUser, cancellationToken);

                return;
            }
            else
            {
                _logger.LogError(new CommandNotFoundException("Command is not found."), "Command is not found");
                await _errorHandler.HandleErrorAsync(
                    botClient, 
                    currentUser,
                    currentUser.ChatId,
                    cancellationToken
                );

                return;
            }
        }

        public async Task HandleCallback(ITelegramBotClient botClient, IItem currentComment, long chatId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            string message = currentComment.GetItemTextMessage();
            await PrintItemsService.PrintUserItem(
                botClient,
                chatId,
                message,
                _handledCommands.Keys.Where(key => key != "Непрочитанные комментарии").ToArray(),
                cancellationToken
            );

            return;
        }

        public async Task ProcessUserEntities(ITelegramBotClient botClient, User currentUser, string jsonResponse, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            List<Comment>? _userComment = JsonConvert.DeserializeObject<List<Comment>>(jsonResponse);

            currentUser.SetUserItems(_userComment);
            await PrintItemsService.PrintAlItems(botClient, currentUser.ChatId, currentUser.UserItems, cancellationToken);
        }
    }
}
