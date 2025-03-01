using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EventsHomeWork.Abstrctions
{
    public interface IItemService
    {
        Task GetItems(ITelegramBotClient botClient, User currentUser, CancellationToken cancellationToken);
        Task PrintUserItems(ITelegramBotClient botClient, User user, CancellationToken cancellationToken);
        Task ReqeustItemAnswer(ITelegramBotClient botClient, User currentUser, CancellationToken cancellationToken);
        Task ProcessEndpoint(ITelegramBotClient botClient, User currentUser, CancellationToken cancellationToken);
    }
}
