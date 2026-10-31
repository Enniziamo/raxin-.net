namespace AdventureGame;

public class WorldManager
{
    private readonly Dictionary<string, Location> locations = new();
    private readonly Stack<string> locationHistory = new();

    public string CurrentLocation { get; private set; } = "entrance";

    public void Initialize()
    {
        locations.Clear();
        locationHistory.Clear();

        locations["entrance"] = new Location
        {
            Key = "entrance",
            Name = "Forest Entrance",
            Description = "You are at the entrance of a mysterious forest.",
            Connections = { "riverside", "beach", "cavern", "city" }
        };

        locations["riverside"] = new Location
        {
            Key = "riverside",
            Name = "Riverside",
            Description = "A quiet river flows beside the path.",
            Connections = { "entrance" }
        };

        locations["beach"] = new Location
        {
            Key = "beach",
            Name = "Beach",
            Description = "Golden sand stretches toward the sea.",
            Items = { "hammer", "rope" },
            Connections = { "entrance" }
        };

        locations["cavern"] = new Location
        {
            Key = "cavern",
            Name = "Cavern",
            Description = "A glowing stone blocks a dark passage.",
            Items = { "glowing stone" },
            Connections = { "entrance" }
        };

        locations["city"] = new Location
        {
            Key = "city",
            Name = "Old City",
            Description = "Ruined buildings surround an old chest.",
            Connections = { "entrance", "church" }
        };

        locations["church"] = new Location
        {
            Key = "church",
            Name = "Church",
            Description = "A priest stands silently near the altar.",
            Connections = { "city" }
        };

        CurrentLocation = "entrance";
    }

    public string GetLocationDescription()
    {
        return GetLocation().Description;
    }

    public List<string> GetAvailablePaths()
    {
        return GetLocation().Connections;
    }

    public List<string> GetKnownLocationNames()
    {
        return locations.Values
            .Select(location => location.Name)
            .ToList();
    }

    public bool HasUndiscoveredLocations()
    {
        return true;
    }

    public CommandResult Move(string destination)
    {
        if (string.IsNullOrWhiteSpace(destination))
            return CommandResult.Fail("Where do you want to go?");

        destination = destination.ToLowerInvariant();

        var current = GetLocation();

        if (!current.Connections.Contains(destination))
            return CommandResult.Fail($"You can't go to {destination} from here.");

        if (!locations.ContainsKey(destination))
            return CommandResult.Fail("That location does not exist.");

        locationHistory.Push(CurrentLocation);
        CurrentLocation = destination;

        return CommandResult.Ok($"You move to {locations[destination].Name}.");
    }

    public CommandResult GoBack()
    {
        if (locationHistory.Count == 0)
            return CommandResult.Fail("There is nowhere to go back to.");

        CurrentLocation = locationHistory.Pop();

        return CommandResult.Ok($"You return to {GetLocation().Name}.");
    }

    public bool LocationHasItem(string item)
    {
        item = item.ToLowerInvariant();
        return GetLocation().Items.Contains(item);
    }

    public bool TryTakeItem(string item)
    {
        item = item.ToLowerInvariant();
        return GetLocation().Items.Remove(item);
    }

    public CommandResult BreakGlowingStone()
    {
        if (CurrentLocation != "cavern")
            return CommandResult.Fail("There is no glowing stone here.");

        if (!LocationHasItem("glowing stone"))
            return CommandResult.Fail("The glowing stone is already broken.");

        TryTakeItem("glowing stone");

        return CommandResult.Ok("You break the glowing stone. A hidden passage opens!");
    }

    private Location GetLocation()
    {
        return locations[CurrentLocation];
    }
}