using EventsHomeWork.Abstrctions;
using EventsHomeWork.Handlers;
using EventsHomeWork.KeyBoardsPrinter;
using EventsHomeWork.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventsHomeWork.StaticServices
{
    public class WelcomeMenu : IWelcomeMenuService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Lazy<IWorker>> _handledCommands;

        public WelcomeMenu(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _handledCommands = new()
            {
                {"Я - исполнитель", new Lazy<IWorker>(() => serviceProvider.GetRequiredService<TasksWorker>())},
                {"Просроченные задачи", new Lazy<IWorker>(() => serviceProvider.GetRequiredService<TasksWorker>())},
                {"Мои вопросы", new Lazy<IWorker>(() => serviceProvider.GetRequiredService<QuestionWorker>())},
                {"Непрочитанные комментарии", new Lazy<IWorker>(() => serviceProvider.GetRequiredService<CommentWorker>())}
            };
        }

        public async Task PrintWelcomeMenuAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            string[] _menuItemsData = [.. _handledCommands.Keys];
            var _keyboard = ReplyKeyboardBuilder.BuildReplyKeyboard(_menuItemsData);

            await botClient.SendMessage(
                chatId: chatId,
                text: "Выберите действие",
                replyMarkup: _keyboard,
                cancellationToken: cancellationToken
            );

            return;
        }

        public IWorker? GetWorker(string workerKey)
        {
            if (_handledCommands.TryGetValue(workerKey, out var worker))
            {
                return worker.Value;
            }
            else
            {
                return null;
            }
        }
    }
}
