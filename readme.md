Roles
Product Owner - Adam Pastorok Email: Apastorok@gmail.com
Scrum Master - Tanner Gleason
Developers - Fong Vang, Ahmad Idris, Jeffrey Cohn, Manas Patel

Team Contributions:

Ahmad Idris - Main Menu, Pause Menu (Resume, Main Menu, Quit), GameManager implementation, CharacterNode save/load integration, SaveService integration, in-game pause menu listener, save button functionality, character persistence between scenes, code reviews and testing.

Fong Vang - 
Jeffrey Cohn - EnemyAI, Character Class, Enemy Display
Manas Patel - 
Tanner Gleason -

Build Instructions:
To build our project it requires the Godot engine which can be found at the following link:
https://godotengine.org/download/windows/
The .NET version of Godot Engine should be downloaded as this is what allows Godot to use C# classes.

Godot requires a windows computer and the .NET SDK which can be found at the following link:
https://dotnet.microsoft.com/en-us/download

After cloning the repository and downloading Godot, launch "Godot_*version*-stable_mono_win64.exe" from the downloaded zip from godotengine.org. Once Godot loads, select the "Import" button at the top and navigate to the cloned repository folder in the opened window. Within the repository folder, select "project.godot" and press "open". A new window should open, hit the "Import" button on the bottom left of this new window. If your Godot version is different a new window will open with a warning about a version mismatch, hit "OK" at the bottom to open the project. The project editor will open. To build the project, you can press the hammer icon in the top right. To the right of this hammer icon is a triangular play button which will build the project and run the game for playing.

How to play:
Press Start Game to load into the first level.
Use arrow keys to move your character, and move towards the enemy to attack them.
There is a limited amount of movement per turn.
Once close enough, attack by pressing the button.
Once you end your turn the enemy will attack your character if they are in range and then end its turn.
Hit the escape key to pause and save your character's progress.
If you lose you can retry the level, if you win you can save your progress and move on to the next level (Currently only one level exists).
