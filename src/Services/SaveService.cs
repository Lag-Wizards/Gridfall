using System.Collections.Generic;
using Godot;
using System.Text.Json;
using Gridfall.Contracts;
using Gridfall.Domain;
using Gridfall.Characters.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Services;

public class SaveService : ISaveService
{
	private const string SavePath = "user://savegame_{0}.json";

	public void Save(List<CharacterBase> characters, int coins, int slot)
	{
		SaveData saveData = new SaveData();
		saveData.Coins = coins;
		foreach (var character in characters)
		{
			CharacterSaveData characterData = new CharacterSaveData
			{
				CharacterName = character.CharacterName,
				Level = character.Level,
				Health = character.Health,

				// Save BASE stats (without equipment bonuses) to avoid double-application on reload
				MaxHealth = character.BaseMaxHealth,
				Strength = character.BaseStrength,
				Defense = character.BaseDefense,
				Speed = character.BaseSpeed,
				MovementRange = character.BaseMoveDistance,
				Luck = character.BaseLuck,
				Skill = character.BaseSkill,
				Constitution = character.BaseConstitution,
				Resistance = character.BaseResistance,
				IsMagic = character.IsMagic,
				Experience = character.Experience,
				Coins = GameManager.Instance?.GetCoinCount() ?? 0,

				// Serialize equipment by name
				EquippedWeaponName = character.EquippedWeapon?.Name,
				EquippedArmorName = character.EquippedArmor?.Name,
				InventoryItemNames = character.Inventory.ConvertAll(item => item.Name),

				// Legacy fields for backwards compatibility
				WeaponType = character.EquippedWeapon != null ? (int)character.EquippedWeapon.Type : 0,
				WeaponDamage = character.EquippedWeapon != null ? character.EquippedWeapon.Damage : 0,
				WeaponRange = character.EquippedWeapon != null ? character.EquippedWeapon.Range : 0
			};
			saveData.Characters.Add(characterData);
		}

		string json = JsonSerializer.Serialize(saveData);
		GD.Print($"[SaveService] Serialized save data: {json}");
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
		GD.Print($"[SaveService] Loaded raw save JSON: {json}");

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
