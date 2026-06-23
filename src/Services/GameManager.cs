using Godot;
using System;
using Gridfall.Domain;
using Gridfall.Services;

public partial class GameManager : Node
{
	public CharacterBase CurrentCharacter { get; private set; }

	private SaveService _saveService = new SaveService();

	public override void _Ready()
	{
		GD.Print("!!!!!!!! GAME MANAGER READY !!!!!!!!");
		CharacterBase.OnPlayerDeath += HandlePlayerDeath;
		LoadCharacterData();
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

	public override void _Process(double delta)
	{
		
	}
	
	public void HandlePlayerDeath()
	{
		GetTree().ChangeSceneToFile("res://resources/scenes/game_over.tscn");
	}
}