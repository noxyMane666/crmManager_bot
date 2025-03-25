using EventsHomeWork.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EventsHomeWork.Models
{
    public class Comment(int itemId, string itemText, int itemTaskId, string itemTaskText, int? senderId, string? senderName, string? reprlyToMessage) : IItem
    {
        public int ItemId { get; set; } = itemId;
        public string ItemText { get; set; } = itemText;
        public int ItemTaskId { get; set; } = itemTaskId;
        public string ItemTaskText { get; set; } = itemTaskText;
        public int? SenderId { get; set; } = senderId;
        public string? SenderName { get; set; } = senderName;
        public string? ReprlyToMessage { get; set; } = reprlyToMessage;
        public string? AnswerMessage { get; set; }

        public string GetItemTextMessage()
        {
            return $"*Номер задачи комментария:*\n{ItemTaskId}\n\n" +
                   $"*Текст задачи комментария:*\n{EscapeMarkdown(ItemTaskText)}\n\n" +
                   $"*Отправитель комментария:*\n{EscapeMarkdown(SenderName ?? "Неизвестно")}\n\n" +
                   $"*Текст комментария:*\n{EscapeMarkdown(ItemText)}";
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

        private static string EscapeMarkdown(string text)
        {
            return MyRegex().Replace(text, "\\$1");
        }

        private static Regex MyRegex() => new Regex(@"([_*\\[\]()~`>#+=|{}.!])", RegexOptions.Compiled);

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
    }
}
