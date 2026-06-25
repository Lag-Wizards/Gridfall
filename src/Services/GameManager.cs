using Godot;
using System;
using Gridfall.Domain;
using Gridfall.Services;

public partial class GameManager : Node
{
	private int _enemyCount = 0;
	
	public override void _Ready()
	{
		GD.Print("Game Manager Ready");
		Events.OnEnemySpawn += HandleEnemySpawn;
		Events.OnPlayerDeath += HandlePlayerDeath;
		Events.OnEnemyDeath += HandleEnemyDeath;
	}

	public override void _Process(double delta)
	{
		
	}
	
	public void HandleEnemySpawn()
	{
		_enemyCount++;
		GD.Print("Enemies alive: ", _enemyCount);
	}
	
	public void HandlePlayerDeath()
	{
		GetTree().ChangeSceneToFile("res://resources/scenes/game_over.tscn");
	}
	
	public void HandleEnemyDeath()
	{
		_enemyCount--;
		if (_enemyCount == 0)
		{
			GetTree().ChangeSceneToFile("res://resources/scenes/victory.tscn");
		}
	}
}
