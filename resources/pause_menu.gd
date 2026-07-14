extends Control

func _ready():
	$VBoxContainer/ResumeButton.pressed.connect(_on_resume_pressed)
	$VBoxContainer/MainMenuButton.pressed.connect(_on_main_menu_pressed)
	$VBoxContainer/QuitButton.pressed.connect(_on_quit_pressed)
	$VBoxContainer/SaveButton.pressed.connect(_on_save_pressed)

func _on_resume_pressed():
	get_tree().paused = false
	hide()

func _on_main_menu_pressed():
	get_tree().paused = false
	hide()
	get_tree().change_scene_to_file("res://resources/scenes/main_menu.tscn")

func _on_quit_pressed():
	get_tree().quit()

func _on_save_pressed():
	var save_menu_template = load("res://resources/scenes/SaveMenu.tscn")
	
	if save_menu_template:
		var save_menu_instance = save_menu_template.instantiate()
		
		save_menu_instance.IsSaveMode = true
		
		add_child(save_menu_instance)
		print("Opened save menu in savemode.")
	else:
		push_error("Failed to load SaveMenu.tscn")
