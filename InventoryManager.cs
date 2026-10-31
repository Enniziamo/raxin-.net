namespace AdventureGame;

public class InventoryManager
{
    private readonly List<string> items = new();

    public IReadOnlyList<string> Items => items;

    public bool HasItem(string item) =>
        items.Contains(item.ToLowerInvariant());

    public CommandResult AddItem(string item, string message = "")
    {
        item = item.ToLowerInvariant();

        if (HasItem(item))
            return CommandResult.Fail($"You already have a {item}.");

        items.Add(item);

        return CommandResult.Ok(
            string.IsNullOrEmpty(message)
                ? $"You received a {item}."
                : message);
    }

    public CommandResult RemoveItem(string item)
    {
        item = item.ToLowerInvariant();

        if (!HasItem(item))
            return CommandResult.Fail($"You don't have a {item}.");

        items.Remove(item);

        return CommandResult.Ok($"You lost your {item}.");
    }
}