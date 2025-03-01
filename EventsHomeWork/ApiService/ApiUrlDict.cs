using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork
{
    public static class ApiUrlDict
    {
        private static readonly Dictionary<string, string> _apiUrls = new()
        {
            { "GetUserItems", "https://pilot.1forma.ru/app/v1.2/api/publications/action/GetItemService" },
            { "GetUserId", "https://pilot.1forma.ru/app/v1.2/api/publications/action/TelegramAuthorizeService" },
            { "AnswerUser", "https://pilot.1forma.ru/app/v1.2/api/publications/action/TelegramAnswerService" }
        };

        public static string TryGetUrl(string key)
        {
            return _apiUrls.TryGetValue(key, out var url) ? url : string.Empty;
        }
    }
}
