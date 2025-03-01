using EventsHomeWork.Abstrctions;
using EventsHomeWork.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork
{
    public interface IItem : IAnswerable
    {
        int ItemId { get; }
        string ItemText { get; }
        int ItemTaskId { get; }
        string ItemTaskText { get; }
        public string GetItemTextMessage();
    }
}
