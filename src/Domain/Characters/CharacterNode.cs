using Godot;
using Gridfall.Domain;
using Gridfall.Domain.Enums;
using Gridfall.Services;
using Gridfall.Contracts;
using System;
using System.Collections.Generic;

namespace Gridfall.Characters.Domain;

public partial class CharacterNode : Node2D
{
	[Export]
	public string CharacterName = "Hero";

	[Export]
	public int Level = 1;

	private CharacterBase _character = new CharacterBase();
	private Sprite2D _sprite;
	private IMovementService _movementService;

	public CharacterBase Stats => _character;

	public Vector2I CurrentTile { get; set; }

	public override void _EnterTree()
	{
		base._EnterTree();

		SaveService saveService = new SaveService();

		if (saveService.SaveExists())
		{
			GD.Print("Attempting to load save data");
			SaveData saveData = saveService.Load();
			saveData.Health = saveData.MaxHealth;
			_character.LoadFromSave(saveData);
			GD.Print("Loaded character data from save.");
		}
		else
		{
			Weapon starterWeapon = new Weapon(WeaponType.Slash, 5, 1);
			_character.SetupCharacter(CharacterName, Level, starterWeapon);
			GD.Print("No save found. Created default character.");
		}

		_character.OnUnitDeath += OnPlayerDeath;
		GameManager.Instance?.SetCurrentCharacter(_character);
		GameManager.Instance?.RegisterPlayerCharacterNode(this);
	}

	public override void _Ready()
	{
		base._Ready();

		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (_sprite == null)
		{
			CreateSprite();
		}

		UpdateName();
		InitializeGridPosition();
	}

	private void CreateSprite()
	{
		_sprite = new Sprite2D();
		_sprite.Name = "CharacterSprite";
		AddChild(_sprite);
	}

	private void UpdateName()
	{
		Name = _character.CharacterName + " L" + _character.Level;
	}

	public void InitializeGridPosition()
	{
		if (_movementService != null) return;
		var gridManager = GameManager.Instance?.GridManager;
		if (gridManager == null) return;

		_sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
		if (_sprite != null && _sprite.Position != Vector2.Zero)
		{
			GlobalPosition += _sprite.Position;
			_sprite.Position = Vector2.Zero;
		}

		CurrentTile = gridManager.WorldToMap(GlobalPosition);
		GlobalPosition = gridManager.MapToWorld(CurrentTile);
		RegisterOccupantAt(CurrentTile);
		_movementService = new MovementService(gridManager);
	}

	public bool TryMove(Vector2I direction, int remainingMovement, out int movementCost)
	{
		movementCost = 0;
		var gridManager = GameManager.Instance?.GridManager;
		if (gridManager == null || _movementService == null)
			return false;

		var target = CurrentTile + direction;

		if (!_movementService.CanReach(CurrentTile, target, remainingMovement))
			return false;

		var targetState = gridManager.GetTileStateAt(target);
		if (targetState == null || targetState.MovementCost >= 999 || targetState.IsOccupied)
			return false;

		UnregisterOccupantAt(CurrentTile);
		CurrentTile = target;
		RegisterOccupantAt(CurrentTile);
		GlobalPosition = gridManager.MapToWorld(CurrentTile);

		movementCost = targetState.MovementCost;
		return true;
	}

	public List<EnemyNode> GetEnemiesInRange()
	{
		var list = new List<EnemyNode>();
		var gridManager = GameManager.Instance?.GridManager;
		if (gridManager == null || _character == null)
			return list;

		int range = _character.EquippedWeapon?.Range ?? 1;
		var sceneRoot = GetTree().CurrentScene;
		var enemies = FindEnemyNodesRecursive(sceneRoot);

		foreach (var enemyNode in enemies)
		{
			if (!GodotObject.IsInstanceValid(enemyNode))
				continue;

			Vector2I enemyTile = gridManager.WorldToMap(enemyNode.GlobalPosition);
			int distance = Mathf.Max(Mathf.Abs(CurrentTile.X - enemyTile.X), Mathf.Abs(CurrentTile.Y - enemyTile.Y));
			if (distance <= range)
			{
				list.Add(enemyNode);
			}
		}

		return list;
	}

	public EnemyNode GetEnemyInRange()
	{
		var enemies = GetEnemiesInRange();
		return enemies.Count > 0 ? enemies[0] : null;
	}

	private List<EnemyNode> FindEnemyNodesRecursive(Node node)
	{
		var list = new List<EnemyNode>();
		if (node is EnemyNode enemy)
			list.Add(enemy);

		if (node != null)
		{
			foreach (var child in node.GetChildren())
			{
				list.AddRange(FindEnemyNodesRecursive(child));
			}
		}
		return list;
	}

	private void RegisterOccupantAt(Vector2I tile)
	{
		var gridManager = GameManager.Instance?.GridManager;
		if (gridManager != null)
		{
			var tileState = gridManager.GetTileStateAt(tile);
			if (tileState != null)
				tileState.CurrentOccupant = this;
		}
	}

	private void UnregisterOccupantAt(Vector2I tile)
	{
		var gridManager = GameManager.Instance?.GridManager;
		if (gridManager != null)
		{
			var tileState = gridManager.GetTileStateAt(tile);
			if (tileState != null && tileState.CurrentOccupant == this)
				tileState.CurrentOccupant = null;
		}
	}

	private void OnPlayerDeath()
	{
		GD.Print("CharacterNode: Player has died! Emitting player death event.");
		Events.EmitPlayerDied();
	}
}
