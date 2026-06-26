using System;
using Gridfall.Contracts;
using Gridfall.Domain.Enums;
using Gridfall.Domain;

namespace Gridfall.Domain.Enemies
{
	public partial class Goblin : EnemyBase
	{
		public Goblin(int level = 1) : base("Goblin", level, maxHp: 20 + (level-1)*5, attack: 4 + (level-1)*1, defense: 1 + (level-1)*1, speed: 1 + (level-1)*1, skill: 1 + (level-1)*1, constitution: 1 + (level-1)*1, resistance: 1 + (level-1)*1, luck: 1 + (level-1)*1, equippedWeapon: new Weapon(WeaponType.Slash, 10, 1))
		{
		}

		public override int CalculateDamageTo(IEnemy target)
		{
			// Goblins have a small chance to crit
			var baseDmg = base.CalculateDamageTo(target);
			var rand = new Random();
			if (rand.NextDouble() < 0.12) // 12% crit
				return (int)(baseDmg * 1.5);
			return baseDmg;
		}
	}
}
