using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EventsHomeWork.Abstrctions
{
    public interface ICommandHandler
    {
        Task HandleCommandAsync(ITelegramBotClient botClient, User currentUser, string commandMessage, CancellationToken cancellationToken);
        bool CanHandle(string commandMessage);
    }
}
