using System.ComponentModel.DataAnnotations;

namespace dotnet_simple_chat_be.Rooms.Dtos;

public record CreateRoomDto()
{
    [Required]
    [MinLength(3)]
    [MaxLength(32)]
    public string Password { get; set; } = null!;
};