Roles
Product Owner - Adam Pastorok Email: Apastorok@gmail.com
Scrum Master - Tanner Gleason
Developers - Fong Vang, Ahmad Idris, Jeffrey Cohn, Manas Patel

Team Contributions:

Ahmad Idris - Main Menu, Pause Menu (Resume, Main Menu, Quit), GameManager implementation, CharacterNode save/load integration, SaveService integration, in-game pause menu listener, save button functionality, character persistence between scenes, code reviews and testing.

Fong Vang - Character attributes, battling system, character leveling, Player battle phase /UI, Character leveling UI, Exp UI, review and testing

Jeffrey Cohn - EnemyAI implementation, CharacterBase implementation, EnemyDisplay implementation, enemy decision-making logic, character stat management, damage and healing functionality, code reviews, and testing.

Manas Patel - Enemy Class, Character Movement(Player Controller), Movement Phase, Movement UI, code review/debugging and testing. 

Tanner Gleason - Weapon Class, Level Implementation, Enemy Attributes, Game Over state, Enemy Battle Phase, Branch/feature merging, Code Review, Code Testing, Project Management.

Build Instructions:
To build our project it requires the Godot engine which can be found at the following link:
https://godotengine.org
Press "Download Latest" at the top to be redirected to the download link for your OS
Press "Godot Engine - .NET" to download the version of Godot that supports C# classes, which this project uses.

This project also requires the .NET SDK which can be found at the following link:
https://dotnet.microsoft.com/en-us/download

1. After cloning the repository and downloading Godot, extract the downloaded zip and launch "Godot_<version>-stable_mono_win64.exe". 
	1a. The exact version of Godot used for testing was 4.7.
2. Once Godot loads, select the "Import" button at the top and navigate to the cloned repository folder in the opened window. 
3. Within the repository folder, select "project.godot" and press "open". A new window should open, hit the "Import" button on the bottom left of this new window. 
	3a. If your Godot version is different a new window will open with a warning about a version mismatch, hit "OK" at the bottom to open the project. 
4. After Godot loads, select "Project" in the top left and then "Project Settings."
5. A new window should appear, select "Globals" at the top of this new window.
6. In Godot 4.7 a button labelled "Select Script/Scene" should be towards the top of this menu. Press this button and then navigate to src/Services in the window, or enter "res://src/Services" in the path bar at the top of the window.
	6a. Older versions of Godot may have a different method of setting up the global script. Generally this menu should have 
	a section to add a script to load, but researching adding a "Godot Autoload Global Script" for your specific version should guide you. 
7. Select "GameManager.cs" within the Services folder and press "Open" at the bottom of the window. The window should now have a row
	with an autoloading script named "GameManager" at path "res://src/Services" with the "Global Variable" checkbox checked.
8. Press close at the bottom of the Project Settings window.
9. To build the project, you can press the hammer icon in the top right. 
10. To the right of this hammer icon is a triangular play button which will build the project and run the game for playing.

How to play:
CONTROLS:
arrow keys for movement, UI buttons otherwise.

BASICS:
Press Start Game to load into the first level.
The game functions in phases. The first phase is the movement phase.
There is currently only one level, so the victory screen "Next Level" button does not function.

MOVEMENT PHASE:
You are given a set amount of movement points to use, represented by the number in the top left of the screen. 
Each tile costs 1 movement point to enter.
Upon pressing the "Next phase" button or running out of movement, the game will transition to the next phase.
If an enemy is within 1 tile, the battle phase will initiate, otherwise another movement phase will begin.

PLAYER BATTLE PHASE:
During battle phase, pressing "Fight" will trigger combat with the enemy. Pressing cancel will trigger the next phase.
Combat is automated and the outcome is determined by the stats of the attacking and defending units, which can be previewed
on the left side of the screen during the battle phase.
After fighting or cancelling the battle phase, the game will transition to the next phase. 

ENEMY BATTLE PHASE:
If an enemy is within one tile of the character after the player battle phase, it will trigger the enemy battle phase for the enemy to battle the character.

From here the gameplay loops. 

LEVELLING UP:
Upon damaging or killing an enemy the character gains experience, and after 100 experience they will level up increasing their statistics and making battles easier. The character's current level is displayed in the top left corner. Levelling up also heals the character to their max health.

WINNING:
Win by defeating all of the enemies on the map.

LOSING:
Lose by your character's health reaching zero.

SAVING:
Saving can be done by pressing escape at any time to open the pause menu. This will save the character's current level and statistics. When starting the level it will load this character data at full HP to persist progress between sessions. The Godot default save folder location varies between Operating Systems, with Windows being stored at C:\Users\<username>\AppData\Roaming\Godot\app_userdata\Gridfall by default under the file "savegame.json".
