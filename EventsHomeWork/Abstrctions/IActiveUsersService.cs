using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork.Abstrctions
{
    public interface IActiveUsersService
    {
        void AddUser(User user);
        void RemoveUser(User user);
        User? GetUserById(int userId);
        User GetOrCreateUser(long telegramUserId, int ffUserId, long chatId);
        IEnumerable<User> GetAllUsers();
    }
}
