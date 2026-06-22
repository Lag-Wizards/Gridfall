using Godot;
using Gridfall.Characters.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Services;

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

		Weapon starterWeapon = new Weapon(WeaponType.Melee, 5, 1);

		_character.SetupCharacter(CharacterName, Level, starterWeapon);

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
