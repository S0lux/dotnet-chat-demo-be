using dotnet_simple_chat_be.Messages.Model;

namespace dotnet_simple_chat_be.Rooms.Model;

public class Room
{
    public string Id { get; set; }
    // This should be hashed btw
    public string Password { get; set; }
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}