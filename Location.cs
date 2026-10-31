namespace AdventureGame;

public class CommandResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = "";
    public bool GameEnded { get; init; }

    public static CommandResult Ok(string message = "", bool gameEnded = false) =>
        new() { Success = true, Message = message, GameEnded = gameEnded };

    public static CommandResult Fail(string message) =>
        new() { Success = false, Message = message };
}

public class Location
{
    public string Key { get; set; } = "";
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string DescriptionAfterItemsTaken { get; set; } = "";
    public List<string> Connections { get; set; } = new();
    public List<string> Items { get; set; } = new();
}