namespace dotnet_simple_chat_be.Common;

public record MessageResponse
{
    public string Message { get; init; } = null!;
}