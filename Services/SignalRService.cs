using LynxUI_Main.Helpers;
using LynxUI_Main.Models;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Diagnostics;

namespace LynxUI_Main.Services
{
    public class SignalRService
    {
        private HubConnection _connection;

        public event Action<MessageItem> OnReceiveMessage;

        public SignalRService() { }

        public async Task ConnectAsync(string serverUrl, int currentUserId)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(serverUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(TokenStorage.LoadToken());
                })
                .WithAutomaticReconnect()
                .Build();

            // Bắt sự kiện WebSocket bị đóng
            _connection.Closed += async (error) =>
            {
                Console.WriteLine("[SignalR] Connection closed.");
                if (error != null)
                    Console.WriteLine("[SignalR] Close error: " + error.Message);

                await Task.Delay(3000);
                try
                {
                    await _connection.StartAsync();
                    Console.WriteLine("[SignalR] Reconnected.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[SignalR] Reconnect failed: " + ex.Message);
                }
            };

            // Đăng ký handler khi server gửi tin nhắn
            _connection.On<ApiMessageDto>("ReceiveMessage", apiMsg =>
            {
                var item = MessageMapper.MapFromApiMessage(apiMsg, currentUserId);
                OnReceiveMessage?.Invoke(item);
            });

            await _connection.StartAsync();
            Console.WriteLine("[SignalR] Connected.");
        }


        public async Task SendMessageAsync(MessageItem message)
        {
            try
            {
                var apiMsg = MessageMapper.MapToApiMessage(message);

                System.Diagnostics.Debug.WriteLine("[SendMessage] apiMsg = " + JsonConvert.SerializeObject(apiMsg));

                if (_connection.State != HubConnectionState.Connected)
                {
                    System.Diagnostics.Debug.WriteLine("[SendMessage] Connection not active.");
                    return;
                }
                Debug.WriteLine("[SendMessage] Raw apiMsg:\n" + JsonConvert.SerializeObject(apiMsg, Formatting.Indented));

                await _connection.InvokeAsync("SendMessage", apiMsg);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[SendMessage ERROR] " + ex.Message);
            }
        }

        public async Task DisconnectAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }
        }
    }
}
