extends Control

func _ready():
	$MainMenu.pressed.connect(_on_main_menu_pressed)
	$Save.pressed.connect(_on_save_pressed)

func _on_main_menu_pressed():
	get_tree().paused = false
	hide()
	get_tree().change_scene_to_file("res://resources/scenes/main_menu.tscn")

func _on_save_pressed():
	GameManager.SaveCurrentCharacter()
	print("Save button pressed.")
