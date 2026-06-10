using System;
using Gridfall.Contracts;

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
		public bool IsAlive => CurrentHp > 0;

		protected EnemyBase(string name, int level, int maxHp, int attack, int defense)
		{
			Name = name;
			Level = Math.Max(1, level);
			MaxHp = Math.Max(1, maxHp);
			CurrentHp = MaxHp;
			Attack = Math.Max(0, attack);
			Defense = Math.Max(0, defense);
		}

		public virtual void TakeDamage(int amount)
		{
			var dmg = Math.Max(0, amount - Defense);
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
