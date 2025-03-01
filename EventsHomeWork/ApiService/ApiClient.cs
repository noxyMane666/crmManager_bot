using EventsHomeWork.KeyBoardsPrinter;
using EventsHomeWork.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace EventsHomeWork
{
    public class ApiClient(string apiUrl) : IDisposable
    {
        private readonly HttpClient _httpClient = new ();
        private readonly string _apiUrl = apiUrl;

        public async Task<string> GetItemsRequest(int ffUserId, EventServiceTypes requestType, CancellationToken cancellationToken)
        {
            string _urlWithParams = $"{_apiUrl}?userId={Uri.EscapeDataString(ffUserId.ToString())}&type={Uri.EscapeDataString(requestType.ToString())}";

            HttpResponseMessage _response = await _httpClient.GetAsync(_urlWithParams, cancellationToken);
            _response.EnsureSuccessStatusCode();

            string jsonResponse = await _response.Content.ReadAsStringAsync(cancellationToken);

            return jsonResponse;
        }

        public async Task<string> GetUserRequest(long telegramUserId, CancellationToken cancellationToken)
        {
            string _urlWithParams = $"{_apiUrl}?telegramUserId={Uri.EscapeDataString(telegramUserId.ToString())}";

            HttpResponseMessage _response = await _httpClient.GetAsync(_urlWithParams, cancellationToken);
            _response.EnsureSuccessStatusCode();

            string jsonResponse = await _response.Content.ReadAsStringAsync(cancellationToken);

            return jsonResponse;
        }

        public async Task<string> PostAnswerRequest(StringContent requestBody, CancellationToken cancellationToken)
        {
            HttpResponseMessage _response = await _httpClient.PostAsync(_apiUrl, requestBody, cancellationToken);
            _response.EnsureSuccessStatusCode();

            string jsonResponse = await _response.Content.ReadAsStringAsync(cancellationToken);
            return jsonResponse;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

}
