using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using static EventsHomeWork.Models.ChangeTaskDateForm;

namespace EventsHomeWork.Abstrctions
{
    public interface IUserForm : IAnswerable
    {
        Step CurrentStep { get; }

        bool IsCompleted { get; }

        string GetFormData(string userMessage);

    }
}
