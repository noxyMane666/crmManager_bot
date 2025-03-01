using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EventsHomeWork.Abstrctions
{
    public interface IWorker
    {
        Task HandleCommand(ITelegramBotClient botClient, User currentUser, string commandMessage, CancellationToken cancellationToken);

        Task HandleCallback(ITelegramBotClient botClient, IItem currentItem, long chatId, CancellationToken cancellationToken);

        Task ProcessUserEntities(ITelegramBotClient botClient, User currentUser, string jsonResponse, CancellationToken cancellationToken);
    }
}
