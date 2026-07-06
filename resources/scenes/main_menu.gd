extends Control

func _ready():
	$VBoxContainer/StartButton.pressed.connect(_on_start_pressed)
	$VBoxContainer/LoadButton.pressed.connect(_on_load_pressed)
	$VBoxContainer/QuitButton.pressed.connect(_on_quit_pressed)

func _on_start_pressed():
	get_tree().change_scene_to_file("res://resources/scenes/base-level.tscn")

func _on_load_pressed():
	get_tree().change_scene_to_file("res://resources/scenes/SaveMenu.tscn")

func _on_quit_pressed():
	get_tree().quit()
