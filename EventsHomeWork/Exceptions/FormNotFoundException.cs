using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork.Exceptions
{
    public class FormNotFoundException(string message) : ObjectNotFoundException(message)
    {
    }
}
