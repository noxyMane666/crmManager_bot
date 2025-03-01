using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace EventsHomeWork.Abstrctions
{
    public interface IMessageHandler
    {
        Task HandleMessageAsync(ITelegramBotClient botClient, User currentUser, Message message, CancellationToken cancellationToken);
    }
}
