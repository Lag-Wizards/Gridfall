using System;
using Godot;
using System.Collections.Generic;
using Gridfall.Domain;
using Gridfall.Domain.Enums;
using Gridfall.Services;

namespace Gridfall.Characters.Domain
{
	public partial class CharacterBase : CombatUnit
	{
		public string CharacterName
		{
			get => UnitName;
			set => UnitName = value;
		}

		public int MovementRange
		{
			get => MoveDistance;
			set => MoveDistance = value;
		}

		public int Experience { get; set; }

		// Inventory of generic equipment (weapons & armor)
		public List<Equipment> Inventory { get; set; } = new List<Equipment>();

		// Declaring event handler for when exp changes so UI can update
		[Signal]
		public delegate void ExpChangedEventHandler(int exp);

		public void LoadFromSave(SaveData saveData)
		{
			CharacterName = saveData.CharacterName;
			Level = saveData.Level;

			// Base stats initialized (without equipment bonuses since EquippedWeapon/Armor are null right now)
			EquippedWeapon = null;
			EquippedArmor = null;

			MaxHealth = saveData.MaxHealth;
			Strength = saveData.Strength;
			Defense = saveData.Defense;
			Speed = saveData.Speed;
			MovementRange = saveData.MovementRange;
			Luck = saveData.Luck;
			Skill = saveData.Skill;
			Constitution = saveData.Constitution;
			Resistance = saveData.Resistance;
			IsMagic = saveData.IsMagic;
			Experience = saveData.Experience;

			GD.Print($"[CharacterBase LoadFromSave] Loaded character: {CharacterName}, Level: {Level}");
			GD.Print($"[CharacterBase LoadFromSave] Weapon in save: {saveData.EquippedWeaponName}, Armor in save: {saveData.EquippedArmorName}");
			if (saveData.InventoryItemNames != null)
			{
				GD.Print($"[CharacterBase LoadFromSave] Inventory items in save: {string.Join(", ", saveData.InventoryItemNames)}");
			}
			else
			{
				GD.Print($"[CharacterBase LoadFromSave] Inventory items list in save is NULL.");
			}

			Inventory.Clear();

			// 1. Load Inventory Items first
			if (saveData.InventoryItemNames != null && saveData.InventoryItemNames.Count > 0)
			{
				foreach (var itemName in saveData.InventoryItemNames)
				{
					var item = ItemFactory.CreateEquipment(itemName);
					if (item != null)
					{
						Inventory.Add(item);
					}
				}
			}

			// 2. Load Equipped Weapon (linking it to the instance in the inventory)
			if (!string.IsNullOrEmpty(saveData.EquippedWeaponName))
			{
				EquippedWeapon = Inventory.Find(item => item is Weapon && item.Name == saveData.EquippedWeaponName) as Weapon;
				if (EquippedWeapon == null)
				{
					EquippedWeapon = ItemFactory.CreateEquipment(saveData.EquippedWeaponName) as Weapon;
					if (EquippedWeapon != null)
					{
						Inventory.Add(EquippedWeapon);
					}
				}
			}
			else if (saveData.WeaponDamage > 0)
			{
				// Legacy weapon fallback
				EquippedWeapon = new Weapon(
					(WeaponType)saveData.WeaponType,
					saveData.WeaponDamage,
					saveData.WeaponRange
				);
				Inventory.Add(EquippedWeapon);
			}

			// 3. Load Equipped Armor (linking it to the instance in the inventory)
			if (!string.IsNullOrEmpty(saveData.EquippedArmorName))
			{
				EquippedArmor = Inventory.Find(item => item is Armor && item.Name == saveData.EquippedArmorName) as Armor;
				if (EquippedArmor == null)
				{
					EquippedArmor = ItemFactory.CreateEquipment(saveData.EquippedArmorName) as Armor;
					if (EquippedArmor != null)
					{
						Inventory.Add(EquippedArmor);
					}
				}
			}

			// 4. Populate inventory if it was loaded from a legacy file (which had no inventory list)
			if (Inventory.Count == 0)
			{
				if (EquippedWeapon != null)
				{
					Inventory.Add(EquippedWeapon);
				}
				if (EquippedArmor != null)
				{
					Inventory.Add(EquippedArmor);
				}
			}

			// Set HP last to clamp correctly against loaded max health + loaded bonuses
			Health = saveData.Health;
		}

		public override void _Ready()
		{
		}

		public void SetupCharacter(string characterName, int level, Weapon weapon, Armor armor = null, Dictionary<string, int> customGrowthRates = null)
		{
			CharacterName = characterName;
			Level = level;

			// Assign base stats
			MaxHealth = 25 + (level * 5);
			Strength = 6 + level;
			Defense = 3 + level;
			Speed = 4 + level;
			Skill = 3;
			Resistance = 2;
			MovementRange = 5;

			EquippedWeapon = weapon;
			EquippedArmor = armor;

			Inventory.Clear();
			if (weapon != null)
			{
				Inventory.Add(weapon);
			}
			if (armor != null)
			{
				Inventory.Add(armor);
			}

			Health = MaxHealth; // clamp Health to full MaxHealth (with bonuses)

			if (customGrowthRates != null)
			{
				foreach (var growth in customGrowthRates)
				{
					if (GrowthRates.ContainsKey(growth.Key))
					{
						GrowthRates[growth.Key] = growth.Value;
					}
				}
			}
		}

		public void AddEquipmentToInventory(Equipment equipment)
		{
			if (equipment == null)
				return;

			Inventory.Add(equipment);
			GD.Print("Added equipment to inventory: " + equipment.Name);
		}

		public void AddWeaponToInventory(Weapon weapon)
		{
			AddEquipmentToInventory(weapon);
		}

		public void EquipWeapon(Weapon weapon)
		{
			if (weapon == null)
				return;

			if (!Inventory.Contains(weapon))
				return;

			EquippedWeapon = weapon;
			GD.Print("Equipped weapon: " + weapon.Name);
		}

		public void EquipArmor(Armor armor)
		{
			if (armor == null)
				return;

			if (!Inventory.Contains(armor))
				return;

			EquippedArmor = armor;
			GD.Print("Equipped armor: " + armor.Name);
		}

		public void UnequipArmor()
		{
			EquippedArmor = null;
			GD.Print("Unequipped armor.");
		}

		public void AddExperience(int amount)
		{
			Experience += amount;
			EmitSignal(SignalName.ExpChanged, Experience);
		}

		public void ModifyStat(string stat, int amount)
		{
			switch (stat)
			{
				case "Level":
					Level += amount;
					EmitSignal(SignalName.StatChanged, stat, Level);
					break;

				case "HP":
					MaxHealth += amount;
					EmitSignal(SignalName.StatChanged, stat, MaxHealth);
					break;

				case "STR":
					Strength += amount;
					EmitSignal(SignalName.StatChanged, stat, Strength);
					break;

				case "SPD":
					Speed += amount;
					EmitSignal(SignalName.StatChanged, stat, Speed);
					break;

				case "DEF":
					Defense += amount;
					EmitSignal(SignalName.StatChanged, stat, Defense);
					break;

				case "LUCK":
					Luck += amount;
					EmitSignal(SignalName.StatChanged, stat, Luck);
					break;

				case "SKILL":
					Skill += amount;
					EmitSignal(SignalName.StatChanged, stat, Skill);
					break;

				case "RES":
					Resistance += amount;
					EmitSignal(SignalName.StatChanged, stat, Resistance);
					break;
			}
		}
	}
}
