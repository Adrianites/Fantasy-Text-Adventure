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

class Program
{
    static void Main(string[] args)
    {
        // Adjusted Initialization for smaller screen size
        const int screenWidth = 1500; // Adjusted screen width
        const int screenHeight = 600; // Adjusted screen height
        Raylib.InitWindow(screenWidth, screenHeight, "Fantasy Text Adventure");
        Raylib.SetTargetFPS(60);

        // Define locations
        var locations = new List<Location>
        {
            new Location("You are in a mystical village surrounded by ancient trees. A glowing portal stands to the east.") { Actions = { { "Enter the Portal", 1 }, { "Go West to the Forest", 2 }} }, 
            new Location("You step into a shimmering realm filled with vibrant colors and floating islands. A wise sage awaits you.") { Actions = { { "Talk to the Sage", 3 }, { "Return to Village", 0 } } },
            new Location("The forest is alive with the sound of chirping birds and rustling leaves. There's a path leading deeper.") { Actions = { { "Follow the Path", 4 }, { "Return to Village", 0 } } },
            new Location("You encounter a majestic waterfall. A hidden cave is behind it, glistening in the sunlight.") { Actions = { { "Explore the Cave", 5 }, { "Go Back", 2 } } },
            new Location("Inside the cave, you discover a magical artifact glowing softly. It's the Crystal of Destiny!") { Actions = { { "Take the Crystal", 6 }, { "Leave the Cave", 3 } } },
            new Location("The sage reveals that the Crystal of Destiny can grant one wish, but it comes with a price. What will you wish for?") { Actions = { { "Wish for Power", 7 }, { "Wish for Peace", 8 }, { "Return to the Village", 0 } } },
            new Location("You become a powerful sorcerer, feared and respected across the land. But loneliness creeps in.") { Actions = { { "End Journey", 0 } } },
            new Location("You restore harmony to the realm, bringing joy and unity. A new era begins!") { Actions = { { "End Journey", 0 } } },
            
        };

        int currentLocation = 0; // Start in the village
        int selectedActionIndex = 0; // Initialize selection index
        int scrollOffset = 0; // Initialize scroll offset for text visibility

        // Main game loop
        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            // Calculate the total text height for scrolling
            int totalTextHeight = locations[currentLocation].Actions.Count * 30 + 40; // 30 pixels per action, plus some margin

            // Adjust scrollOffset based on the total text height and screen height
            if (totalTextHeight > screenHeight)
            {
            if (Raylib.IsKeyPressed(KeyboardKey.Up))
            {
                scrollOffset = Math.Min(scrollOffset + 30, 0); // Scroll up
            }
            else if (Raylib.IsKeyPressed(KeyboardKey.Down))
            {
                scrollOffset = Math.Max(scrollOffset - 30, screenHeight - totalTextHeight); // Scroll down
            }
        }

        // Navigate through actions
        if (Raylib.IsKeyPressed(KeyboardKey.Up))
        {
            selectedActionIndex = (selectedActionIndex - 1 + locations[currentLocation].Actions.Count) % locations[currentLocation].Actions.Count;
        }   
        else if (Raylib.IsKeyPressed(KeyboardKey.Down))
        {
            selectedActionIndex = (selectedActionIndex + 1) % locations[currentLocation].Actions.Count;
        }

        // Select an action and change location
        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
        {
            var action = locations[currentLocation].Actions.ElementAt(selectedActionIndex);
            currentLocation = action.Value;
            selectedActionIndex = 0; // Reset selection index for the new location
            scrollOffset = 0; // Reset scroll offset for the new location
        }

        // Draw location description with scrolling
        Raylib.DrawText(locations[currentLocation].Description, 20, 20 + scrollOffset, 20, Color.SkyBlue);

        // Draw actions and highlight the selected action
        int posY = 60; // Start drawing actions below the description
        foreach (var action in locations[currentLocation].Actions)
        {
            Color textColor = locations[currentLocation].Actions.ElementAt(selectedActionIndex).Key == action.Key ? Color.Yellow : Color.White;
            Raylib.DrawText(action.Key, 20, posY + scrollOffset, 20, textColor);
            posY += 30;
        }

        Raylib.EndDrawing();
        }
    }
}