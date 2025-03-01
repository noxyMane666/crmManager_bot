using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventsHomeWork
{
    internal static class InlineKeyboardBuilder
    {
        public static InlineKeyboardMarkup BuildInlineKeyboard(List<IItem> items)
        {
            var _inlineKeyboardButtons = new List<List<InlineKeyboardButton>>();

            foreach (var item in items)
            {
                var button = new InlineKeyboardButton
                {
                    Text = item.ItemText.ToString(),
                    CallbackData = item.ItemId.ToString()
                };

                _inlineKeyboardButtons.Add([button]);
            }

            return new InlineKeyboardMarkup(_inlineKeyboardButtons);
        }
    }
}
