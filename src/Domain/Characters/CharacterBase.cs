using System;
using Godot;
using System.Collections.Generic;
using Gridfall.Domain;
using Gridfall.Domain.Enums;

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

		// Inventory of weapons
		public List<Weapon> Inventory { get; set; } = new List<Weapon>();

		public int HealthPotionCount { get; private set; }

		public void AddHealthPotion()
		{
			HealthPotionCount++;
			GD.Print($"Health potion added. Total: {HealthPotionCount}");
		}

		public bool UseHealthPotion()
		{
			if (HealthPotionCount <= 0 || Health >= MaxHealth)
				return false;

			HealthPotionCount--;
			Heal(10);
			GD.Print($"Health potion used. Remaining: {HealthPotionCount}");
			return true;
		}


		// Declaring event handler for when exp changes so UI can update
		[Signal]
		public delegate void ExpChangedEventHandler(int exp);

		public void LoadFromSave(SaveData saveData)
		{
			CharacterName = saveData.CharacterName;
			Level = saveData.Level;
			Health = saveData.Health;
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

			EquippedWeapon = new Weapon(
				(WeaponType)saveData.WeaponType,
				saveData.WeaponDamage,
				saveData.WeaponRange
			);

			Inventory.Clear();
			Inventory.Add(EquippedWeapon);
		}

		public override void _Ready()
		{
			// Cleaned up the debug auto-damage timers.
			// Weapon and character will be set up via CharacterNode or GameManager.
		}

		public void SetupCharacter(string characterName, int level, Weapon weapon, Dictionary<string, int> customGrowthRates = null)
		{
			CharacterName = characterName;
			Level = level;

			MaxHealth = 25 + (level * 5);
			Health = MaxHealth;

			Strength = 6 + level;
			Defense = 3 + level;
			Speed = 4 + level;
			Skill = 3;
			Resistance = 2;
			MovementRange = 5;

			EquippedWeapon = weapon;

			Inventory.Clear();
			Inventory.Add(weapon);

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

		public void AddWeaponToInventory(Weapon weapon)
		{
			if (weapon == null)
				return;

			Inventory.Add(weapon);
			GD.Print("Added weapon to inventory: " + weapon.Name);
		}

		public void EquipWeapon(Weapon weapon)
		{
			if (weapon == null)
				return;

			if (!Inventory.Contains(weapon))
				return;

			EquippedWeapon = weapon;

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
