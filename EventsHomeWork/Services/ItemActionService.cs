using EventsHomeWork.Abstrctions;
using EventsHomeWork.Exceptions;
using EventsHomeWork.StaticServices;
using EventsHomeWork.Models;
using EventsHomeWork.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Logging;

namespace EventsHomeWork
{
    internal class ItemActionService(ILogger<ItemActionService> logger, IErrorHandler errorHandler, IWelcomeMenuService welcomeMenu, IActiveUsersService activeUsersService) : IItemService
    {
        private readonly ILogger<ItemActionService> _logger = logger;
        private readonly IErrorHandler _errorHandler = errorHandler;
        private readonly IWelcomeMenuService _welcomeMenu = welcomeMenu;
        private readonly IActiveUsersService _activeUsersService = activeUsersService;

        public async Task GetItems(ITelegramBotClient botClient, User currentUser, CancellationToken cancellationToken)
        {
            if (currentUser.CurrentWorker is null)
                throw new WorkerNotFoundException("Не обнаружен worker");

            try
            {
                string _apiUrl = ApiUrlDict.TryGetUrl("GetUserItems");
                var _client = new ApiClient(_apiUrl);

                var _userItems = await _client.GetItemsRequest(currentUser.FfUserId, currentUser.CurrentEventType, cancellationToken);
                await currentUser.CurrentWorker.ProcessUserEntities(botClient, currentUser, _userItems, cancellationToken);
                _client.Dispose();

                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while reqeuting items");
                await _errorHandler.HandleErrorAsync(botClient, currentUser, currentUser.ChatId, cancellationToken);

                return;
            }
        }

        public async Task PrintUserItems(ITelegramBotClient botClient, User user, CancellationToken cancellationToken)
        {
            await PrintItemsService.PrintAlItems(botClient, user.ChatId, user.UserItems, cancellationToken);

            return;
        }

        public async Task ReqeustItemAnswer(ITelegramBotClient botClient, User currentUser, CancellationToken cancellationToken)
        {
            currentUser.SetAnswerRequest(true);
            await botClient.SendMessage(currentUser.ChatId, $"Введите ответ", cancellationToken: cancellationToken);
        }

        public async Task ProcessEndpoint(ITelegramBotClient botClient, User currentUser, CancellationToken cancellationToken)
        {
            _activeUsersService.RemoveUser(currentUser);
            await _welcomeMenu.PrintWelcomeMenuAsync(botClient, currentUser.ChatId, cancellationToken);
        }
    }
}
