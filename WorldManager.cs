namespace AdventureGame;

public class WorldManager
{
    public void Initialize()
    {
        // Konumları, yolları ve nesneleri başlat
    }

    public string GetLocationDescription()
    {
        return "You are in the starting area.";
    }

    public List<string> GetAvailablePaths()
    {
        return new List<string> { "forest", "cave" };
    }

    public List<string> GetKnownLocationNames()
    {
        return new List<string> { "Starting Area", "Forest" };
    }

    public bool HasUndiscoveredLocations()
    {
        return true;
    }

    public CommandResult Move(string destination)
    {
        return CommandResult.Ok($"You move to {destination}.");
    }

    public CommandResult GoBack()
    {
        return CommandResult.Ok("You return to the previous location.");
    }
}