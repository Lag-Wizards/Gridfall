using Godot;
using System.Text.Json;
using Gridfall.Contracts;
using Gridfall.Domain;
using Gridfall.Characters.Domain;

namespace Gridfall.Services;

public class SaveService : ISaveService
{
	private const string SavePath = "user://savegame_{0}.json";

	public void Save(CharacterBase character, int slot)
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

			WeaponType = character.EquippedWeapon != null ? (int)character.EquippedWeapon.Type : 0,
			WeaponDamage = character.EquippedWeapon != null ? character.EquippedWeapon.Damage : 0,
			WeaponRange = character.EquippedWeapon != null ? character.EquippedWeapon.Range : 0
		};

		string json = JsonSerializer.Serialize(saveData);
		string path = GetSavePath(slot);
		
		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
		file.StoreString(json);
	}

	public SaveData Load(int slot)
	{
		if (!SaveExists(slot))
		{
			return null;
		}
		
		string path = GetSavePath(slot);
		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		string json = file.GetAsText();

		return JsonSerializer.Deserialize<SaveData>(json);
	}

	public void DeleteSave(int slot)
	{
		if (SaveExists(slot))
		{
			using var dir = DirAccess.Open("user://");
			dir.Remove(System.IO.Path.GetFileName(GetSavePath(slot)));
		}
	}

	public bool SaveExists(int slot)
	{
		return FileAccess.FileExists(GetSavePath(slot));
	}

	private string GetSavePath(int slot)
	{
		return string.Format(SavePath, slot);
	}
}
