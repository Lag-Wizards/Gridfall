using System;
using Gridfall.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Domain.Enemies
{
	public partial class Orc : EnemyBase
	{
		public Orc(int level = 1) : base("Orc", level, maxHp: 24 + (level-1)*6, attack: 6 + (level-1)*2, defense: 1 + (level-1)*1, speed: 1 + (level-1)*1, skill: 1 + (level-1)*1, constitution: 1 + (level-1)*1, resistance: 1 + (level-1)*1, luck: 1 + (level-1)*1, equippedWeapon: new Weapon(WeaponType.Blunt, 10, 1), moveDistance: 2)
		{
		}

		public override void TakeDamage(int amount, bool isMagic)
		{
			amount = amount - 1;
			base.TakeDamage(amount, isMagic);
			if (IsAlive)
				Heal(1);
		}
	}
}
