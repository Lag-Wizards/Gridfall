using System;
using Gridfall.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Services
{
	public static class ItemFactory
	{
		public static Equipment CreateEquipment(string name)
		{
			if (string.IsNullOrEmpty(name)) return null;

			switch (name.ToLower())
			{
				case "copper sword":
					return new Weapon(WeaponType.Slash, "Copper Sword", 5, 1, 1, 80, 5);

				case "iron sword":
					var ironSword = new Weapon(WeaponType.Slash, "Iron Sword", 8, 1, 2, 80, 10);
					ironSword.StrengthBonus = 1;
					return ironSword;

				case "steel sword":
					var steelSword = new Weapon(WeaponType.Slash, "Steel Sword", 12, 1, 4, 75, 20);
					steelSword.StrengthBonus = 2;
					return steelSword;

				case "iron lance":
					var ironLance = new Weapon(WeaponType.Pierce, "Iron Lance", 10, 1, 3, 75, 15);
					return ironLance;

				case "iron armor":
					var ironArmor = new Armor("Iron Armor", 15, 5);
					ironArmor.HpBonus = 5;
					ironArmor.DefenseBonus = 4;
					ironArmor.SpeedBonus = -1;
					return ironArmor;

				case "health potion":
					return new Consumable("Health Potion", 5, "Restores 10 HP.");

				default:
					// Fallback weapon
					return new Weapon(WeaponType.Slash, name, 5, 1, 1, 85, 5);
			}
		}
	}
}
