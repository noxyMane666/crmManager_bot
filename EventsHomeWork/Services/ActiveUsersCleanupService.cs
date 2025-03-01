using EventsHomeWork.Abstrctions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EventsHomeWork.Services
{
    public class ActiveUsersCleanupService : BackgroundService
    {
        private readonly IActiveUsersService _activeUsersService;
        private readonly TimeSpan _cleanUpInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _userCleanInterval = TimeSpan.FromMinutes(10);

        public ActiveUsersCleanupService(IActiveUsersService activeUsersService)
        {
            Console.WriteLine("✅ Конструктор ActiveUsersCleanupService вызван!");
            _activeUsersService = activeUsersService ?? throw new ArgumentNullException(nameof(activeUsersService));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            Console.WriteLine("qqw");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    CleanUpInactiveUsers();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Ошибка при очистке неактивных пользователей: {ex.Message}");
                }
                await Task.Delay(_cleanUpInterval, stoppingToken);
            }
        }

        private void CleanUpInactiveUsers()
        {
            var usersToRemove = _activeUsersService.GetAllUsers()
                .Where(user => (DateTime.UtcNow - user.LastActivity) > _userCleanInterval)
                .ToList();
            Console.WriteLine("вызван сервис удаления");
            foreach (var user in usersToRemove)
            {
                _activeUsersService.RemoveUser(user);
                Console.WriteLine($"Пользователь {user.FfUserId} удалён из активных из-за неактивности.");
            }
        }
    }
}
