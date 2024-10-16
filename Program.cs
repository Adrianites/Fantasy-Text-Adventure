using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;

struct Location
{
    public string Description;
    public Dictionary<string, int> Actions; // Action text and target location ID

    public Location(string description)
    {
        Description = description;
        Actions = new Dictionary<string, int>();
    }
}

class Item
{
    public string Name { get; set; }
    public int HealingAmount { get; set; } // Amount of health the item heals

    public Item(string name, int healingAmount = 0)
    {
        Name = name;
        HealingAmount = healingAmount;
    }
}

class Player
{
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int AttackPower { get; set; }
    public bool HasSword { get; set; }
    public List<Item> Inventory { get; set; } // New property to track inventory

    public Player(int health, int attackPower)
    {
        Health = health;
        MaxHealth = health; // Set max health
        AttackPower = attackPower;
        HasSword = false;
        Inventory = new List<Item>(); // Initialize inventory
    }

    public void Attack(Enemy enemy)
    {
        enemy.Health -= AttackPower;
    }

    public void Heal(int amount)
    {
        Health = Math.Min(Health + amount, MaxHealth); // Heal but do not exceed max health
        Console.WriteLine($"You have been healed by {amount} points. Current health: {Health}");
    }

    public void AddItem(Item item)
    {
        Inventory.Add(item);
        Console.WriteLine($"You have acquired a {item.Name}!");
    }

    public void UseItem(string itemName)
    {
        var item = Inventory.FirstOrDefault(i => i.Name == itemName);
        if (item != null)
        {
            if (item.Name == "Potion of Healing")
            {
                Heal(item.HealingAmount);
                Inventory.Remove(item);
                Console.WriteLine($"You used a {item.Name}.");
            }
            else if (item.Name == "Sword of Valor")
            {
                HasSword = true;
                Inventory.Remove(item);
                Console.WriteLine($"You equipped the {item.Name}.");
            }
            // Add more item usage logic as needed
        }
        else
        {
            Console.WriteLine($"You do not have a {itemName} in your inventory.");
        }
    }
}

class Enemy
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int AttackPower { get; set; }

    public Enemy(string name, int health, int attackPower)
    {
        Name = name;
        Health = health;
        AttackPower = attackPower;
    }

    public void Attack(Player player)
    {
        player.Health -= AttackPower;
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Adjusted Initialization for smaller screen size
        const int screenWidth = 1500; // Adjusted screen width
        const int screenHeight = 600; // Adjusted screen height
        Raylib.InitWindow(screenWidth, screenHeight, "Fantasy Text Adventure");
        Raylib.SetTargetFPS(60);

        // Define player
        Player player = new Player(100, 10);

        // Define enemies
        var enemies = new List<Enemy>
        {
            new Enemy("Goblin", 30, 5),
            new Enemy("Orc", 50, 10),
            new Enemy("Dragon", 200, 50)
        };

        // Define items
        var items = new List<Item>
        {
            new Item("Crystal of Destiny"),
            new Item("Sword of Valor"),
            new Item("Shield of Light"),
            new Item("Potion of Healing", 20) // Heals 20 health points
        };

        // Define locations
        var locations = new List<Location>
        {
            new Location("You are in a mystical village surrounded by ancient trees. A glowing portal stands to the east.") 
            { 
                Actions = 
                { 
                    { "Enter the Portal", 1 }, 
                    { "Go West to the Forest", 2 }, 
                    { "Check Inventory", -1 }, 
                    { "Check Health", -2 },
                    { "Search Area", -3 },
                    { "Talk to Villager", -4 }
                } 
            }, 
            new Location("You step into a shimmering realm filled with vibrant colors and floating islands. A wise sage awaits you.") 
            { 
                Actions = 
                { 
                    { "Talk to the Sage", 3 }, 
                    { "Return to Village", 0 }, 
                    { "Check Inventory", -1 }, 
                    { "Check Health", -2 },
                    { "Search Area", -3 }
                } 
            },
            // Add similar actions to other locations...
        };

        int currentLocation = 0; // Start in the village
        int selectedActionIndex = 0; // Initialize selection index
        int selectedItemIndex = 0; // Initialize inventory selection index

        // Main game loop
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Draw the current location description
            Raylib.DrawText(locations[currentLocation].Description, 10, 10, 20, Color.RayWhite);

            // Draw the available actions
            int yOffset = 50;
            foreach (var action in locations[currentLocation].Actions)
            {
                Color actionColor = (selectedActionIndex == yOffset / 30 - 1) ? Color.Yellow : Color.RayWhite;
                Raylib.DrawText(action.Key, 10, yOffset, 20, actionColor);
                yOffset += 30;
            }

            // Handle user input for navigation
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                if (locations[currentLocation].Actions.Count > 0)
                {
                    selectedActionIndex = (selectedActionIndex + 1) % locations[currentLocation].Actions.Count;
                }
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                if (locations[currentLocation].Actions.Count > 0)
                {
                    selectedActionIndex = (selectedActionIndex - 1 + locations[currentLocation].Actions.Count) % locations[currentLocation].Actions.Count;
                }
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                if (locations[currentLocation].Actions.Count > 0)
                {
                    var action = locations[currentLocation].Actions.ElementAt(selectedActionIndex);
                    if (action.Value == -1) // Check Inventory
                    {
                        DisplayInventory(player);
                    }
                    else if (action.Value == -2) // Check Health
                    {
                        DisplayHealth(player);
                    }
                    else if (action.Value == -3) // Search Area
                    {
                        SearchArea(player);
                    }
                    else if (action.Value == -4) // Talk to Villager
                    {
                        TalkToVillager(player);
                    }
                    else
                    {
                        currentLocation = action.Value;

                        // Check for battles
                        if (currentLocation == 9) // Goblin encounter
                        {
                            Battle(player, enemies[0]);
                        }
                        else if (currentLocation == 10) // Orc encounter
                        {
                            Battle(player, enemies[1]);
                        }
                        else if (currentLocation == 11) // Dragon encounter
                        {
                            Battle(player, enemies[2]);
                        }
                        else if (currentLocation == 12) // Sword of Valor
                        {
                            player.AddItem(new Item("Sword of Valor"));
                        }
                        else if (currentLocation == 13) // Potion of Healing
                        {
                            player.AddItem(new Item("Potion of Healing", 20));
                        }
                    }
                }
            }

            // Handle user input for inventory navigation
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                if (player.Inventory.Count > 0)
                {
                    selectedItemIndex = (selectedItemIndex + 1) % player.Inventory.Count;
                }
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                if (player.Inventory.Count > 0)
                {
                    selectedItemIndex = (selectedItemIndex - 1 + player.Inventory.Count) % player.Inventory.Count;
                }
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Backspace))
            {
                if (player.Inventory.Count > 0 && selectedItemIndex >= 0 && selectedItemIndex < player.Inventory.Count)
                {
                    player.UseItem(player.Inventory[selectedItemIndex].Name);
                }
            }

            Raylib.EndDrawing();
        }
    }

    static void DisplayInventory(Player player)
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            int yOffset = 50;
            Raylib.DrawText("Inventory:", 10, yOffset, 20, Color.RayWhite);
            yOffset += 30;
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                Raylib.DrawText(player.Inventory[i].Name, 10, yOffset, 20, Color.RayWhite);
                yOffset += 30;
            }
            Raylib.EndDrawing();

            // Wait for user input to go back to the main game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                break;
            }
        }
    }

    static void DisplayHealth(Player player)
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawText($"Health: {player.Health}/{player.MaxHealth}", 10, 50, 20, Color.RayWhite);
            Raylib.EndDrawing();

            // Wait for user input to go back to the main game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                break;
            }
        }
    }

    static void SearchArea(Player player)
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawText("You search the area and find nothing of interest.", 10, 50, 20, Color.RayWhite);
            Raylib.EndDrawing();

            // Wait for user input to go back to the main game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                break;
            }
        }
    }

    static void TalkToVillager(Player player)
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawText("You talk to the villager. They have no useful information.", 10, 50, 20, Color.RayWhite);
            Raylib.EndDrawing();

            // Wait for user input to go back to the main game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Escape))
            {
                break;
            }
        }
    }

    static void Battle(Player player, Enemy enemy)
    {
        if (player.HasSword)
        {
            Console.WriteLine($"You have the Sword of Valor! You easily defeat the {enemy.Name}.");
            enemy.Health = 0; // Enemy is defeated
        }
        else
        {
            while (player.Health > 0 && enemy.Health > 0)
            {
                // Player attacks first
                player.Attack(enemy);
                Console.WriteLine($"You attack the {enemy.Name} for {player.AttackPower} damage. {enemy.Name} has {enemy.Health} health left.");

                if (enemy.Health <= 0)
                {
                    Console.WriteLine($"You defeated the {enemy.Name}!");
                    return;
                }

                // Enemy attacks back
                enemy.Attack(player);
                Console.WriteLine($"The {enemy.Name} attacks you for {enemy.AttackPower} damage. You have {player.Health} health left.");

                if (player.Health <= 0)
                {
                    Console.WriteLine("You have been defeated!");
                    return;
                }
            }
        }
    }
}