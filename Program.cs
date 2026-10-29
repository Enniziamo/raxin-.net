using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Location
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string AsciiArt { get; set; }
    public List<string> Connections { get; set; } = new List<string>();
}

class Program
{
    static List<string> inventory = new List<string>();
    static string foundItem = "";

    static void Main()
    {
        var locations = new Dictionary<string, Location>
        {
            ["entrance"] = new Location
            {
                Name = "entrance",
                Description = "there's a stick. you stand at the forest entrance. paths lead forward into the woods.",
                AsciiArt = @"
  /\  /\  /\  /\
 /  \/  \/  \/  \
 |   ENTRANCE    |
 |      []       |
  \/  \/  \/  \/
",
                Connections = new List<string> { "riverside", "cavern" }
            },
            ["riverside"] = new Location
            {
                Name = "riverside",
                Description = "you reach the riverside. the sound of water calms you. maybe you can fish here.",
                AsciiArt = @"
 ~ ~ ~ ~ ~ ~ ~ ~
~  RIVERSIDE   ~
 ~ ~ ~ ~ ~ ~ ~ ~
    \_______/
",
                Connections = new List<string> { "entrance", "city", "riverbank" }
            },
            ["city"] = new Location
            {
                Name = "city",
                Description = "you arrive at a noisy city. there’s a chest near a fountain. an old man is here.",
                AsciiArt = @"
  ___  ___  ___
 |___||___||___|
      .---.
     ( o o )
  [ CITY SQUARE ]
       [C]
",
                Connections = new List<string> { "riverside", "beach", "cavern" }
            },
            ["beach"] = new Location
            {
                Name = "beach",
                Description = "you stand on a sunny beach. the sand hides something shiny.",
                AsciiArt = @"
  \_/\_/\_/\_/\_/
   ~ ~ ~ ~ ~ ~ ~
    [ BEACH ]
  .___________.
",
                Connections = new List<string> { "city" }
            },
            ["cavern"] = new Location
            {
                Name = "cavern",
                Description = "you enter a dark cavern. a glowing stone blocks your way.",
                AsciiArt = @"
      ______
   __/      \__
  /   CAVERN   \
 |      **      |
  \____________/
",
                Connections = new List<string> { "riverside" }
            },
            ["church"] = new Location
            {
                Name = "church",
                Description = "a secret church beyond the cavern. a priest stands here, looking sad.",
                AsciiArt = @"
       /\
      /  \
     /____\
    |  ++  |
    |______|
    [ CHURCH ]
",
                Connections = new List<string> { "cavern" }
            },
            ["riverbank"] = new Location
            {
                Name = "riverbank",
                Description = "you stand at the edge of sparkling river. a brown raccoon is here, sipping on a soda. it looks at you curiously.",
                AsciiArt = @"
  /\/\  /\/\  /\/\
 /    \/    \/    \
     [ RIVERBANK ]
       \_____/
",
                Connections = new List<string> { "riverside" }
            }
        };

        var history = new List<string>();
        string currentLocation = "entrance";

        Console.WriteLine("welcome to the adventure!");
        Console.WriteLine("type 'help' to see available commands.\n");

        while (true)
        {
            Location loc = locations[currentLocation];

            Console.WriteLine(loc.AsciiArt);
            Console.WriteLine($"location: {loc.Name}");
            Console.WriteLine(loc.Description);
            Console.Write("\n> ");

            string input = Console.ReadLine()?.ToLower().Trim() ?? "";
            string[] parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                continue;

            string command = parts[0];

            switch (command)
            {
                case "help":
                    Console.WriteLine("\ncommands:");
                    Console.WriteLine("  look          - look around");
                    Console.WriteLine("  go <place>    - move to a connected place (ex: go city)");
                    Console.WriteLine("  interact      - interact with environment");
                    Console.WriteLine("  take <item>   - take an item you've found");
                    Console.WriteLine("  open          - open chests or items");
                    Console.WriteLine("  talk          - talk to people");
                    Console.WriteLine("  inventory     - show your items");
                    Console.WriteLine("  map list      - show map");
                    Console.WriteLine("  exit          - quit game");
                    break;

                case "map":
                case "map list":
                    Console.WriteLine(@"
            [ ENTRANCE ]
             /        \
        [ RIVERSIDE ]--[ CAVERN ]
        /     |    \       |
 [ RIVERBANK ] [ CITY ]  [ CHURCH ]
                 |
             [ BEACH ]
");
                    break;

                case "back":
                    if (history.Count == 0)
                    {
                        Console.WriteLine("there's nowhere to back");
                    }
                    else
                    {
                        string last = history[history.Count - 1];
                        history.RemoveAt(history.Count - 1);
                        currentLocation = last;
                    }
                    break;

                case "go":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("where do you want to go? (example: go city)");
                        continue;
                    }

                    string target = parts[1];

                    if (target == currentLocation)
                    {
                        Console.WriteLine("you are already there.");
                        continue;
                    }

                    if (CanMove(currentLocation, target, locations))
                    {
                        history.Add(currentLocation);
                        currentLocation = target;
                        foundItem = "";
                    }
                    else
                    {
                        Console.WriteLine("you can't go there directly.");
                    }
                    break;

                case "interact":
                    HandleInteract(currentLocation, locations);
                    break;

                case "craft":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("craft what? (example: craft fishingrod)");
                        continue;
                    }

                    string item = parts[1];

                    if (item == "fishingrod")
                    {
                        if (HasItem(inventory, "stick") &&
                            HasItem(inventory, "rope") &&
                            HasItem(inventory, "nail"))
                        {
                            if (!HasItem(inventory, "fishingrod"))
                            {
                                Console.WriteLine("you tie rope to the stick and use the nail as a hook - a fishing rod is made!");
                                AddItem(inventory, "fishingrod");
                            }
                            else
                            {
                                Console.WriteLine("you already made a fishing rod.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("you don't have all materials (need stick, rope, nail)");
                        }
                    }
                    else
                    {
                        Console.WriteLine("you can't craft that.");
                    }
                    break;

                case "look":
                    Console.WriteLine(loc.AsciiArt);
                    Console.WriteLine($"location: {loc.Name}");
                    Console.WriteLine(loc.Description);
                    Console.WriteLine($"-> you can go to {string.Join(", ", loc.Connections)}");
                    break;

                case "take":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("take what?");
                        continue;
                    }

                    string takeItem = parts[1];

                    if (foundItem == takeItem)
                    {
                        Console.WriteLine($"you picked up the {takeItem}!");
                        AddItem(inventory, takeItem);
                        foundItem = "";
                    }
                    else
                    {
                        Console.WriteLine("there's nothing like that to take here.");
                    }
                    break;

                case "open":
                    if (currentLocation == "city")
                    {
                        if (!HasItem(inventory, "hatenna"))
                        {
                            Console.WriteLine("you open the chest and find a pokemon - hatenna joins you!");
                            AddItem(inventory, "hatenna");
                        }
                        else
                        {
                            Console.WriteLine("the chest is empty now.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("there's nothing to open here.");
                    }
                    break;

                case "talk":
                    switch (currentLocation)
                    {
                        case "city":
                            Console.WriteLine("old man: 'i heard there's a tunnel under the glowing stone in the cavern...'");
                            break;

                        case "church":
                            if (HasItem(inventory, "gem"))
                            {
                                Console.WriteLine("priest: 'my gem! thank you, traveler!'");
                                Console.WriteLine("you are blessed with a divine light.");
                                AddItem(inventory, "blessing");
                            }
                            else
                            {
                                Console.WriteLine("priest: 'i lost my precious gem somewhere in the forest...'");
                            }
                            break;

                        default:
                            Console.WriteLine("no one to talk to here.");
                            break;
                    }
                    break;

                case "inventory":
                    if (inventory.Count == 0)
                    {
                        Console.WriteLine("your inventory is empty.");
                    }
                    else
                    {
                        Console.WriteLine("inventory: " + string.Join(", ", inventory));
                    }
                    break;

                case "exit":
                    Console.WriteLine("you exit the game. goodbye!");
                    return;

                default:
                    Console.WriteLine("unknown command. type 'help' to see commands.");
                    break;
            }
        }
    }

    static bool CanMove(string current, string target, Dictionary<string, Location> locations)
    {
        return locations[current].Connections.Contains(target);
    }

    static bool HasItem(List<string> inventory, string item)
    {
        return inventory.Contains(item);
    }

    static void AddItem(List<string> inventory, string item)
    {
        if (!inventory.Contains(item))
        {
            inventory.Add(item);
        }
    }

    static void HandleInteract(string currentLocation, Dictionary<string, Location> locations)
    {
        switch (currentLocation)
        {
            case "entrance":
                if (!HasItem(inventory, "stick"))
                {
                    Console.WriteLine("you pull a sturdy stick from a fallen branch.");
                    AddItem(inventory, "stick");
                }
                else
                {
                    Console.WriteLine("you already took a stick.");
                }
                break;

            case "riverside":
                if (HasItem(inventory, "fishingrod"))
                {
                    if (!HasItem(inventory, "gem"))
                    {
                        Console.WriteLine("something shines under the water...");
                        Console.WriteLine("you cast your fishing rod and pull out a glowing gem!");
                        AddItem(inventory, "gem");
                    }
                    else
                    {
                        Console.WriteLine("the river is calm now.");
                    }
                }
                else
                {
                    Console.WriteLine("you see something shiny in the water, but it's too far to reach by hand.");
                }
                break;

            case "beach":
                if (!HasItem(inventory, "hammer"))
                {
                    Console.WriteLine("you see something shiny buried in the sand. maybe you can 'take hammer'.");
                    foundItem = "hammer";
                }
                else
                {
                    Console.WriteLine("you already took the hammer.");
                }

                if (!HasItem(inventory, "rope"))
                {
                    Console.WriteLine("you find an old rope tangled in the sand.");
                    AddItem(inventory, "rope");
                }
                else
                {
                    Console.WriteLine("the rope is already taken.");
                }
                break;

            case "cavern":
                if (HasItem(inventory, "hammer"))
                {
                    Console.WriteLine("you break the glowing stone! a passage opens to a hidden church!");

                    if (!locations["cavern"].Connections.Contains("church"))
                    {
                        locations["cavern"].Connections.Add("church");
                    }
                }
                else
                {
                    Console.WriteLine("you need something strong to break the stone.");
                }
                break;

            case "church":
                if (!HasItem(inventory, "nail"))
                {
                    Console.WriteLine("you notice a bent nail on the floor and pick it up.");
                    AddItem(inventory, "nail");
                }
                else
                {
                    Console.WriteLine("there's nothing else of interest here.");
                }

                if (HasItem(inventory, "gem"))
                {
                    Console.WriteLine("priest: 'my gem... thank you traveler.'");
                    Thread.Sleep(1000);
                    Console.WriteLine("'FOOL! You freed me!' - the priest transforms into a witcher");
                    Console.WriteLine("the church trembles as darkness spreads. demo ends here.");
                    Console.WriteLine("----------------------------------------------------------");
                    Environment.Exit(0);
                }
                else
                {
                    Console.WriteLine("priest: 'have you found my lost gem?'");
                }
                break;

            default:
                Console.WriteLine("nothing special to interact with here.");
                break;
        }
    }
}