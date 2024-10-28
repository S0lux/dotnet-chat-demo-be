namespace dotnet_simple_chat_be.Common;

public class StringGenerator
{
    private static readonly Random Random = new Random();
    private const string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateRandomRoomId(int partLength = 3)
    {
        if (partLength <= 0)
            throw new ArgumentException("Length must be greater than 0", nameof(partLength));

        var firstPart = new char[partLength];
        var secondPart = new char[partLength];

        for (int i = 0; i < partLength; i++)
        {
            firstPart[i] = Characters[Random.Next(Characters.Length)];
            secondPart[i] = Characters[Random.Next(Characters.Length)];
        }

        return $"{new string(firstPart)}-{new string(secondPart)}";
    }
}