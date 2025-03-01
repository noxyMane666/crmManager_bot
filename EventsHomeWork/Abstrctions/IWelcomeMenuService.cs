using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EventsHomeWork.Abstrctions
{
    public interface IWelcomeMenuService
    {
        Task PrintWelcomeMenuAsync(ITelegramBotClient botClient, long chatId, CancellationToken cancellationToken);
        IWorker? GetWorker(string workerKey);
    }
}
