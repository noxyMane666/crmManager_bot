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
        private readonly ILogger<ActiveUsersCleanupService> _logger;
        private readonly IActiveUsersService _activeUsersService;
        private readonly TimeSpan _cleanUpInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _userCleanInterval = TimeSpan.FromMinutes(10);

        public ActiveUsersCleanupService(IActiveUsersService activeUsersService, ILogger<ActiveUsersCleanupService> logger)
        {
            _activeUsersService = activeUsersService ?? throw new ArgumentNullException(nameof(activeUsersService));
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    CleanUpInactiveUsers();
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при очистке неактивных пользователей");
                }
                await Task.Delay(_cleanUpInterval, stoppingToken);
            }
        }

        private void CleanUpInactiveUsers()
        {
            var usersToRemove = _activeUsersService.GetAllUsers()
                .Where(user => (DateTime.UtcNow - user.LastActivity) > _userCleanInterval)
                .ToList();

            foreach (var user in usersToRemove)
            {
                _activeUsersService.RemoveUser(user);
                _logger.LogInformation($"Пользователь {user.FfUserId} удалён из активных из-за неактивности.");
            }
        }
    }
}
