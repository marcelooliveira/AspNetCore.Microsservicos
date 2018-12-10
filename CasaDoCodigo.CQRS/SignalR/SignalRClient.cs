using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using MVC.Model.Redis;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using IdentityModel;
using Microsoft.Extensions.Configuration;

namespace MVC.SignalR
{
    public class SignalRClient : ISignalRClient
    {
        private HubConnection connection;
        private readonly IConfiguration configuration;
        private readonly IUserRedisRepository userRedisRepository;
        private bool started = false;

        public SignalRClient(IConfiguration configuration, IUserRedisRepository userRedisRepository)
        {
            this.configuration = configuration;
            this.userRedisRepository = userRedisRepository;

            connection =
            new HubConnectionBuilder()
                .WithUrl($"{configuration["CallBackUrl"]}usernotificationhub")
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information);
                    logging.AddConsole();
                })
                .Build();
        }

        public async Task Start(string clientId)
        {
            if (started)
            {
                return;
            }
            started = true;

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<string, string>("ReceiveMessage", async (user, message) =>
            {
                int userNotificationCount = await userRedisRepository.GetUserNotificationCountAsync(clientId);
                await userRedisRepository.UpdateUserNotificationCountAsync(clientId, userNotificationCount + 1);
            });

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                //messagesList.Items.Add(ex.Message);
            }
        }
    }
}
