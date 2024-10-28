namespace dotnet_simple_chat_be.Common.Errors;

public static class RoomErrors
{
    public static readonly Error PasswordNotMeetRequirements = 
        new Error("InvalidPassword", "Password must be between 3 and 32 characters.");
}