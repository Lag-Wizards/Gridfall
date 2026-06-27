How to build:
1. After cloning the repository and downloading Godot, extract the downloaded zip and launch "Godot_<version>-stable_mono_win64.exe". 
2. Once Godot loads, select the "Import" button at the top and navigate to the cloned repository folder in the opened window. 
3. Within the repository folder, select "project.godot" and press "open". A new window should open, hit the "Import" button on the bottom left of this new window. 
    3a. If your Godot version is different a new window will open with a warning about a version mismatch, hit "OK" at the bottom to open the project. 
4. After Godot loads, select "Project" in the top left and then "Project Settings."
5. A new window should appear, select "Globals" at the top of this new window.
6. In Godot 4.7 a button labelled "Select Script/Scene" should be towards the top of this menu. Press this button and then navigate to src/Services in the window, or enter "res://src/Services" in the path bar at the top of the window.
    6a. Older versions of Godot may have a different method of setting up the global script. Generally this menu should have 
    a section to add a script to load, but researching adding a "Godot Autoload Global Script" for your specific version should guide you. 
7. Select "GameManager.cs" within the Services folder and press "Open" at the bottom of the window.
8. Press close at the bottom of the Project Settings window.
9. To build the project, you can press the hammer icon in the top right. 
10. To the right of this hammer icon is a triangular play button which will build the project and run the game for playing.