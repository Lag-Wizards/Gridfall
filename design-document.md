Architecture:
The architecture for Gridfall is primarily event-driven with the components of the system communicating through events. Examples of events that occur within the system are key inputs that trigger character movement and the character's health reaching zero triggers a game over screen.

Data Model:
Units are the base class for characters and enemies which store values such as their statistics and currently equipped weapon, which is it's own class storing the weapon's statistics. Save data is another class used for storing the required data for a player's character to be loaded to its saved state. Tile states store what type of terrain it is, the cost of movement for that tile, and what unit is occupying the tile, if any.

Technical Decisions:
Godot with C# support is the engine used for this project. This was chosen as Godot is free to use, supported our game concept, and allowed for direct C# usage, which our group was familiar with. Save data is stored as a JSON file in the default location for Godot save files, referred to as user:// in the engine but stored at a specific folder on the user's device (e.g. C:\Users\<username>\AppData\Roaming\Godot\app_userdata\Gridfall by default on Windows). This was chosen as the game standard for game saves are in a file format, and JSON is a relatively simple format to convert C# classes to and from files.