package main

import (
	"bufio"
	"fmt"
	"os"
	"strings"
	"time"
)

type Location struct {
	name        string
	description string
	asciiArt    string
	connections []string
}

func main() {
	scanner := bufio.NewScanner(os.Stdin)
	inventory := []string{}
	foundItem := "" // geçici olarak yerde bulunan item

	locations := map[string]Location{
		"entrance": {
			name:        "entrance",
			description: "there's a stick. you stand at the forest entrance. paths lead forward into the woods.",
			asciiArt: `
🌲🌲🌲🌲🌲
🌲   🚪   🌲
🌲 entrance 🌲
🌲🌲🌲🌲🌲
`,
			connections: []string{"riverside", "cavern"},
		},
		"riverside": {
			name:        "riverside",
			description: "you reach the riverside. the sound of water calms you. maybe you can fish here.",
			asciiArt: `
~~~~~~~~~~~~~
🌿  ~  🌿
🌊 riverside 🌊
~~~~~~~~~~~~~
`,
			connections: []string{"entrance", "city", "riverbank"},
		},
		"city": {
			name:        "city",
			description: "you arrive at a noisy city. there’s a chest near a fountain. an old man is here.",
			asciiArt: `
🏙️🏠🏢🏠🏙️
   ⛲
  (👴)
 city square
`,
			connections: []string{"riverside", "beach", "cavern"},
		},
		"beach": {
			name:        "beach",
			description: "you stand on a sunny beach. the sand hides something shiny.",
			asciiArt: `
🌴🌊🌊🏖️🌊🌊🌴
   🏝️ beach 🏝️
🌴🌊🌊🌴🌊🌊🌴
`,
			connections: []string{"city"},
		},
		"cavern": {
			name:        "cavern",
			description: "you enter a dark cavern. a glowing stone blocks your way.",
			asciiArt: `
   
 🌑 cavern 🌑
  💎 glowing 💎
`,
			connections: []string{"riverside"},
		},
		"church": {
			name:        "church",
			description: "a secret church beyond the cavern. a priest stands here, looking sad.",
			asciiArt: `
⛪🙏⛪
 priest 👨‍
"i lost my gem..."
`,
			connections: []string{"cavern"},
		},
		"riverbank": {
			name: "riverbank",
			description: "you stand at the edge of sparkling river. a brown raccoon is here, sipping on a soda. it looks at you curiously.",
			asciiArt: `🌳🌿🌳🌿🌳
			  🥤
			  riverbank
			🌿🌳🌿🌳🌿`,
			connections: []string{"riverside"},
		},
	}

	var history []string
	currentLocation := "entrance"

	fmt.Println("welcome to the adventure!")
	fmt.Println("type 'help' to see available commands.\n")

	for {
		loc := locations[currentLocation]
		fmt.Println(loc.asciiArt)
		fmt.Printf("📍 location: %s\n%s\n", loc.name, loc.description)
		fmt.Print("\n> ")

		scanner.Scan()
		input := strings.ToLower(strings.TrimSpace(scanner.Text()))
		parts := strings.Fields(input)
		if len(parts) == 0 {
			continue
		}
		command := parts[0]

		switch command {
		case "help":
			fmt.Println("\ncommands:")
			fmt.Println("  look          -look around")
			fmt.Println("  go <place>    - move to a connected place (ex: go city)")
			fmt.Println("  interact      - interact with environment")
			fmt.Println("  take <item>   - take an item you’ve found")
			fmt.Println("  open          - open chests or items")
			fmt.Println("  talk          - talk to people")
			fmt.Println("  inventory     - show your items")
			fmt.Println("  map list      - show map")
			fmt.Println("  exit          - quit game")

		case "map", "map list":
			fmt.Println(`
🗺️   map:

entrance
cavern
riverside
riverbank
city
beach
`)

		case "back":
			if len(history) == 0 {
				fmt.Println("there's nowhere to back")
			} else {
				last := history[len(history)-1]
				history = history[:len(history)-1]
				currentLocation = last
			}

		case "go":
			if len(parts) < 2 {
				fmt.Println("where do you want to go? (example: go city)")
				continue
			}
			target := parts[1]
			if target == currentLocation {
				fmt.Println("you are already there.")
				continue
			}
			if canMove(currentLocation, target, locations) {
				history = append(history, currentLocation)
				currentLocation = target
				foundItem = ""
			} else {
				fmt.Println("you can’t go there directly.")
			}

		case "interact":
			handleInteract(currentLocation, &inventory, &foundItem, locations)

		case "craft":
			if len(parts) < 2 {
				fmt.Println("craft what? (example: craft fishingrod)")
				continue
			}
			item := parts[1]
			if item == "fishingrod" {
				if hasItem(inventory, "stick") && hasItem(inventory, "rope") && hasItem(inventory, "nail") {
					if !hasItem(inventory, "fishingrod") {
						fmt.Println("🎣 you tie rope to the stick and use the nail as a hook - a fishing rod is made!")
						addItem(&inventory, "fishingrod")
					} else {
						fmt.Println("you already made a fishing rod.")
					}
				} else {
					fmt.Println("you don't have all materials (need stick, rope, nail)")
				}
			} else {
				fmt.Println("you can't craft that.")
			}

		case "look":
			fmt.Println(loc.asciiArt)
			fmt.Printf("📍 location: %s\n%s\n", loc.name, loc.description)
			fmt.Printf("-> you can go to %s\n", strings.Join(loc.connections, ", "))

		case "take":
			if len(parts) < 2 {
				fmt.Println("take what?")
				continue
			}
			item := parts[1]
			if foundItem == item {
				fmt.Printf("you picked up the %s!\n", item)
				addItem(&inventory, item)
				foundItem = ""
			} else {
				fmt.Println("there’s nothing like that to take here.")
			}

		case "open":
			if currentLocation == "city" {
				if !hasItem(inventory, "hatenna") {
					fmt.Println("you open the chest and find a pokemon — hatenna joins you!")
					addItem(&inventory, "hatenna")
				} else {
					fmt.Println("the chest is empty now.")
				}
			} else {
				fmt.Println("there’s nothing to open here.")
			}

		case "talk":
			switch currentLocation {
			case "city":
				fmt.Println("👴 old man: 'i heard there’s a tunnel under the glowing stone in the cavern…'")
			case "church":
				if hasItem(inventory, "gem") {
					fmt.Println("👨‍🦳 priest: 'my gem! thank you, traveler!'")
					fmt.Println("✨ you are blessed with a divine light.")
					addItem(&inventory, "blessing")
				} else {
					fmt.Println("👨‍🦳 priest: 'i lost my precious gem somewhere in the forest…'")
				}
			default:
				fmt.Println("no one to talk to here.")
			}

		case "inventory":
			if len(inventory) == 0 {
				fmt.Println("your inventory is empty.")
			} else {
				fmt.Println("inventory:", inventory)
			}

		case "exit":
			fmt.Println("you exit the game. goodbye!")
			os.Exit(0)

		default:
			fmt.Println("unknown command. type 'help' to see commands.")
		}
	}
}

// yardımcı fonksiyonlar
func canMove(current, target string, locations map[string]Location) bool {
	for _, c := range locations[current].connections {
		if c == target {
			return true
		}
	}
	return false
}

func hasItem(inventory []string, item string) bool {
	for _, i := range inventory {
		if i == item {
			return true
		}
	}
	return false
}

func addItem(inventory *[]string, item string) {
	if !hasItem(*inventory, item) {
		*inventory = append(*inventory, item)
	}
}

func handleInteract(currentLocation string, inventory *[]string, foundItem *string, locations map[string]Location) {
	switch currentLocation {
	case "entrance":
		if !hasItem(*inventory, "stick") {
			fmt.Println("🌿 you pull a sturdy stick from a fallen branch.")
			addItem(inventory, "stick")
		} else {
			fmt.Println("you already took a stick")
		}
	case "riverside":
		if hasItem(*inventory, "fishingrod") {
			if !hasItem(*inventory, "gem") {
				fmt.Println("✨ something shines under the water...")
				fmt.Println("🎣 you cast your fishing rod and pull out a glowing gem!")
				addItem(inventory, "gem")
			} else {
				fmt.Println("the river is calm now.")
			}
		} else {
			fmt.Println("💧 you see something shiny in the water, but its too far to reach by hand.")
		}
	case "beach":
		if !hasItem(*inventory, "hammer") {
			fmt.Println("you see something shiny buried in the sand. maybe you can 'take hammer'.")
			*foundItem = "hammer"
		} else {
			fmt.Println("you already took the hammer.")
		}
		if !hasItem(*inventory, "rope") {
			fmt.Println("you find an old rope tangled in the sand")
			addItem(inventory, "rope")
		} else {
			fmt.Println("the rope is already taken.")
		}
	case "cavern":
		if hasItem(*inventory, "hammer") {
			fmt.Println("💥 you break the glowing stone! a passage opens to a hidden church!")
			loc := locations["cavern"]
			loc.connections = append(loc.connections, "church")
			locations["cavern"] = loc
		} else {
			fmt.Println("you need something strong to break the stone.")
		}
	case "church":
		if !hasItem(*inventory, "nail") {
			fmt.Println("🔩 you notice a bent nail on the floor and pick it up.")
			addItem(inventory, "nail")
		} else {
			fmt.Println("there's nothing else of interest here.")
		}
		if hasItem(*inventory, "gem") {
			fmt.Println("👩‍priest: 'my gem... thank you traveler.'")
			time.Sleep(1 * time.Second)
			fmt.Println("🧙‍'FOOL You freed me!' - the priest transforms into a witcher")
			fmt.Println("🔥 The church trembles as darkness spreads. Demo ends here.")
			fmt.Println("----------------------------------------------------------")
			os.Exit(0)
		} else {
			fmt.Println("👩‍priest: 'have you found my lost gem?'")
		}
	default:
		fmt.Println("nothing special to interact with here.")
	}
}
