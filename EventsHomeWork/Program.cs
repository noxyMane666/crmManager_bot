
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using static Telegram.Bot.TelegramBotClient;
using Telegram.Bot.Types;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EventsHomeWork.StaticServices;
using EventsHomeWork.DB;
using EventsHomeWork.Abstrctions;
using EventsHomeWork.Services;
using EventsHomeWork.Workers;
using EventsHomeWork.Handlers;

namespace EventsHomeWork
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    var path = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
                    config.AddJsonFile(path, optional: false, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {

                    services.AddHostedService<BotService>();
                    services.AddHostedService<ActiveUsersCleanupService>();
                    services.AddSingleton<UpdateHandler>();
                    services.AddSingleton<IErrorHandler, ErrorHandler>();
                    services.AddSingleton<IActiveUsersService, ActiveUsersService>();
                    services.AddTransient<ICallbackHandler, CallbackHandler>();
                    services.AddTransient<AnswerWorker>();
                    services.AddTransient<TasksWorker>();
                    services.AddTransient<QuestionWorker>();
                    services.AddTransient<CommentWorker>();
                    services.AddTransient<ICommandHandler, CommandHandler>();
                    services.AddTransient<IMessageHandler, MessageHandler>();
                    services.AddTransient<IItemService, ItemActionService>();
                    services.AddTransient<IAuthorizationService, AuthorizationService>();
                    services.AddTransient<IWelcomeMenuService, WelcomeMenu>();
                    services.AddLogging(logging =>
                    {
                        logging.ClearProviders();
                        logging.AddConsole();
                        logging.AddProvider(new DatabaseLoggerProvider(
                            context.Configuration.GetConnectionString("DefaultConnection") 
                            ?? throw new InvalidOperationException("No connection string")
                        ));
                    });
                });
    }
}
