using EventsHomeWork.Abstrctions;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventsHomeWork
{
    public class ActiveUsersService : IActiveUsersService
    {
        private readonly ConcurrentDictionary<int, User> _activeUsers = new();

        public void AddUser(User user)
        {

            _activeUsers[user.FfUserId] = user;
        }

        public void RemoveUser(User user)
        {
            _activeUsers.TryRemove(user.FfUserId, out _);
        }

        public User? GetUserById(int userId)
        {
            _activeUsers.TryGetValue(userId, out var user);
            return user;
        }

        public User GetOrCreateUser(long telegramUserId, int ffUserId, long chatId)
        {
            
            if (!_activeUsers.TryGetValue(ffUserId, out var user))
            {
                user = new User(telegramUserId, ffUserId, chatId);
                AddUser(user);
            }
            return user;
        }
        public IEnumerable<User> GetAllUsers()
        {
            return _activeUsers.Values;
        }

    }
}
