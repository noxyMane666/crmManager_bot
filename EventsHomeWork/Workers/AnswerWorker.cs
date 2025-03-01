using EventsHomeWork.Abstrctions;
using EventsHomeWork.Exceptions;
using EventsHomeWork.KeyBoardsPrinter;
using EventsHomeWork.Models;

using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace EventsHomeWork.Workers
{
    internal class AnswerWorker : IWorker
    {
        private readonly ILogger<ItemActionService> _logger;
        private readonly IErrorHandler _errorHandler;
        private readonly IItemService _itemService;
        private readonly Dictionary<string, Func<ITelegramBotClient, User, CancellationToken, Task>> _answerEndpoint;
        public AnswerWorker(ILogger<ItemActionService> logger, IErrorHandler errorHandler, IItemService itemService)
        {
            _errorHandler = errorHandler;
            _logger = logger;
            _itemService = itemService;
            _answerEndpoint = new()
            {
                {"Главное меню",  async (botClient, user, token) =>
                    await _itemService.ProcessEndpoint(botClient, user, token)
                }
            };
        }
        
        public async Task HandleCommand(ITelegramBotClient botClient, User currentUser, string commandMessage, CancellationToken cancellationToken)
        {
            if (_answerEndpoint.TryGetValue(commandMessage, out var _endPoint))
            {
                await _endPoint(botClient, currentUser, cancellationToken);

                return;
            }

            if (currentUser.UserForm is null)
                await AnswerToItem(botClient, currentUser, commandMessage, cancellationToken);
            else
                await AnswerToForm(botClient, currentUser, commandMessage, cancellationToken);

            return;
        }

        public async Task HandleCallback(ITelegramBotClient botClient, IItem currentTask, long chatId, CancellationToken cancellationToken)
        {
            return;
        }

        public async Task ProcessUserEntities(ITelegramBotClient botClient, User currentUser, string jsonResponse, CancellationToken cancellationToken)
        {
            try
            {
                var _apiUrl = ApiUrlDict.TryGetUrl("AnswerUser");
                ApiClient _client = new(_apiUrl);

                var content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");
                var jsonResult = await _client.PostAnswerRequest(content, cancellationToken);
                AnswerApiResposnse? apiResponse = JsonConvert.DeserializeObject<AnswerApiResposnse>(jsonResult);

                if (apiResponse == null || apiResponse.Message != "success")
                {
                    string _errorMessage = apiResponse?.Message ?? "При выполнении запроса пролизошли ошибки";

                    _logger.LogError(new AnswerIsFailedException(_errorMessage), "Error while processing answer");
                    await _errorHandler.HandleErrorAsync(botClient, currentUser, currentUser.ChatId, cancellationToken);
                    _client.Dispose();
                    return;
                }

                var _keyboard = ReplyKeyboardBuilder.BuildReplyKeyboard(["Главное меню"]);
                await botClient.SendMessage(
                chatId: currentUser.ChatId,
                    text: "Ваш ответ доставлен",
                    replyMarkup: _keyboard,
                    cancellationToken: cancellationToken
                );

                _client.Dispose();
                return;

            }
            catch (JsonException jEx)
            {
                _logger.LogError(jEx, "Error while parsing JSON");
                await _errorHandler.HandleErrorAsync(botClient, currentUser, currentUser.ChatId, cancellationToken);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing answer");
                await _errorHandler.HandleErrorAsync(botClient, currentUser, currentUser.ChatId, cancellationToken);
                return;
            }
        }

        private async Task AnswerToForm(ITelegramBotClient botClient, User currentUser, string commandMessage, CancellationToken cancellationToken) 
        {
            if (currentUser.UserForm is not null)
            {
                try
                {
                    string formData = currentUser.UserForm.GetFormData(commandMessage);
                    await botClient.SendMessage(
                        chatId: currentUser.ChatId,
                        text: formData,
                        cancellationToken: cancellationToken
                    );

                    if (currentUser.UserForm.IsCompleted)
                    {
                        string _answerContent = currentUser.UserForm.GetFormRequestBody(currentUser.FfUserId);
                        await this.ProcessUserEntities(botClient, currentUser, _answerContent, cancellationToken);

                        return;
                    }

                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing UserForm");
                    await _errorHandler.HandleErrorAsync(botClient, currentUser, currentUser.ChatId, cancellationToken);
                }

                return;
            }
            else
            {
                _logger.LogError(new FormNotFoundException("UserForm is not found"), "UserForm is not found");
                await _errorHandler.HandleErrorAsync(botClient, currentUser, currentUser.ChatId, cancellationToken);

                return;
            }
        }

        private async Task AnswerToItem(ITelegramBotClient botClient, User currentUser, string commandMessage, CancellationToken cancellationToken)
        {
            
            if (currentUser.ChoosenItem is not null)
            {
                currentUser.ChoosenItem.AnswerMessage = commandMessage;

                string answerContent = currentUser.ChoosenItem.GetFormRequestBody(currentUser.FfUserId);
                await this.ProcessUserEntities(botClient, currentUser, answerContent, cancellationToken);
            }
            else
            {
                _logger.LogError(new ItemNotFoundException("Item is not found"), "Item is not found");
                await _errorHandler.HandleErrorAsync(botClient, currentUser, currentUser.ChatId, cancellationToken);

                return;
            }
        }
    }
}
