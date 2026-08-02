extends Control

func _ready():
	$MainMenu.pressed.connect(_on_main_menu_pressed)
	$Save.pressed.connect(_on_save_pressed)
	$NextLevel.pressed.connect(_on_next_level_pressed)
	GameManager.CurrentLevelNumber += 1

func _on_main_menu_pressed():
	get_tree().paused = false
	hide()
	get_tree().change_scene_to_file("res://resources/scenes/main_menu.tscn")

func _on_save_pressed():
	var save_menu_template = load("res://resources/scenes/SaveMenu.tscn")
	if save_menu_template:
		var save_menu_instance = save_menu_template.instantiate()
		save_menu_instance.IsSaveMode = true
		add_child(save_menu_instance)
		print("Opened save menu in savemode on Victory screen.")
	else:
		push_error("Failed to load SaveMenu.tscn")

func _on_next_level_pressed():
	get_tree().paused = false
	hide()
	var next_level_path = "res://resources/scenes/level-%d.tscn" % GameManager.CurrentLevelNumber
	if FileAccess.file_exists(next_level_path):
		get_tree().change_scene_to_file(next_level_path)
	else:
		get_tree().change_scene_to_file("res://resources/scenes/main_menu.tscn")
