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
		_enemy = EnemyManager.CreateEnemy(EnemyType, Level);
	}

	public override void _Ready()
	{
		base._Ready();
		UpdateName();
		Events.EmitEnemySpawned();

		if (_enemy != null)
		{
			_enemy.OnUnitDeath += Die;
		}

		CreateInteractionArea();

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

	private void CreateInteractionArea()
	{
		var area = new Area2D
		{
			Name = "EnemyInteractionArea",
			InputPickable = true
		};

		var collision = new CollisionShape2D
		{
			Name = "EnemyCollisionShape"
		};

		var shape = new RectangleShape2D();
		var sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (sprite != null && sprite.Texture != null)
		{
			var textureSize = sprite.Texture.GetSize();
			shape.Size = textureSize * 0.75f;
		}
		else
		{
			shape.Size = new Vector2(32, 32);
		}

		collision.Shape = shape;
		collision.Position = sprite?.Position ?? Vector2.Zero;

		area.AddChild(collision);
		area.MouseEntered += OnInteractionMouseEntered;
		area.MouseExited += OnInteractionMouseExited;
		area.InputEvent += OnInteractionInputEvent;
		AddChild(area);
	}

	private void OnInteractionMouseEntered()
	{
		GameManager.Instance?.ShowEnemyPreview(this);
	}

	private void OnInteractionMouseExited()
	{
		GameManager.Instance?.HideEnemyPreview(this);
	}

	private void OnInteractionInputEvent(Node viewport, InputEvent @event, long shapeIdx)
	{
		if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
		{
			GameManager.Instance?.ShowEnemyPreview(this);
		}
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
