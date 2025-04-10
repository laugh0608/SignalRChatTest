using Microsoft.AspNetCore.SignalR;

namespace SignalRChatTest.Hubs
{
    // ChatHub 类继承自 SignalRHub，Hub 类管理连接、组和消息
    // 使用 SendAsync 的缺点在于，它依赖于字符串来指定要调用的客户端方法
    // public class ChatHub : Hub
    // {
    //     // SendMessage 使用 Clients.All 将消息发送到所有连接的客户端
    //     public async Task SendMessage(string user, string message)
    //         => await Clients.All.SendAsync("ReceiveMessage", user, message);
    //     // SendMessageToCaller 使用 Clients.Caller 将消息发送回调用方
    //     public async Task SendMessageToCaller(string user, string message)
    //         => await Clients.Caller.SendAsync("ReceiveMessage", user, message);
    //     // SendMessageToGroup 将消息发送给 SignalR Users 组中的所有客户端
    //     public async Task SendMessageToGroup(string user, string message)
    //         => await Clients.Group("SignalR Users").SendAsync("ReceiveMessage", user, message);
    // }
    
    // 使用 SendAsync 的替代方法是使用 Hub<T> 将 Hub 类设为强类型
    public interface IChatClient
    {
        Task ReceiveMessage(string user, string message);
    }
    
    // 添加密钥服务，其是指使用密钥注册和检索依赖项注入 (DI) 服务的机制
    public interface ICache
    {
        object Get(string key);
    }
    public class BigCache : ICache
    {
        public object Get(string key) => $"Resolving {key} from big cache.";
    }
    public class SmallCache : ICache
    {
        public object Get(string key) => $"Resolving {key} from small cache.";
    }
    
    // 通过声明从 Hub 继承的类来创建中心。 将 public 方法添加到类，使其可从客户端调用
    // 此接口可用于将上面的 ChatHub 示例使用泛型重构为强类型：
    // 对客户端方法进行编译时检查，可防止由于使用字符串而导致的问题
    public class StronglyTypedChatHub : Hub<IChatClient>
    {
        // 发送到所有连接的客户端
        public async Task SendMessage(string user, string message)
            => await Clients.All.ReceiveMessage(user, message);

        // 发送回调用方
        public async Task SendMessageToCaller(string user, string message)
            => await Clients.Caller.ReceiveMessage(user, message);

        // 发送给 SignalR Users 组中的所有客户端
        public async Task SendMessageToGroup(string user, string message)
            => await Clients.Group("SignalR Users").ReceiveMessage(user, message);
        
        // 添加密钥服务，其是指使用密钥注册和检索依赖项注入 (DI) 服务的机制
        // 使用 [FromKeyedServices] 属性指定密钥来访问已注册的服务
        public void SmallCacheMethod([FromKeyedServices("small")] ICache cache)
        {
            Console.WriteLine(cache.Get("signalr"));
        }
        public void BigCacheMethod([FromKeyedServices("big")] ICache cache)
        {
            Console.WriteLine(cache.Get("signalr"));
        }
        
        // SignalR 中心 API 提供 OnConnectedAsync 和 OnDisconnectedAsync 虚拟方法来管理和跟踪连接
        // 替代 OnConnectedAsync 虚拟方法可在客户端连接到中心时执行操作
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnConnectedAsync();
        }
        // 替代 OnDisconnectedAsync 虚拟方法可在客户端断开连接时执行操作
        // 如果客户端有意断开连接（例如通过调用 connection.stop()），则 exception 参数将设置为 null
        // 但是，如果客户端由于错误（例如网络故障）而断开连接，则 exception 参数将包含描述故障的异常
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}