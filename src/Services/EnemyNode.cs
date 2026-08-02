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

	public EnemyBase Stats => _enemy;

	public override void _EnterTree()
	{
		base._EnterTree();
		int effectiveLevel = GameManager.Instance?.CurrentLevelNumber ?? Level;
		var resolvedType = ResolveEnemyType(EnemyType, effectiveLevel);
		_enemy = EnemyManager.CreateEnemy(resolvedType, effectiveLevel);
	}

	private string ResolveEnemyType(string baseType, int level)
	{
		if (level >= 5 && (string.IsNullOrEmpty(baseType) || baseType.Equals("Slime", StringComparison.OrdinalIgnoreCase)))
		{
			return "NormalSlime";
		}

		return baseType;
	}

	public override void _Ready()
	{
		base._Ready();
		UpdateName();
		Events.EmitEnemySpawned();

		if (_enemy != null)
		{
			_enemy.OnUnitDeath += Die;
			var healthBar = new EnemyHealthBar(_enemy);
			AddChild(healthBar);
		}

		var sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite != null && sprite.Position != Vector2.Zero)
		{
			GlobalPosition += sprite.Position;
			sprite.Position = Vector2.Zero;
		}

		if (GameManager.Instance?.GridManager != null)
		{
			Vector2I gridPos = GameManager.Instance.GridManager.WorldToMap(GlobalPosition);
			GlobalPosition = GameManager.Instance.GridManager.MapToWorld(gridPos);
			GD.Print($"Snapped enemy node to grid tile {gridPos} at world {GlobalPosition}");

			var tileState = GameManager.Instance.GridManager.GetTileStateAt(gridPos);
			if (tileState != null)
			{
				tileState.CurrentOccupant = this;
			}
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (@event is not InputEventMouseButton mouseButton || !mouseButton.Pressed || mouseButton.ButtonIndex != MouseButton.Left)
		{
			return;
		}

		var sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite == null)
		{
			return;
		}

		var localMousePosition = sprite.ToLocal(GetGlobalMousePosition());
		var spriteRect = sprite.GetRect();
		if (spriteRect.HasPoint(localMousePosition))
		{
			GameManager.Instance?.ShowEnemyPreview(this);
			GameManager.Instance?.SelectEnemy(this);
			GetViewport().SetInputAsHandled();
		}
	}
	
	
	public bool IsMouseOver(Vector2 globalMousePosition)
	{
		var sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite == null) return false;

		var localMousePosition = sprite.ToLocal(globalMousePosition);
		return sprite.GetRect().HasPoint(localMousePosition);
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
		if (GameManager.Instance?.GridManager != null)
		{
			Vector2I gridPos = GameManager.Instance.GridManager.WorldToMap(GlobalPosition);
			var tileState = GameManager.Instance.GridManager.GetTileStateAt(gridPos);
			if (tileState != null && tileState.CurrentOccupant == this)
			{
				tileState.CurrentOccupant = null;
			}
		}
		QueueFree();
	}
}
