using EventsHomeWork.Abstrctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork.Workers
{
    public class AuthApiResponse : ApiResponse
    {
        public int UserId { get; set; } = 0;
    }
}
