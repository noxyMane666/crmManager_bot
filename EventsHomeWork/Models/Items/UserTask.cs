using EventsHomeWork.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EventsHomeWork.Models
{
    public class UserTask(int itemId, string itemText, int itemTaskId, string itemTaskText, DateTime? itemOrderDate, int? senderId, string? senderName) : IItem
    {
        public int ItemId { get; set; } = itemId;
        public string ItemText { get; set; } = itemText;
        public string ItemTaskText { get; set; } = itemTaskText;
        public DateTime? ItemOrderDate { get; set; } = itemOrderDate;
        public int ItemTaskId { get; set; } = itemTaskId;
        public int? SenderId { get; set; } = senderId;
        public string? SenderName { get; set; } = senderName;
        public string? AnswerMessage { get; set; }

        public string GetItemTextMessage()
        {
            return $"*Номер задачи:*\n{ItemTaskId}\n\n" +
                   $"*Заказчик задачи:*\n{EscapeMarkdown(SenderName ?? "Неизвестно")}\n\n" +
                   $"*Текст задачи:*\n{EscapeMarkdown(ItemText)}\n\n" +
                   $"*Срок задачи:*\n{EscapeMarkdown(ItemOrderDate?.ToString() ?? "Дата не указана")}";
        }

        public string GetFormRequestBody(int userId)
        {
            var requestBody = new
            {
                answerType = AnswerTypes.ItemAnswer.ToString(),
                itemId = ItemId,
                itemTaskId = ItemTaskId,
                currentUserId = userId,
                answerText = AnswerMessage,
                senderId = SenderId,
            };
            return JsonConvert.SerializeObject(requestBody);
        }

        public async Task SendAnswerAsync(ITelegramBotClient botClient, User user, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(AnswerMessage))
            {
                await botClient.SendMessage(user.ChatId, "Ответ не может быть пустым", cancellationToken: cancellationToken);
                return;
            }

            string requestBody = GetFormRequestBody(user.FfUserId);
            await botClient.SendMessage(user.ChatId, $"Отправка данных: {requestBody}", cancellationToken: cancellationToken);

            return;
        }
        private static string EscapeMarkdown(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Regex.Replace(text, "([_*\\[\\]()~`>#\\+=|{}.!-])", "\\$1");
        }
    }
}
