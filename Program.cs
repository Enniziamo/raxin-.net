using AdventureGame;

class Program
{
    static void Main(string[] args)
    {
        var world = new WorldManager();
        var inventory = new InventoryManager();
        var interactions = new InteractionManager(world, inventory);

        world.Initialize();

        Console.WriteLine("Welcome to the adventure!");
        Console.WriteLine("Type 'help' to see available commands.");

        bool gameRunning = true;

        while (gameRunning)
        {
            ShowLocation(world);

            Console.Write("\n> ");
            string input = Console.ReadLine()?.Trim().ToLowerInvariant() ?? "";

            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                continue;

            string command = parts[0];
            string argument = parts.Length > 1 ? parts[1] : "";

            CommandResult result;

            switch (command)
            {
                case "help":
                    ShowHelp();
                    continue;

                case "look":
                    Console.WriteLine(world.GetLocationDescription());
                    Console.WriteLine("Paths: " + string.Join(", ", world.GetAvailablePaths()));
                    continue;

                case "map":
                    result = ShowMap(world, argument);
                    break;

                case "go":
                    result = world.Move(argument);
                    break;

                case "back":
                    result = world.GoBack();
                    break;

                case "interact":
                    result = interactions.Interact();
                    break;

                case "take":
                    result = interactions.TakeItem(argument);
                    break;

                case "craft":
                    result = interactions.Craft(argument);
                    break;

                case "open":
                    result = interactions.OpenChest();
                    break;

                case "talk":
                    result = interactions.Talk();
                    break;

                case "inventory":
                    result = ShowInventory(inventory);
                    break;

                case "exit":
                    Console.WriteLine("Goodbye!");
                    return;

                default:
                    result = CommandResult.Fail("Unknown command. Type 'help'.");
                    break;
            }

            Console.WriteLine(result.Message);

            if (result.GameEnded)
            {
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
                gameRunning = false;
            }
        }
    }

    static void ShowLocation(WorldManager world)
    {
        Console.WriteLine(world.GetLocationDescription());
    }

    static void ShowHelp()
    {
        Console.WriteLine(
            "Available commands:\n" +
            "- look\n" +
            "- map list\n" +
            "- go <location>\n" +
            "- back\n" +
            "- interact\n" +
            "- take <item>\n" +
            "- craft <item>\n" +
            "- open\n" +
            "- talk\n" +
            "- inventory\n" +
            "- exit");
    }

    static CommandResult ShowMap(WorldManager world, string argument)
    {
        if (argument != "list" && argument != "")
            return CommandResult.Fail("Usage: map list");

        var lines = new List<string> { "Known locations:" };
        lines.AddRange(world.GetKnownLocationNames().Select(name => $"- {name}"));

        if (world.HasUndiscoveredLocations())
            lines.Add("Some locations remain undiscovered.");

        return CommandResult.Ok(string.Join("\n", lines));
    }

    static CommandResult ShowInventory(InventoryManager inventory)
    {
        if (inventory.Items.Count == 0)
            return CommandResult.Ok("Your inventory is empty.");

        return CommandResult.Ok(
            "Inventory:\n" +
            string.Join("\n", inventory.Items.Select(item => $"- {item}")));
    }
}