using dotnet_simple_chat_be.Messages.Model;
using dotnet_simple_chat_be.Rooms.Model;
using Microsoft.EntityFrameworkCore;

namespace dotnet_simple_chat_be.Data;

public class ChatContext(DbContextOptions<ChatContext> options) : DbContext(options)
{
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Message> Messages { get; set; }
}