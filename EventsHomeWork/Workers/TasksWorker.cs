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
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using static EventsHomeWork.Models.ChangeTaskDateForm;
using EventsHomeWork.DB;
using Microsoft.Extensions.Logging;

namespace EventsHomeWork
{
    public class TasksWorker : IWorker
    {
        private readonly ILogger<TasksWorker> _logger;
        private readonly IErrorHandler _errorHandler;
        private readonly IItemService _itemService;
        private readonly AnswerWorker _answerService;
        private readonly Dictionary<string, Func<ITelegramBotClient, User, CancellationToken, Task>> _handledCommands;
        private readonly Dictionary<string, EventServiceTypes> _commandEventTypeMapping;

        public TasksWorker(ILogger<TasksWorker> logger, IErrorHandler errorHandler, IItemService itemService, AnswerWorker answerWorker)
        {
            _logger = logger;
            _errorHandler = errorHandler;
            _itemService = itemService;
            _answerService = answerWorker;
            _handledCommands = new()
            {
                {"Ко всем задачамм", async (botClient, user, token) =>
                    await _itemService.PrintUserItems(botClient, user, token)},

                {"Я - исполнитель", async (botClient, user, token) =>
                    await _itemService.GetItems(botClient, user, token)},

                {"Просроченные задачи", async (botClient, user, token) =>
                    await _itemService.GetItems(botClient, user, token)},

                {"Главное меню",  async (botClient, user, token) =>
                    await _itemService.ProcessEndpoint(botClient, user, token)},

                {"Сменить срок", async (botClient, user, token) =>
                    await ChangeUserTaskDate(botClient, user, token)}
            };

            _commandEventTypeMapping = new()
            {
                {"Я - исполнитель", EventServiceTypes.UserTask},
                {"Просроченные задачи", EventServiceTypes.OverduedTask}
            };
        }


        public async Task HandleCommand(ITelegramBotClient botClient, User currentUser, string commandMessage, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            currentUser.SetUserWorker(this);
            
            if (_commandEventTypeMapping.TryGetValue(commandMessage, out var _eventType))
            {
                currentUser.SetUserEventType(_eventType);
            }

            if (_handledCommands.TryGetValue(commandMessage, out var _welcomeCommandHandler))
            {
                await _welcomeCommandHandler(botClient, currentUser, cancellationToken);
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
            }

            return;
        }

        public async Task HandleCallback(ITelegramBotClient botClient, IItem currentTask, long chatId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            string message = currentTask.GetItemTextMessage();
            await PrintItemsService.PrintUserItem(
                botClient,
                chatId,
                message,
                [.. _handledCommands.Keys.Except(_commandEventTypeMapping.Keys)],
                cancellationToken
            );

            return;
        }

        public async Task ProcessUserEntities(ITelegramBotClient botClient, User currentUser, string jsonResponse, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            List<UserTask>? _userTasks = JsonConvert.DeserializeObject<List<UserTask>>(jsonResponse);

            currentUser.SetUserItems(_userTasks);
            await PrintItemsService.PrintAlItems(botClient, currentUser.ChatId, currentUser.UserItems, cancellationToken);

            return;
        }

        public async Task ChangeUserTaskDate(ITelegramBotClient botClient, User currentUser, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (currentUser.ChoosenItem is not null)
            {
                currentUser.SetUserForm(new ChangeTaskDateForm(currentUser.ChoosenItem.ItemTaskId));
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


                await currentUser.CurrentWorker.HandleCommand(botClient, currentUser, "", cancellationToken);

                return;
            }
            else
            {
                _logger.LogError(new ItemNotFoundException("Item is not found."), "Item is not found");
                await _errorHandler.HandleErrorAsync(
                    botClient, 
                    currentUser,
                    currentUser.ChatId,
                    cancellationToken
                );

                return;
            }
        }
    }
}
