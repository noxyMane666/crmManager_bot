using EventsHomeWork.Abstrctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork.Workers
{
    class AnswerApiResposnse : ApiResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}
