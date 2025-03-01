using EventsHomeWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using EventsHomeWork.StaticServices;
using EventsHomeWork.Abstrctions;

namespace EventsHomeWork
{
    internal class ErrorHandler(IWelcomeMenuService welcomeMenuService, IActiveUsersService activeUsersService) : IErrorHandler
    {
        private readonly IWelcomeMenuService _welcomeMenuService = welcomeMenuService;
        private readonly IActiveUsersService _activeUsersService = activeUsersService;

        public async Task HandleErrorAsync(ITelegramBotClient botClient, User? currentUser, long? chatId, CancellationToken cancellationToken)
        {

            if (currentUser is not null)
            {
                _activeUsersService.RemoveUser(currentUser);
            }

            if (chatId is not null)
            {

                await botClient.SendMessage(chatId, "Произошла ошибка. Попробуйте снова.", cancellationToken: cancellationToken);
                await _welcomeMenuService.PrintWelcomeMenuAsync(botClient, (long)chatId, cancellationToken);

                return;
            }

            return;
        }
    }
}
