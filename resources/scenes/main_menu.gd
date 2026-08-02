extends Control

func _ready():
	$VBoxContainer/StartButton.pressed.connect(_on_start_pressed)
	$VBoxContainer/LoadButton.pressed.connect(_on_load_pressed)
	$VBoxContainer/QuitButton.pressed.connect(_on_quit_pressed)

func _on_start_pressed():
	GameManager.LoadCharacterData()
	var level_num = GameManager.CurrentLevelNumber
	var level_path = "res://resources/scenes/level-%d.tscn" % level_num
	if FileAccess.file_exists(level_path):
		get_tree().change_scene_to_file(level_path)
	else:
		get_tree().change_scene_to_file("res://resources/scenes/level-1.tscn")

func _on_load_pressed():
	get_tree().change_scene_to_file("res://resources/scenes/SaveMenu.tscn")

func _on_quit_pressed():
	get_tree().quit()
