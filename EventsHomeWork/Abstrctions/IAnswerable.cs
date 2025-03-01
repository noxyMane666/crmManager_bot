using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork.Abstrctions
{
    public interface IAnswerable
    {
        string? AnswerMessage { get; set; }
        string GetFormRequestBody(int currentUserId);
    }
}
