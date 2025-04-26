using Microsoft.AspNetCore.SignalR;

public class ChatHub : Hub
{
    // Optional: Add methods like sending to groups or private messaging
    public async Task SendMessageToUser(string receiverId, object message)
    {
        await Clients.User(receiverId).SendAsync("ReceiveMessage", message);
    }
}
