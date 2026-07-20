using Godot;
using System;
using Gridfall.Domain.Enums;

namespace Gridfall.Domain
{
	public partial class Weapon : Equipment
	{
		public WeaponType Type { get; init; }
		public int Damage { get; init; }
		public int Range { get; init; }
		public int Weight { get; init; }
		public int HitRate { get; init; }

		public Weapon(WeaponType type, string name, int damage, int range, int weight, int hitRate, int price = 0)
			: base(name, price)
		{
			Type = type;
			Damage = damage;
			Range = range;
			Weight = weight;
			HitRate = hitRate;
		}

		public Weapon(WeaponType type, int damage, int range)
			: this(type, "Equipped Weapon", damage, range, 0, 80, 0)
		{
		}
	}
}
