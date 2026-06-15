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
		public int Speed { get; set; }
		public int MovementRange { get; set; }
		// Declaring event handler for when exp changes so UI can update
		[Signal]
		public delegate void ExpChangedEventHandler(int exp);
		[Signal]
		public delegate void StatChangedEventHandler(string stat, int value);
		public int Experience { get; set; }
		
		public Weapon EquippedWeapon { get; set; }

		public void SetupCharacter(string characterName, int level, Weapon weapon)
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
		}

		public void TakeDamage(int damageAmount)
		{
			int finalDamage = damageAmount - Defense;

			if (finalDamage < 1)
			{
				finalDamage = 1;
			}

			Health -= finalDamage;

			if (Health < 0)
			{
				Health = 0;
			}
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
			}
		}
	}
}
