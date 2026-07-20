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

		// Backing fields for base stats (without equipment bonuses)
		protected int _baseMaxHealth;
		protected int _baseStrength;
		protected int _baseDefense;
		protected int _baseResistance;
		protected int _baseSpeed;
		protected int _baseLuck;
		protected int _baseSkill;
		protected int _baseConstitution;
		protected int _baseMoveDistance;

		public int MaxHealth
		{
			get => _baseMaxHealth + GetStatBonus("HP");
			set => _baseMaxHealth = value - GetStatBonus("HP");
		}

		public int Strength
		{
			get => _baseStrength + GetStatBonus("STR");
			set => _baseStrength = value - GetStatBonus("STR");
		}

		public int Defense
		{
			get => _baseDefense + GetStatBonus("DEF");
			set => _baseDefense = value - GetStatBonus("DEF");
		}

		public int Resistance
		{
			get => _baseResistance + GetStatBonus("RES");
			set => _baseResistance = value - GetStatBonus("RES");
		}

		public int Speed
		{
			get => _baseSpeed + GetStatBonus("SPD");
			set => _baseSpeed = value - GetStatBonus("SPD");
		}

		public int Luck
		{
			get => _baseLuck + GetStatBonus("LUCK");
			set => _baseLuck = value - GetStatBonus("LUCK");
		}

		public int Skill
		{
			get => _baseSkill + GetStatBonus("SKILL");
			set => _baseSkill = value - GetStatBonus("SKILL");
		}

		public int Constitution
		{
			get => _baseConstitution + GetStatBonus("CON");
			set => _baseConstitution = value - GetStatBonus("CON");
		}

		public int MoveDistance
		{
			get => _baseMoveDistance + GetStatBonus("MOV");
			set => _baseMoveDistance = value - GetStatBonus("MOV");
		}

		public bool IsMagic { get; set; }

		public Weapon EquippedWeapon { get; set; }
		public Armor EquippedArmor { get; set; }

		// Public accessors for the base stats (excluding equipment bonuses) for save serialization
		public int BaseMaxHealth => _baseMaxHealth;
		public int BaseStrength => _baseStrength;
		public int BaseDefense => _baseDefense;
		public int BaseResistance => _baseResistance;
		public int BaseSpeed => _baseSpeed;
		public int BaseLuck => _baseLuck;
		public int BaseSkill => _baseSkill;
		public int BaseConstitution => _baseConstitution;
		public int BaseMoveDistance => _baseMoveDistance;

		public int GetStatBonus(string stat)
		{
			int bonus = 0;
			if (EquippedWeapon != null)
			{
				bonus += stat.ToUpper() switch
				{
					"HP" => EquippedWeapon.HpBonus,
					"STR" => EquippedWeapon.StrengthBonus,
					"DEF" => EquippedWeapon.DefenseBonus,
					"SPD" => EquippedWeapon.SpeedBonus,
					"LUCK" => EquippedWeapon.LuckBonus,
					"SKILL" => EquippedWeapon.SkillBonus,
					"RES" => EquippedWeapon.ResistanceBonus,
					"MOV" => EquippedWeapon.MovementBonus,
					"CON" => EquippedWeapon.ConstitutionBonus,
					_ => 0
				};
			}
			if (EquippedArmor != null)
			{
				bonus += stat.ToUpper() switch
				{
					"HP" => EquippedArmor.HpBonus,
					"STR" => EquippedArmor.StrengthBonus,
					"DEF" => EquippedArmor.DefenseBonus,
					"SPD" => EquippedArmor.SpeedBonus,
					"LUCK" => EquippedArmor.LuckBonus,
					"SKILL" => EquippedArmor.SkillBonus,
					"RES" => EquippedArmor.ResistanceBonus,
					"MOV" => EquippedArmor.MovementBonus,
					"CON" => EquippedArmor.ConstitutionBonus,
					_ => 0
				};
			}
			return bonus;
		}

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
		public event Action OnHealthChanged;

		public bool IsAlive() => Health > 0;

		public virtual void TakeDamage(int damageAmount)
		{
			Health = Math.Max(0, Health - damageAmount);
			OnHealthChanged?.Invoke();
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
			OnHealthChanged?.Invoke();
		}
	}
}
