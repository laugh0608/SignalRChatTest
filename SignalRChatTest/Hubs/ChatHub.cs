using Microsoft.AspNetCore.SignalR;

namespace SignalRChatTest.Hubs
{
    // ChatHub 类继承自 SignalRHub，Hub 类管理连接、组和消息
    // 使用 SendAsync 的缺点在于，它依赖于字符串来指定要调用的客户端方法
    // public class ChatHub : Hub
    // {
    //     // SendMessage 使用 Clients.All 将消息发送到所有连接的客户端
    //     public async Task SendMessage(string user, string message)
    //     {
    //         await Clients.All.SendAsync("ReceiveMessage", user, message);
    //     }
    //     
    //     // SendMessageToCaller 使用 Clients.Caller 将消息发送回调用方
    //     public async Task SendMessageToCaller(string user, string message)
    //         => await Clients.Caller.SendAsync("ReceiveMessage", user, message);
    //     
    //     // SendMessageToGroup 将消息发送给 SignalR Users 组中的所有客户端
    //     public async Task SendMessageToGroup(string user, string message)
    //         => await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);
    // }
    
    // 使用 SendAsync 的替代方法是使用 Hub<T> 将 Hub 类设为强类型
    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message);
    }
    // 此接口可用于将上面的 ChatHub 示例重构为强类型：
    public class StronglyTypedChatHub : Hub<IChatClient>
    {
        public async Task SendMessage(string user, string message)
            => await Clients.All.ReceiveMessage(user, message);

        public async Task SendMessageToCaller(string user, string message)
            => await Clients.Caller.ReceiveMessage(user, message);

        public async Task SendMessageToGroup(string user, string message)
            => await Clients.Group("SignalR Users").ReceiveMessage(user, message);
    }
}