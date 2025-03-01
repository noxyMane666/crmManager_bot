using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace EventsHomeWork.Abstrctions
{
    public interface IHandler<T>
    {
        static abstract Task Handle(ITelegramBotClient botClient, User currentUser, T data , CancellationToken cancellationToken);
    }
}
