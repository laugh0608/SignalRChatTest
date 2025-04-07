using Microsoft.AspNetCore.SignalR;

namespace SignalRChatTest.Hubs
{
    // ChatHub 类继承自 SignalRHub，Hub 类管理连接、组和消息。
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}