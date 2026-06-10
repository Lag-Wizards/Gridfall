using Godot;
using System.Collections.Generic;

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

		public List<string> Abilities { get; private set; } = new();

		public void SetupCharacter(string characterName, int level)
		{
			CharacterName = characterName;
			Level = level;

			MaxHealth = 25 + (level * 5);
			Health = MaxHealth;

			Strength = 5 + level;
			Defense = 3 + level;
			Speed = 4 + level;
			MovementRange = 5;
		}

		public void AddAbility(string abilityName)
		{
			if (!Abilities.Contains(abilityName))
			{
				Abilities.Add(abilityName);
			}
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
	}
}
