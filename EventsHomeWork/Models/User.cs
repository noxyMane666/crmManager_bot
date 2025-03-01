using EventsHomeWork.Abstrctions;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork
{
    public class User(long tgUserId, int ffUserId, long chatId)
    {
        public long TelegramUserId { get; private set; } = tgUserId;
        public int FfUserId { get; private set; } = ffUserId;
        public long ChatId { get; private set; } = chatId;
        public bool IsAnswerRequested { get; private set; } = false;
        public EventServiceTypes CurrentEventType { get; private set; }
        public List<IItem> UserItems { get; private set; } = [];
        public IItem? ChoosenItem { get; private set; }
        public IUserForm? UserForm { get; private set; }
        public IWorker? CurrentWorker { get; private set; }
        public DateTime LastActivity { get; private set; }

        public void SetUserItems<T>(List<T>? userItems) where T : IItem
        {
            if (userItems is null) return;

            UserItems = [.. userItems];
        }

        public void SetChoosenItem(IItem item)
        {
            ChoosenItem = item;
        }

        public void SetUserForm(IUserForm userForm)
        {
            UserForm = userForm;
        }

        public void SetUserWorker(IWorker worker)
        {
            CurrentWorker = worker;
        }

        public void UpdateActivity()
        {
            LastActivity = DateTime.UtcNow;
        }

        public void SetUserEventType(EventServiceTypes serviceType)
        {
            CurrentEventType = serviceType;
        }

        public void SetAnswerRequest(bool value)
        {
            IsAnswerRequested = value;
        }
    }
}
