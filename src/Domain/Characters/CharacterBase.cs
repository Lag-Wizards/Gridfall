using System;
using Godot;
using System.Collections.Generic;
using Gridfall.Domain;

namespace Gridfall.Characters.Domain
{
	public partial class CharacterBase : Node
	{
		public string CharacterName { get; set; }
		public int Level { get; set; }
		public int Health { get; private set; }
		public int MaxHealth { get; private set; }
		public int Strength { get; set; }
		public int Defense { get; set; }
		public int Resistance { get; set; }
		public int Speed { get; set; }
		public int Luck { get; set; }
		public int Skill {get; set;}
		public int Constitution {get; set;}
		public bool IsMagic {get; set;}
		public int MovementRange { get; set; }
		// Declaring event handler for when exp changes so UI can update
		[Signal]
		public delegate void ExpChangedEventHandler(int exp);
		[Signal]
		public delegate void StatChangedEventHandler(string stat, int value);
		public int Experience { get; set; }
		
		public Weapon EquippedWeapon { get; set; }

		public Dictionary<string, int> GrowthRates { get; set; } = new Dictionary<string, int>
		{
			{ "HP", 0 },
			{ "STR", 0 },
			{ "SPD", 0 },
			{ "DEF", 0 },
			{ "RES", 0 },
			{ "LUCK", 0 },
			{ "SKILL", 0 },
		};

		public void SetupCharacter(string characterName, int level, Weapon weapon, Dictionary<string, int> customGrowthRates = null)
		{
			CharacterName = characterName;
			Level = level;

			MaxHealth = 25 + (level * 5);
			Health = MaxHealth;

			Strength = 5 + level;
			Defense = 3 + level;
			Speed = 4 + level;
			MovementRange = 5;
			
			EquippedWeapon = weapon;
			
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

		public void TakeDamage(int damageAmount)
		{
			Health = Math.Max(0, Health - damageAmount);
		}

		public void Heal(int healAmount)
		{
			Health += healAmount;

			if (Health > MaxHealth)
			{
				Health = MaxHealth;
			}
		}

		public bool IsAlive()
		{
			return Health > 0;
		}
		// Adds exp to character
		public void AddExperience(int amount)
		{
			Experience += amount;
			// let's UI know that exp has been changed
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
					break;
				
				case "SKILL":
					Skill += amount;
					break;
				
			}
		}
	}
}
