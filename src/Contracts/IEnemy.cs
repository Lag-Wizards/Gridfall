using System;

namespace Gridfall.Contracts
{
	public interface IEnemy
	{
		string Name { get; }
		int Level { get; }
		int MaxHp { get; }
		int CurrentHp { get; }
		int Attack { get; }
		int Defense { get; }
		bool IsAlive { get; }

		void TakeDamage(int amount, bool isMagic);
		int CalculateDamageTo(IEnemy target);
		void Heal(int amount);
		void LevelUp(int toLevel);
	}
}
