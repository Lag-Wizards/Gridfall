using System;
using Gridfall.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Domain.Enemies
{
	public partial class NormalSlime : Slime
	{
		private const double StatBoostMultiplier = 1.30;

		public NormalSlime(int level = 1) : base(level)
		{
			Name = "Normal Slime";
			ApplyStatBoost(level);
		}

		private void ApplyStatBoost(int level)
		{
			MaxHealth = (int)Math.Ceiling((12 + (level - 1) * 4) * StatBoostMultiplier);
			Strength = (int)Math.Ceiling((2 + (level - 1) * 1) * StatBoostMultiplier);
			Defense = (int)Math.Ceiling((0 + (level - 1) * 0) * StatBoostMultiplier);
			Speed = (int)Math.Ceiling((1 + (level - 1) * 1) * StatBoostMultiplier);
			Skill = (int)Math.Ceiling((1 + (level - 1) * 1) * StatBoostMultiplier);
			Constitution = (int)Math.Ceiling((1 + (level - 1) * 1) * StatBoostMultiplier);
			Resistance = (int)Math.Ceiling((1 + (level - 1) * 1) * StatBoostMultiplier);
			Luck = (int)Math.Ceiling((1 + (level - 1) * 1) * StatBoostMultiplier);
			MoveDistance = (int)Math.Ceiling(4 * StatBoostMultiplier);
			Health = MaxHealth;
		}

		public override void TakeDamage(int amount, bool isMagic)
		{
			base.TakeDamage(amount, isMagic);
			if (IsAlive)
			{
				Heal(2);
			}
		}
	}
}
