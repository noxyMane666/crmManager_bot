using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace EventsHomeWork.Abstrctions
{
    public interface ICallbackHandler
    {
        Task HandleCallbackAsync(ITelegramBotClient botClient, User currentUser, CallbackQuery callbackQuery, CancellationToken cancellationToken);
    }
}
