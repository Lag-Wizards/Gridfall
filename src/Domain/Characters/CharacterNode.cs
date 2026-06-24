using Godot;
using Gridfall.Domain;
using Gridfall.Domain.Enums;
using Gridfall.Services;

namespace Gridfall.Characters.Domain;

public partial class CharacterNode : Node2D
{
	[Export]
	public string CharacterName = "Hero";

	[Export]
	public int Level = 1;

	private CharacterBase _character;
	private Sprite2D _sprite;

	public override void _Ready()
	{
		base._Ready();

		_character = new CharacterBase();
		SaveService saveService = new SaveService();

		if (saveService.SaveExists())
		{
			SaveData saveData = saveService.Load();
			_character.LoadFromSave(saveData);
			GD.Print("Loaded character data from save.");
		}
		else
		{
			Weapon starterWeapon = new Weapon(WeaponType.Slash, 5, 1);
			_character.SetupCharacter(CharacterName, Level, starterWeapon);
			GD.Print("No save found. Created default character.");
		}

		GameManager gameManager = GetNode<GameManager>("/root/GameManager");
		gameManager.SetCurrentCharacter(_character);

		CreateSprite();

		UpdateName();
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

	public void SetGridPosition(Vector2I gridPosition, TileMapLayer gridMap)
	{
		Position = gridMap.MapToLocal(gridPosition);
	}

	public void MoveToGridPosition(Vector2I gridPosition, TileMapLayer gridMap)
	{
		Position = gridMap.MapToLocal(gridPosition);
	}
}
