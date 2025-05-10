//using Microsoft.AspNetCore.SignalR;

//public class ChatHub : Hub
//{
//    // Optional: Add methods like sending to groups or private messaging
//    public async Task SendMessageToUser(string receiverId, object message)
//    {
//        await Clients.User(receiverId).SendAsync("ReceiveMessage", message);
//    }
//}

using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    public static Dictionary<int, string> UserConnections = new();

    public override Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userIdString = httpContext?.Request.Query["userId"];

        if (int.TryParse(userIdString, out int userId))
        {
            UserConnections[userId] = Context.ConnectionId;
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var user = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (!user.Equals(default(KeyValuePair<int, string>)))
        {
            UserConnections.Remove(user.Key);
        }

        return base.OnDisconnectedAsync(exception);
    }
}
