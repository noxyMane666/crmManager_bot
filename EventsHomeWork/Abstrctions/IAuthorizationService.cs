using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork.Abstrctions
{
    public interface IAuthorizationService
    {
        Task<int> GetUserIdAsync(long telegramUserId, CancellationToken cancellationToken);
    }
}
