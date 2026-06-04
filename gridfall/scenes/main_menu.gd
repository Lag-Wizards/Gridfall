extends Control

func _ready():
	$VBoxContainer/StartButton.pressed.connect(_on_start_pressed)
	$VBoxContainer/QuitButton.pressed.connect(_on_quit_pressed)

func _on_start_pressed():
	print("Start button clicked")

func _on_quit_pressed():
	get_tree().quit()
