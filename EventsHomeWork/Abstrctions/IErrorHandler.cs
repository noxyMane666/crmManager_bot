using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EventsHomeWork.Abstrctions
{
    public interface IErrorHandler
    {
        Task HandleErrorAsync(ITelegramBotClient botClient, User? currentUser, long? chatId, CancellationToken cancellationToken);
    }
}
