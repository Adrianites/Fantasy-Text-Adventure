using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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

class Level
{
    public string Description { get; set; }
    public Dictionary<string, int> Actions { get; set; } // Action name and the ID of the next level

    public Level(string description)
    {
        Description = description;
        Actions = new Dictionary<string, int>();
    }

    public void AddAction(string actionName, int nextLevelId)
    {
        Actions[actionName] = nextLevelId;
    }
}

class LevelManager
{
    private Dictionary<int, Level> levels;
    public int CurrentLevelId { get; private set; }

    public LevelManager()
    {
        levels = new Dictionary<int, Level>();
        CurrentLevelId = 0;
    }

    public void AddLevel(int levelId, Level level)
    {
        levels[levelId] = level;
    }

    public Level GetCurrentLevel()
    {
        return levels[CurrentLevelId];
    }

    public void MoveToLevel(int levelId)
    {
        if (levels.ContainsKey(levelId))
        {
            CurrentLevelId = levelId;
        }
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

        // Define the Integer Values for easier reference
        // Locations
        int StartingVillage = 0;
        int PortalStart = 1;
        int TheForest = 2;
        int TheForestGoblinArena = 3;
        int DarkCaveEntrance = 4;
        int DarkCavePit = 5;
        int DarkCaveDragonArena = 6;
        int TheCavernEntrance = 7;
        int TheCavern = 8;


        // Items
        int SwordOfValor = 20;
        int PotionOfHealing = 21;

        // NPCs
        int TalkToSagePortal = 31;
        

        // Enemies
        int GoblinEncounter = 40;
        int OrcEncounter = 41;
        int DragonEncounter = 42;

        // HUD
        int CheckInventory = 50;
        int CheckHealth = 51;

        //Actions
        int TalkToTheVillager = 60;
        int Pray = 61;
        int Shout = 62;
        int SearchCurrentArea = 63;
        int EndYourSuffering = 64;




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
            new Item("Lesser Potion of Healing", 20), // Heals 20 health points
            new Item("Potion of Healing", 50), // Heals 50 health points
            new Item("Greater Potion of Healing", 100) // Heals 100 health points
        };

        // Define level manager
        LevelManager levelManager = new LevelManager();

        // Define levels
        var village = new Level("You wake up in a mystical village surrounded by ancient trees. A glowing portal stands to the east, while a dense forest lies to the west.");
        village.AddAction("Enter the Portal", PortalStart);
        village.AddAction("Go West to the Forest", TheForest);
        village.AddAction("Talk to Villager", TalkToTheVillager);

        village.AddAction("Search Area", SearchCurrentArea);
        village.AddAction("Pray", Pray);
        village.AddAction("Shout", Shout);
        village.AddAction("Check Inventory", CheckInventory);
        village.AddAction("Check Health", CheckHealth);
        village.AddAction("End your Suffering...", EndYourSuffering);



        var portal = new Level("You step into a shimmering realm filled with vibrant colors and floating islands. A wise sage awaits you.");
        portal.AddAction("Talk to the Sage", TalkToSagePortal);
        portal.AddAction("Return to Village", StartingVillage);

        portal.AddAction("Search Area", SearchCurrentArea);
        portal.AddAction("Pray", Pray);
        portal.AddAction("Shout", Shout);
        portal.AddAction("Check Inventory", CheckInventory);
        portal.AddAction("Check Health", CheckHealth);
        portal.AddAction("End your Suffering...", EndYourSuffering);

        

        var forest = new Level("You find yourself in a dense forest. A path north leads to a clearing where a goblin is lurking. To the east, a dark cave entrance beckons.");
        forest.AddAction("Continue walking down the Path", TheForestGoblinArena);
        forest.AddAction("Go East towards the Cave", DarkCaveEntrance);
        forest.AddAction("Return to Village", StartingVillage);

        forest.AddAction("Search Area", SearchCurrentArea);
        forest.AddAction("Pray", Pray);
        forest.AddAction("Shout", Shout);
        forest.AddAction("Check Inventory", CheckInventory);
        forest.AddAction("Check Health", CheckHealth);
        forest.AddAction("End your Suffering...", EndYourSuffering);



        var forestGoblinArena = new Level("You get too close to the goblin and it attacks you!");
        forestGoblinArena.AddAction("Fight the Goblin", GoblinEncounter);
        forestGoblinArena.AddAction("Run away", TheForest);

        forestGoblinArena.AddAction("Search Area", SearchCurrentArea);
        forestGoblinArena.AddAction("Pray", Pray);
        forestGoblinArena.AddAction("Shout", Shout);
        forestGoblinArena.AddAction("Check Inventory", CheckInventory);
        forestGoblinArena.AddAction("Check Health", CheckHealth);
        forestGoblinArena.AddAction("End your Suffering...", EndYourSuffering);



        var darkCave = new Level("You enter the dark cave. A faint light glimmers in the distance.");
        darkCave.AddAction("Continue towards the Light", SwordOfValor);
        darkCave.AddAction("Leave the cave, it's too scary...", TheForest);
        darkCave.AddAction("There seems to be a pit on the left, should you go down?", DarkCavePit);

        darkCave.AddAction("Search Area", SearchCurrentArea);
        darkCave.AddAction("Pray", Pray);
        darkCave.AddAction("Shout", Shout);
        darkCave.AddAction("Check Inventory", CheckInventory);
        darkCave.AddAction("Check Health", CheckHealth);
        darkCave.AddAction("End your Suffering...", EndYourSuffering);



        var darkCavePit = new Level("You descend into the pit and find a sleeping dragon. It's guarding a treasure chest.");
        darkCavePit.AddAction("Attack the Dragon", DarkCaveDragonArena);
        darkCavePit.AddAction("Go back up the shaft", DarkCaveEntrance);
        darkCavePit.AddAction("Try to sneak past the Dragon", TheCavernEntrance);

        darkCavePit.AddAction("Search Area", SearchCurrentArea);
        darkCavePit.AddAction("Pray", Pray);
        darkCavePit.AddAction("Shout", Shout);
        darkCavePit.AddAction("Check Inventory", CheckInventory);
        darkCavePit.AddAction("Check Health", CheckHealth);
        darkCavePit.AddAction("End your Suffering...", EndYourSuffering);



        var darkCaveDragonArena = new Level("You wake the dragon and it attacks you!");
        darkCaveDragonArena.AddAction("Fight the Dragon", DragonEncounter);
        darkCaveDragonArena.AddAction("Run far away", DarkCaveEntrance);

        darkCaveDragonArena.AddAction("Search Area", SearchCurrentArea);
        darkCaveDragonArena.AddAction("Pray", Pray);
        darkCaveDragonArena.AddAction("Shout", Shout);
        darkCaveDragonArena.AddAction("Check Inventory", CheckInventory);
        darkCaveDragonArena.AddAction("Check Health", CheckHealth);
        darkCaveDragonArena.AddAction("End your Suffering...", EndYourSuffering);



        var theCavernEntrance = new Level("You sneak past the dragon leaving the treaure behind, you find yourself in a massive cavern.");
        theCavernEntrance.AddAction("Explore the Cavern", TheCavern);
        theCavernEntrance.AddAction("Return to the Dragons layer", DarkCavePit);
        
        theCavernEntrance.AddAction("Search Area", SearchCurrentArea);
        theCavernEntrance.AddAction("Pray", Pray);
        theCavernEntrance.AddAction("Shout", Shout);
        theCavernEntrance.AddAction("Check Inventory", CheckInventory);
        theCavernEntrance.AddAction("Check Health", CheckHealth);
        theCavernEntrance.AddAction("End your Suffering...", EndYourSuffering);



        var theCavern = new Level("You explore the cavern and find a hidden exit..");



        // Add levels to the level manager
        levelManager.AddLevel(0, village);
        levelManager.AddLevel(1, portal);
        levelManager.AddLevel(2, forest);
        levelManager.AddLevel(3, forestGoblinArena);
        levelManager.AddLevel(4, darkCave);
        levelManager.AddLevel(5, darkCavePit);
        levelManager.AddLevel(6, darkCaveDragonArena);
        levelManager.AddLevel(7, theCavernEntrance);
        levelManager.AddLevel(8, theCavern);

        int selectedActionIndex = 0; // Initialize selection index

        // Show title screen
        ShowTitleScreen();

        // Main game loop
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Get the current node
            var currentLevel = levelManager.GetCurrentLevel();

            // Draw the current location description
            DrawCenteredText(currentLevel.Description, 10, 20, Color.RayWhite);

            // Draw the available actions
            int yOffset = 50;
            foreach (var action in currentLevel.Actions)
            {
                Color actionColor = (selectedActionIndex == yOffset / 30 - 1) ? Color.Yellow : Color.RayWhite;
                DrawCenteredText(action.Key, yOffset, 20, actionColor);
                yOffset += 30;
            }

            // Handle user input for navigation
            if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                if (currentLevel.Actions.Count > 0)
                {
                    selectedActionIndex = (selectedActionIndex + 1) % currentLevel.Actions.Count;
                }
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                if (currentLevel.Actions.Count > 0)
                {
                    selectedActionIndex = (selectedActionIndex - 1 + currentLevel.Actions.Count) % currentLevel.Actions.Count;
                }
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                if (currentLevel.Actions.Count > 0)
                {
                    var action = currentLevel.Actions.ElementAt(selectedActionIndex);
                    if (action.Value == CheckInventory) // Check Inventory
                    {
                        DisplayInventory(player);
                    }
                    else if (action.Value == CheckHealth) // Check Health
                    {
                        DisplayHealth(player);
                    }
                    else if (action.Value == SearchCurrentArea) // Search Area
                    {
                        SearchArea(player);
                    }
                    else if (action.Value == TalkToTheVillager) // Talk to Villager
                    {
                        TalkToVillager(player);
                    }
                    else if (action.Value == Pray)
                    {
                        StartPraying(player);
                    }
                    else if (action.Value == Shout)
                    {
                        StartShouting(player);
                    }
                    else if (action.Value == EndYourSuffering)
                    {
                        StopSuffering(player);
                    }
                    
                    {
                        levelManager.MoveToLevel(action.Value);

                        // Check for battles
                        if (action.Value == GoblinEncounter) // Goblin encounter
                        {
                            StartBattle(player, enemies[0]);
                        }
                        else if (action.Value == OrcEncounter) // Orc encounter
                        {
                            StartBattle(player, enemies[1]);
                        }
                        else if (action.Value == DragonEncounter) // Dragon encounter
                        {
                            StartBattle(player, enemies[2]);
                        }
                        else if (action.Value == SwordOfValor) // Sword of Valor
                        {
                            player.AddItem(new Item("Sword of Valor"));
                        }
                        else if (action.Value == PotionOfHealing) // Potion of Healing
                        {
                            player.AddItem(new Item("Potion of Healing", 20));
                        }
                    }
                }
            }

            Raylib.EndDrawing();
        }
    }

    static void ShowTitleScreen()
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Draw title with shaking effect
            DrawShakingText("Fantasy Text Adventure", Raylib.GetScreenWidth() / 2, 200, 40, Color.RayWhite);
            DrawShakingText("Created by Adrian", Raylib.GetScreenWidth() / 2, 300, 20, Color.RayWhite);

            // Draw start button
            DrawCenteredText("Press ENTER to Start Dreaming..", 400, 20, Color.Red);

            Raylib.EndDrawing();

            // Wait for user to press ENTER to start the game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                break;
            }
        }
    }

    static void DrawShakingText(string text, int centerX, int posY, int fontSize, Color color)
    {
        Random random = new Random();
        int textWidth = Raylib.MeasureText(text, fontSize);
        int startX = centerX - textWidth / 2;

        for (int i = 0; i < text.Length; i++)
        {
            int offsetX = random.Next(-1, 1); 
            int offsetY = random.Next(-1, 1); 
            Raylib.DrawText(text[i].ToString(), startX + i * fontSize + offsetX, posY + offsetY, fontSize, color);
        }
    }

static void DrawCenteredText(string text, int posY, int fontSize, Color color)
{
    int textWidth = Raylib.MeasureText(text, fontSize);
    int posX = (Raylib.GetScreenWidth() - textWidth) / 2;
    Raylib.DrawText(text, posX, posY, fontSize, color);
}
    static void DisplayInventory(Player player)
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);
            int yOffset = 50;
            DrawCenteredText("Inventory:", yOffset, 20, Color.RayWhite);
            yOffset += 30;
            for (int i = 0; i < player.Inventory.Count; i++)
            {
                DrawCenteredText(player.Inventory[i].Name, yOffset, 20, Color.RayWhite);
                yOffset += 30;
            }
            Raylib.EndDrawing();

            // Wait for user input to go back to the main game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Backspace))
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

            // Center the health text
            string healthText = $"Health: {player.Health}/{player.MaxHealth}";
            DrawCenteredText(healthText, 50, 20, Color.RayWhite);

            Raylib.EndDrawing();

            // Wait for user input to go back to the main game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Backspace))
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

            // Center the search area text
            string searchText = "You search the area and find nothing of interest.";
            DrawCenteredText(searchText, 50, 20, Color.RayWhite);

            Raylib.EndDrawing();

            // Wait for user input to go back to the main game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Backspace))
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

            // Center the talk to villager text
            string talkText = "You talk to the villager. They have no useful information.";
            DrawCenteredText(talkText, 50, 20, Color.RayWhite);

            Raylib.EndDrawing();

            // Wait for user input to go back to the main game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Backspace))
            {
                break;
            }
        }
    }

    static void StopSuffering(Player player)
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Center the death text
            string deathText = "You successfully ended your Suffering! Press ENTER to dream once again...";
            DrawCenteredText(deathText, 50, 20, Color.RayWhite);

            Raylib.EndDrawing();

            // Wait for user input to restart the game
            if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                // Reset the player's health
                player.Health = player.MaxHealth;
                break;
            }
        }
    }
    static void StartPraying(Player player)
    {
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

                // Center the praying text
            {
                string talkText = "You start praying. You feel a sense of calm and peace.";
                DrawCenteredText(talkText, 50, 20, Color.RayWhite);

                Raylib.EndDrawing();

                // Wait for user input to go back to the main game
                if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Backspace))
                {
                    break;
                }
            }
        }
    }

    static void StartShouting(Player player)
    {
        string userInput =  "";
        string resultText = "";

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Center the shouting prompt text
            string promptText = "What do you want to shout?";
            DrawCenteredText(promptText, 200, 20, Color.RayWhite);

            // Draw the user input
            DrawCenteredText(userInput, 250, 20, Color.Red);

            // Draw the result text
            if (!string.IsNullOrEmpty(resultText))
            {
                DrawCenteredText(resultText, 300, 20, Color.RayWhite);
            }

            Raylib.EndDrawing();

            // Handle user input
            int key = Raylib.GetKeyPressed();
            if (key >= 32 && key <= 126) // Printable characters
            {
                userInput += (char)key;
            }
            else if (key == 259 && userInput.Length > 0) // Backspace
            {
                userInput = userInput.Substring(0, userInput.Length - 1);
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Enter))
            {
                // Check the user input and perform the corresponding action
                resultText = CheckShout(userInput);
                userInput = ""; // Clear the input after checking
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Backspace))
            {
                break; // Exit the shouting mode
            }
        }
    }

    static string CheckShout(string input)
    {
        // Define the predefined words and corresponding actions
        var shoutActions = new Dictionary<string, string>
        {
            { "help", "You hear a distant voice that leads you back home.."},
            { "attack", "You prepare for battle!" },
            { "run", "You run away quickly!" },
            { "heal", "You use a healing potion!" },
            { "magic", "You cast a powerful spell!" }
        };

        // Check if the input matches any predefined word
        if (shoutActions.ContainsKey(input.ToLower()))
        {
            return shoutActions[input.ToLower()];
        }
        else
        {
            return "Nothing happens...";
        }
    }


    static void StartBattle(Player player, Enemy enemy)
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
                    string promptText = "You died! Press ENTER to dream once again...";
                    DrawCenteredText(promptText, 200, 20, Color.RayWhite);
                    Raylib.EndDrawing();
                    return;
                }
            }
        }
    }
}