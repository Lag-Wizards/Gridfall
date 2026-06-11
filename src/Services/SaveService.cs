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

            WeaponType = (int)character.EquippedWeapon.Type,
            WeaponDamage = character.EquippedWeapon.Damage,
            WeaponRange = character.EquippedWeapon.Range
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