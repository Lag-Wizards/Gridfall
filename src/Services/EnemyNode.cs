using Godot;
using System;
using Gridfall.Services;
using Gridfall.Domain;
using Gridfall.Domain.Enemies;

public partial class EnemyNode : Node2D
{
	[Export]
	public string EnemyType = "Goblin";

	[Export]
	public int Level = 1;

	private EnemyBase _enemy;

	public async override void _Ready()
	{
		base._Ready();
		_enemy = EnemyManager.CreateEnemy(EnemyType, Level);
		UpdateName();
		Events.EmitEnemySpawned();
		GD.Print("Victory in 3 seconds (triggered in EnemyNode)");
		await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);
		ReceiveDamage(100);
	}

	private void UpdateName()
	{
		if (_enemy != null)
		{
			Name = _enemy.Name + " L" + _enemy.Level;
		}
	}

	public void ReceiveDamage(int amount, bool isMagic = false)
	{
		_enemy.TakeDamage(amount, isMagic);
		if (!_enemy.IsAlive)
			Die();
	}

	public int AttackTarget(EnemyNode target)
	{
		var dmg = _enemy.CalculateDamageTo(target._enemy);
		target.ReceiveDamage(dmg);
		return dmg;
	}

	private void Die()
	{
		// Default behavior: queue free. Teammates can override/extend in scene.
		QueueFree();
	}
}
