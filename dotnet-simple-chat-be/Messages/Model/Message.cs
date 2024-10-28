using System.ComponentModel.DataAnnotations;
using dotnet_simple_chat_be.Rooms.Model;

namespace dotnet_simple_chat_be.Messages.Model;

public class Message
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    

    public Room Room { get; set; } = null!;
    public string RoomId { get; set; } = null!;

    [Required]
    public string SenderName { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
}