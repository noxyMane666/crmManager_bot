using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork.Exceptions
{
    internal class WorkerNotFoundException(string message) : ObjectNotFoundException(message)
    {
    }
}
