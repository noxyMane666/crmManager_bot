using EventsHomeWork.KeyBoardsPrinter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace EventsHomeWork
{
    internal static class PrintItemsService
    {
        internal static async Task PrintAlItems(
            ITelegramBotClient botClient,
            long chatId, 
            List<IItem>? items, 
            CancellationToken cancellationToken
        )
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (items is null || items.Count == 0)
            {
                var _keyBoard = ReplyKeyboardBuilder.BuildReplyKeyboard(["Главное меню"]);
                await botClient.SendMessage(chatId, "Пусто", replyMarkup: _keyBoard, cancellationToken: cancellationToken);

                return;
            }
            else
            {
                var _keyBoard = InlineKeyboardBuilder.BuildInlineKeyboard(items);

                await botClient.SendMessage(
                    chatId,
                    "Выберите сущность",
                    replyMarkup: _keyBoard,
                    cancellationToken: cancellationToken
                );
                return;
            }
        }

        internal static async Task PrintUserItem(
            ITelegramBotClient botClient,
            long chatId,
            string itemMessageText,
            string[] menuItems,
            CancellationToken cancellationToken
        )
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var _keyBoard = ReplyKeyboardBuilder.BuildReplyKeyboard(menuItems);

            await botClient.SendMessage(
                chatId,
                itemMessageText,
                replyMarkup: _keyBoard,
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken
            );
        }
    }
}
