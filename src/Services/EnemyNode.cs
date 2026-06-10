using Godot;
using System;
using Gridfall.Services;


public partial class EnemyNode : Node2D
{
	[Export]
	public string EnemyType = "Goblin";

	[Export]
	public int Level = 1;

	private EnemyBase _enemy;

	public override void _Ready()
	{
		base._Ready();
		_enemy = EnemyManager.CreateEnemy(EnemyType, Level);
		// You can expose stats to the editor or other nodes
		UpdateName();
	}

	private void UpdateName()
	{
		Name = _enemy.Name + " L" + _enemy.Level;
	}

	public void ReceiveDamage(int amount)
	{
		_enemy.TakeDamage(amount);
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
