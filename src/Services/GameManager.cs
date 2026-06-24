using Godot;
using System;
using Gridfall.Domain;
using Gridfall.Services;

public partial class GameManager : Node
{
	public CharacterBase CurrentCharacter { get; private set; }

	private SaveService _saveService = new SaveService();
	private Control _pauseMenu;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;

		GD.Print("!!!!!!!! GAME MANAGER READY !!!!!!!!");
		CharacterBase.OnPlayerDeath += HandlePlayerDeath;
		LoadCharacterData();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			TogglePauseMenu();
		}
	}

	private void TogglePauseMenu()
	{
		if (_pauseMenu == null || !GodotObject.IsInstanceValid(_pauseMenu))
		{
			_pauseMenu = GetTree().Root.GetNodeOrNull<Control>("Node2D/PauseMenu");
		}

		if (_pauseMenu == null)
		{
			GD.Print("PauseMenu not found.");
			return;
		}

		bool shouldPause = !GetTree().Paused;

		GetTree().Paused = shouldPause;
		_pauseMenu.Visible = shouldPause;
	}

	public void LoadCharacterData()
	{
		if (_saveService.SaveExists())
		{
			SaveData saveData = _saveService.Load();

			CurrentCharacter = new CharacterBase();
			CurrentCharacter.LoadFromSave(saveData);

			GD.Print("GameManager loaded saved character data.");
			GD.Print(CurrentCharacter.CharacterName);
			GD.Print(CurrentCharacter.Level);
		}
		else
		{
			GD.Print("GameManager found no save data.");
		}
	}

	public void SetCurrentCharacter(CharacterBase character)
	{
		CurrentCharacter = character;
		GD.Print("GameManager current character set.");
	}

	public void SaveCurrentCharacter()
	{
		if (CurrentCharacter == null)
		{
			GD.Print("No current character to save.");
			return;
		}

		_saveService.Save(CurrentCharacter);
		GD.Print("Current character saved.");
	}

	public void HandlePlayerDeath()
	{
		GetTree().ChangeSceneToFile("res://resources/scenes/game_over.tscn");
	}
}