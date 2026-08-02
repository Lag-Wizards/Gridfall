using System.Collections.Generic;

namespace Gridfall.Domain.Enums;

public class CharacterSaveData
{
    public string CharacterName { get; set; }
    public int Level { get; set; }
    public int Health { get; set; }
    public int MaxHealth { get; set; }
    public int Strength { get; set; }
    public int Defense { get; set; }
    public int Speed { get; set; }
    public int MovementRange { get; set; }
    public int Luck { get; set; }
    public int Skill { get; set; }
    public int Constitution { get; set; }
    public int Resistance { get; set; }
    public bool IsMagic { get; set; }
    public int Experience { get; set; }
    public int Coins { get; set; }

    public string EquippedWeaponName { get; set; }
    public string EquippedArmorName { get; set; }
    public List<string> InventoryItemNames { get; set; } = new List<string>();

    public int WeaponType { get; set; }
    public int WeaponDamage { get; set; }
    public int WeaponRange { get; set; }
}