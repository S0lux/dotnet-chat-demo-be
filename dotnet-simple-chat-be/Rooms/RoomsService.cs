using dotnet_simple_chat_be.Common;
using dotnet_simple_chat_be.Data;
using dotnet_simple_chat_be.Rooms.Dtos;
using dotnet_simple_chat_be.Rooms.Model;

namespace dotnet_simple_chat_be.Rooms;

public class RoomsService(ChatContext context)
{
    public async Task<Result> CreateRoomsAsync(CreateRoomDto createRoomDto)
    {
        var roomId = StringGenerator.GenerateRandomRoomId();
        var room = new Room { Id = roomId, Password = createRoomDto.Password };
        
        await context.Rooms.AddAsync(room);
        await context.SaveChangesAsync();

        return Result.Success();
    }
}