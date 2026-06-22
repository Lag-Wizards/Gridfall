using System;
using Gridfall.Contracts;
using Gridfall.Domain;
namespace Gridfall.Domain.Enemies
{
	public abstract class EnemyBase : IEnemy
	{
		public string Name { get; protected set; }
		public int Level { get; protected set; }
		public int MaxHp { get; protected set; }
		public int CurrentHp { get; protected set; }
		public int Attack { get; protected set; }
		public int Defense { get; protected set; }
		public int Speed { get; protected set; }
		public int Skill { get; protected set; }
		public int Constitution { get; protected set; }
		public int Resistance { get; protected set; }
		public int Luck { get; protected set; }
		public Weapon EquippedWeapon { get; protected set; }
		public bool IsAlive => CurrentHp > 0;

		protected EnemyBase(string name, int level, int maxHp, int attack, int defense, int speed, int skill, int constitution, int resistance, int luck, Weapon equippedWeapon)
		{
			Name = name;
			Level = Math.Max(1, level);
			MaxHp = Math.Max(1, maxHp);
			CurrentHp = MaxHp;
			Attack = Math.Max(0, attack);
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
			int dmg;
			if (!isMagic)
			{
				dmg = Math.Max(0, amount - Defense);
			}
			else 
			{
				dmg = Math.Max(0, amount - Resistance);
			}
			CurrentHp = Math.Max(0, CurrentHp - dmg);
			if (CurrentHp == 0)
				OnDeath();
		}

		public virtual int CalculateDamageTo(IEnemy target)
		{
			var raw = Attack;
			var dmg = Math.Max(0, raw - target.Defense);
			return dmg;
		}

		public virtual void Heal(int amount)
		{
			CurrentHp = Math.Min(MaxHp, CurrentHp + Math.Max(0, amount));
		}

		public virtual void LevelUp(int toLevel)
		{
			if (toLevel <= Level) return;
			while (Level < toLevel)
			{
				Level++;
				MaxHp += 5;
				Attack += 1;
				Defense += 1;
			}
			CurrentHp = MaxHp;
		}

		protected virtual void OnDeath()
		{
			// Hook for subclasses or node wrappers
		}
	}
}
