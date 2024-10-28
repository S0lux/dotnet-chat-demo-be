using dotnet_simple_chat_be.Common;
using dotnet_simple_chat_be.Rooms.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_simple_chat_be.Rooms
{
    [Route("v1/rooms")]
    [ApiController]
    public class RoomsController(RoomsService roomsService) : ControllerBase
    {
        [HttpPost()]
        public async Task<ActionResult<MessageResponse>> CreateRoom(CreateRoomDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await roomsService.CreateRoomsAsync(dto);
            if (result.IsFailure) return StatusCode(500, result.Error);
            return new MessageResponse { Message = "Room created successfully." };
        }
    }
}
