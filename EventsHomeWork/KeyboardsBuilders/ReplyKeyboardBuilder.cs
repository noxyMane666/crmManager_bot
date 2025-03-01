using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace EventsHomeWork.KeyBoardsPrinter
{
    internal class ReplyKeyboardBuilder
    {
        public static ReplyKeyboardMarkup BuildReplyKeyboard(string[] services)
        {
            List<KeyboardButton[]> _buttons = [];

            for (int i = 0; i < services.Length; i += 2)
            {
                if (services.Last() != services[i])
                    _buttons.Add([new KeyboardButton(services[i]), new KeyboardButton(services[i + 1])]);
                else
                    _buttons.Add([new KeyboardButton(services[i])]);
            }

            ReplyKeyboardMarkup replyKeyBoard = new(_buttons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            return replyKeyBoard;
        }
    }
}
