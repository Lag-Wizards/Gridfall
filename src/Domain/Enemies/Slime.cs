using System;
using Gridfall.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Domain.Enemies
{
	public partial class Slime : EnemyBase
	{
		public Slime(int level = 1) : base("Slime", level, maxHp: 12 + (level-1)*4, attack: 2 + (level-1)*1, defense: 0 + (level-1)*0, speed: 1 + (level-1)*1, skill: 1 + (level-1)*1, constitution: 1 + (level-1)*1, resistance: 1 + (level-1)*1, luck: 1 + (level-1)*1, equippedWeapon: new Weapon(WeaponType.Magic, 10, 1), moveDistance: 4)
		{
		}

		public override void TakeDamage(int amount, bool isMagic)
		{
			base.TakeDamage(amount, isMagic);
			// Slimes regenerate a bit when hit
			if (IsAlive)
				Heal(1);
		}
	}
}
