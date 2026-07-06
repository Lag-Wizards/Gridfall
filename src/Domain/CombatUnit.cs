using Godot;
using System;
using System.Collections.Generic;

namespace Gridfall.Domain
{
	public abstract partial class CombatUnit : Node
	{
		[Signal]
		public delegate void StatChangedEventHandler(string stat, int value);

		public string UnitName { get; set; }
		public int Level { get; set; }
		public int Health { get; set; }
		public int MaxHealth { get; protected set; }
		public int Strength { get; set; }
		public int Defense { get; set; }
		public int Resistance { get; set; }
		public int Speed { get; set; }
		public int Luck { get; set; }
		public int Skill { get; set; }
		public int Constitution { get; set; }
		public bool IsMagic { get; set; }
		public int MoveDistance { get; set; }
		public Weapon EquippedWeapon { get; set; }

		public Dictionary<string, int> GrowthRates { get; set; } = new Dictionary<string, int>
		{
			{ "HP", 100 },
			{ "STR", 100 },
			{ "SPD", 100 },
			{ "DEF", 100 },
			{ "RES", 100 },
			{ "LUCK", 100 },
			{ "SKILL", 100 },
		};

		public event Action OnUnitDeath;

		public bool IsAlive() => Health > 0;

		public virtual void TakeDamage(int damageAmount)
		{
			Health = Math.Max(0, Health - damageAmount);
			if (Health == 0)
			{
				OnUnitDeath?.Invoke();
			}
		}

		public virtual void Heal(int healAmount)
		{
			Health = Health + healAmount;
			if (Health > MaxHealth)
			{
				Health = MaxHealth;
			}
		}
	}
}
