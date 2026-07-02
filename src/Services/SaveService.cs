using Godot;
using System.Text.Json;
using Gridfall.Contracts;
using Gridfall.Domain;
using Gridfall.Characters.Domain;

namespace Gridfall.Services;

public class SaveService : ISaveService
{
	private const string SavePath = "user://savegame.json";

	public void Save(CharacterBase character)
	{
		SaveData saveData = new SaveData
		{
			CharacterName = character.CharacterName,
			Level = character.Level,
			Health = character.Health,
			MaxHealth = character.MaxHealth,
			Strength = character.Strength,
			Defense = character.Defense,
			Speed = character.Speed,
			MovementRange = character.MovementRange,
			Luck = character.Luck,
			Skill = character.Skill,
			Constitution = character.Constitution,
			Resistance = character.Resistance,
			IsMagic = character.IsMagic,
			Experience = character.Experience,
			Coins = character.Coins,

			WeaponType = character.EquippedWeapon != null ? (int)character.EquippedWeapon.Type : 0,
			WeaponDamage = character.EquippedWeapon != null ? character.EquippedWeapon.Damage : 0,
			WeaponRange = character.EquippedWeapon != null ? character.EquippedWeapon.Range : 0
		};

		string json = JsonSerializer.Serialize(saveData);

		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
		file.StoreString(json);
	}

	public SaveData Load()
	{
		if (!SaveExists())
		{
			return null;
		}

		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
		string json = file.GetAsText();

		return JsonSerializer.Deserialize<SaveData>(json);
	}

	public bool SaveExists()
	{
		return FileAccess.FileExists(SavePath);
	}
}
