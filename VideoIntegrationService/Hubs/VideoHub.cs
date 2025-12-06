using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VideoIntegrationService.Data;
using VideoIntegrationService.Models;

namespace VideoIntegrationService.Hubs
{
    public class VideoHub : Hub
    {
        private readonly AppDbContext _db;

        public VideoHub(AppDbContext db)
        {
            _db = db;
        }

        // sessionId used as group name (Guid string)
        public async Task SendMessage(string sessionId, int senderId, string senderName, string message)
        {
            // Persist chat message (optional)
            if (Guid.TryParse(sessionId, out var sid))
            {
                var chat = new ChatMessage
                {
                    Id = Guid.NewGuid(),
                    SessionId = sid,
                    SenderId = senderId,
                    SenderName = senderName,
                    Message = message,
                    SentAt = DateTime.UtcNow
                };

                _db.ChatMessages.Add(chat);
                await _db.SaveChangesAsync();
            }

            await Clients.Group(sessionId).SendAsync("ReceiveMessage", new
            {
                sessionId,
                senderId,
                senderName,
                message,
                sentAt = DateTime.UtcNow
            });
        }

        public override async Task OnConnectedAsync()
        {
            var http = Context.GetHttpContext();
            var sessionId = http?.Request.Query["sessionId"].ToString();
            var userId = http?.Request.Query["userId"].ToString();

            if (!string.IsNullOrEmpty(sessionId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
                await Clients.Group(sessionId).SendAsync("UserJoined", new { sessionId, userId });
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var http = Context.GetHttpContext();
            var sessionId = http?.Request.Query["sessionId"].ToString();
            var userId = http?.Request.Query["userId"].ToString();

            if (!string.IsNullOrEmpty(sessionId))
            {
                await Clients.Group(sessionId).SendAsync("UserLeft", new { sessionId, userId });
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
