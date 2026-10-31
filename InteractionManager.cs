namespace AdventureGame;

public class InteractionManager
{
    private readonly WorldManager world;
    private readonly InventoryManager inventory;

    public InteractionManager(WorldManager world, InventoryManager inventory)
    {
        this.world = world;
        this.inventory = inventory;
    }

    public CommandResult Interact()
    {
        switch (world.CurrentLocation)
        {
            case "riverside":
                return InteractRiverside();

            case "beach":
                return InteractBeach();

            case "cavern":
                return InteractCavern();

            case "church":
                return InteractChurch();

            default:
                return CommandResult.Fail("There's nothing to interact with here.");
        }
    }

    private CommandResult InteractRiverside()
    {
        if (!inventory.HasItem("fishingrod"))
            return CommandResult.Fail(
                "Something shines in the water, but you can't reach it.");

        return inventory.AddItem(
            "gem",
            "You fish a glowing gem out of the river!");
    }

    private CommandResult InteractBeach()
    {
        var messages = new List<string>();

        if (!world.LocationHasItem("hammer"))
            messages.Add("You see a hammer lying in the sand. Try: take hammer");

        if (!world.LocationHasItem("rope"))
            messages.Add("You see an old rope lying in the sand. Try: take rope");

        if (messages.Count == 0)
            return CommandResult.Fail("There's nothing here to interact with.");

        return CommandResult.Ok(string.Join("\n", messages));
    }

    private CommandResult InteractCavern()
    {
        if (!inventory.HasItem("hammer"))
            return CommandResult.Fail(
                "You need something strong to break the stone.");

        return world.BreakGlowingStone();
    }

    private CommandResult InteractChurch()
    {
        if (!inventory.HasItem("gem"))
            return CommandResult.Fail("There's nothing to interact with here.");

        return CommandResult.Ok(
            "Priest: 'My gem... thank you, traveler.'\n" +
            "'FOOL! You freed me!' The priest transforms.\n" +
            "Darkness spreads. Demo ends here.",
            gameEnded: true);
    }

    public CommandResult TakeItem(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
            return CommandResult.Fail("What do you want to take?");

        item = item.ToLowerInvariant();

        if (!world.LocationHasItem(item))
            return CommandResult.Fail($"There's no {item} here to take.");

        world.TryTakeItem(item);

        return inventory.AddItem(item, $"You picked up a {item}!");
    }

    public CommandResult Craft(string item)
    {
        if (string.IsNullOrWhiteSpace(item))
            return CommandResult.Fail("What do you want to craft?");

        item = item.ToLowerInvariant();

        if (item != "fishingrod")
            return CommandResult.Fail("You can't craft that.");

        if (!inventory.HasItem("stick") ||
            !inventory.HasItem("rope") ||
            !inventory.HasItem("nail"))
        {
            return CommandResult.Fail("You need a stick, rope and nail.");
        }

        return inventory.AddItem(
            "fishingrod",
            "You combine the stick, rope and nail to make a fishing rod!");
    }

    public CommandResult OpenChest()
    {
        if (world.CurrentLocation != "city")
            return CommandResult.Fail("There's nothing to open here.");

        return inventory.AddItem(
            "hatenna",
            "You open the chest. Hatenna joins you!");
    }

    public CommandResult Talk()
    {
        switch (world.CurrentLocation)
        {
            case "entrance":
                return CommandResult.Ok(
                    "You talk to yourself. It's a lonely forest.");

            case "city":
                return CommandResult.Ok(
                    "Old man: 'There's a tunnel under the glowing stone in the cavern...'");

            case "church":
                return TalkToPriest();

            default:
                return CommandResult.Fail("No one to talk to here.");
        }
    }

    private CommandResult TalkToPriest()
    {
        if (!inventory.HasItem("gem"))
            return CommandResult.Ok(
                "Priest: 'I lost my precious gem in the forest...'");

        var addResult = inventory.AddItem("blessing");

        if (!addResult.Success)
            return addResult;

        return CommandResult.Ok(
            "Priest: 'My gem! Thank you, traveler!'\n" +
            addResult.Message);
    }
}