using EventsHomeWork.Abstrctions;
using EventsHomeWork.Exceptions;
using EventsHomeWork.StaticServices;
using EventsHomeWork.KeyBoardsPrinter;
using EventsHomeWork.Models;
using EventsHomeWork.Workers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using EventsHomeWork.Services;
using Microsoft.Extensions.Logging;

namespace EventsHomeWork
{
    public class QuestionWorker : IWorker
    {
        private readonly ILogger<QuestionWorker> _logger;
        private readonly IErrorHandler _errorHandler;
        private readonly IItemService _itemService;
        private readonly AnswerWorker _answerService;
        private readonly Dictionary<string, Func<ITelegramBotClient, User, CancellationToken, Task>> _handledCommands;
        public QuestionWorker(ILogger<QuestionWorker> logger, IErrorHandler errorHandler, IItemService itemService, AnswerWorker answerWorker)
        {
            _logger = logger;
            _errorHandler = errorHandler;
            _itemService = itemService;
            _answerService = answerWorker;

            _handledCommands = new()
            {
                {"Ответить на вопрос", async (botClient, user, token) =>
                    await _itemService.ReqeustItemAnswer(botClient, user, token)},

                {"Ко всем вопросам", async (botClient, user, token) =>
                    await _itemService.PrintUserItems(botClient, user, token)},

                {"Мои вопросы", async (botClient, user, token) =>
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

            currentUser.SetUserEventType(EventServiceTypes.Question);

            if (_handledCommands.TryGetValue(commandMessage, out var _questionCommandHandler))
            {
                if (_questionCommandHandler != null)
                    await _questionCommandHandler(botClient, currentUser, cancellationToken);

                return;
            }
            else
            {
                _logger.LogError(new CommandNotFoundException("Command is not found"), "Command is not found");
                await _errorHandler.HandleErrorAsync(
                    botClient, 
                    currentUser,
                    currentUser.ChatId,
                    cancellationToken
                );

                return;
            }
        }

        public async Task HandleCallback(ITelegramBotClient botClient, IItem currentQuestion, long chatId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            string message = currentQuestion.GetItemTextMessage();
            await PrintItemsService.PrintUserItem(
                botClient,
                chatId,
                message,
                _handledCommands.Keys.Where(key => key != "Мои вопросы").ToArray(),
                cancellationToken
            );

            return;
        }

        public async Task ProcessUserEntities(ITelegramBotClient botClient, User currentUser, string jsonResponse, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            List<Question>? _userQuestions = JsonConvert.DeserializeObject<List<Question>>(jsonResponse);

            currentUser.SetUserItems(_userQuestions);
            await PrintItemsService.PrintAlItems(botClient, currentUser.ChatId, currentUser.UserItems, cancellationToken);

            return;
        }
    }
}
