using Godot;
using System;
using Gridfall.Domain;

public partial class GameManager : Node
{
	public override void _Ready()
	{
		GD.Print("Game Manager Ready");
		CharacterBase.OnPlayerDeath += HandlePlayerDeath;
	}

	public override void _Process(double delta)
	{
		
	}
	
	public void HandlePlayerDeath()
	{
		GetTree().ChangeSceneToFile("res://resources/scenes/game_over.tscn");
	}
}
