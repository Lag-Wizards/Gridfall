extends Control

func _ready():
	$MainMenu.pressed.connect(_on_main_menu_pressed)
	$Retry.pressed.connect(_on_retry_pressed)

func _on_main_menu_pressed():
	get_tree().change_scene_to_file("res://resources/scenes/main_menu.tscn")

func _on_retry_pressed():
	get_tree().change_scene_to_file("res://resources/scenes/base-level.tscn")
