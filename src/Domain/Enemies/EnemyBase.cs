using System;
using Godot;
using Gridfall.Contracts;
using Gridfall.Domain;
using Gridfall.Services;

namespace Gridfall.Domain.Enemies
{
	public abstract partial class EnemyBase : CombatUnit, IEnemy
	{
		public string Name
		{
			get => UnitName;
			protected set => UnitName = value;
		}

		public int MaxHp
		{
			get => MaxHealth;
			protected set => MaxHealth = value;
		}

		public int CurrentHp
		{
			get => Health;
			protected set => Health = value;
		}

		public int Attack
		{
			get => Strength;
			protected set => Strength = value;
		}

		public bool IsAlive => Health > 0;

		protected EnemyBase(string name, int level, int maxHp, int attack, int defense, int speed, int skill, int constitution, int resistance, int luck, Weapon equippedWeapon)
		{
			UnitName = name;
			Level = Math.Max(1, level);
			MaxHealth = Math.Max(1, maxHp);
			Health = MaxHealth;
			Strength = Math.Max(0, attack);
			Defense = Math.Max(0, defense);
			Speed = Math.Max(0, speed);
			Skill = Math.Max(0, skill);
			Constitution = Math.Max(0, constitution);
			Resistance = Math.Max(0, resistance);
			Luck = Math.Max(0, luck);
			EquippedWeapon = equippedWeapon;
		}

		public virtual void TakeDamage(int amount, bool isMagic)
		{
			int dmg = isMagic ? Math.Max(0, amount - Resistance) : Math.Max(0, amount - Defense);
			TakeDamage(dmg);
		}

		public override void TakeDamage(int damageAmount)
		{
			base.TakeDamage(damageAmount);
			if (Health == 0)
			{
				OnDeath();
			}
		}

		public virtual int CalculateDamageTo(IEnemy target)
		{
			var raw = Attack;
			var dmg = Math.Max(0, raw - target.Defense);
			return dmg;
		}

		public virtual void LevelUp(int toLevel)
		{
			if (toLevel <= Level) return;
			while (Level < toLevel)
			{
				Level++;
				MaxHealth += 5;
				Strength += 1;
				Defense += 1;
			}
			Health = MaxHealth;
		}

		protected virtual void OnDeath()
		{
			Events.EmitEnemyDied();
		}
	}
}
