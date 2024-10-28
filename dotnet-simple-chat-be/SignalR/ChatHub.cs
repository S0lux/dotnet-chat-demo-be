using System.Collections.Concurrent;
using dotnet_simple_chat_be.Data;
using dotnet_simple_chat_be.Messages.Model;
using dotnet_simple_chat_be.Rooms.Model;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace dotnet_simple_chat_be.SignalR;

public class ChatHub : Hub
{
    private readonly ChatContext _context;
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _rooms = new();
    
    public ChatHub(ChatContext context)
    {
        _context = context;
    }
    
    public async Task JoinRoom(string roomCode, string displayName)
    {
        // Leave any current room first
        await LeaveCurrentRoom();
        
        // Ensure room exists in database
        var room = await _context.Rooms
            .FirstOrDefaultAsync(r => r.Id == roomCode);
            
        if (room == null)
        {
            room = new Room { Id = roomCode };
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
        }
        
        // Add user to the room's user dictionary
        var roomUsers = _rooms.GetOrAdd(roomCode, _ => new ConcurrentDictionary<string, string>());
        roomUsers.TryAdd(Context.ConnectionId, displayName);
        
        // Add to SignalR group
        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
        
        // Store room code in connection context
        Context.Items["RoomCode"] = roomCode;
        Context.Items["DisplayName"] = displayName;
        
        // Notify others in the room
        await Clients.Group(roomCode).SendAsync("UserJoined", displayName);
        
        // Send current user list to the joining user
        var users = roomUsers.Values.ToList();
        await Clients.Caller.SendAsync("UserList", users);
        
        // Send message history
        var messageHistory = await _context.Messages
            .Where(m => m.RoomId == roomCode)
            .OrderBy(m => m.Timestamp)
            .Take(50) // Limit to last 50 messages
            .Select(m => new
            {
                m.SenderName,
                m.Content,
                m.Timestamp
            })
            .ToListAsync();
            
        await Clients.Caller.SendAsync("MessageHistory", messageHistory);
    }
    
    public async Task SendMessage(string message)
    {
        var roomCode = Context.Items["RoomCode"] as string;
        var displayName = Context.Items["DisplayName"] as string;
        
        if (string.IsNullOrEmpty(roomCode) || string.IsNullOrEmpty(displayName))
            return;
            
        // Create and save message to database
        var newMessage = new Message
        {
            RoomId = roomCode,
            SenderName = displayName,
            Content = message,
            Timestamp = DateTime.UtcNow
        };
        
        _context.Messages.Add(newMessage);
        await _context.SaveChangesAsync();
        
        // Broadcast message to all users in the room
        await Clients.Group(roomCode).SendAsync("ReceiveMessage", new
        {
            sender = displayName,
            message = message,
            timestamp = newMessage.Timestamp
        });
    }
    
    private async Task LeaveCurrentRoom()
    {
        var currentRoom = Context.Items["RoomCode"] as string;
        var displayName = Context.Items["DisplayName"] as string;
        
        if (string.IsNullOrEmpty(currentRoom))
            return;
            
        if (_rooms.TryGetValue(currentRoom, out var roomUsers))
        {
            roomUsers.TryRemove(Context.ConnectionId, out _);
            
            // Remove room from memory if empty (but keep in database)
            if (roomUsers.IsEmpty)
                _rooms.TryRemove(currentRoom, out _);
        }
        
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, currentRoom);
        await Clients.Group(currentRoom).SendAsync("UserLeft", displayName);
        
        Context.Items["RoomCode"] = null;
        Context.Items["DisplayName"] = null;
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await LeaveCurrentRoom();
        await base.OnDisconnectedAsync(exception);
    }
}