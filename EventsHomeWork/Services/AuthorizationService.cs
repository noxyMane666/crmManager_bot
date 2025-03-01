using EventsHomeWork.Workers;
using EventsHomeWork.Abstrctions;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventsHomeWork
{
    public class AuthorizationService : IAuthorizationService
    {
        public async Task<int> GetUserIdAsync(long telegramUserId, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return 0;


            string _apiUrl = ApiUrlDict.TryGetUrl("GetUserId");
            var _client = new ApiClient(_apiUrl);

            var jsonResult = await _client.GetUserRequest(telegramUserId, cancellationToken);
            _client.Dispose();

            AuthApiResponse? apiResponse = JsonConvert.DeserializeObject<AuthApiResponse>(jsonResult) ?? throw new Exception("Failed to get data.");
            return apiResponse.UserId;
        }
    }
}
