using System;

namespace Gridfall.Domain
{
	public abstract partial class Equipment
	{
		public string Name { get; set; }
		public int Price { get; set; }

		// Stat Bonuses
		public int HpBonus { get; set; }
		public int StrengthBonus { get; set; }
		public int DefenseBonus { get; set; }
		public int SpeedBonus { get; set; }
		public int LuckBonus { get; set; }
		public int SkillBonus { get; set; }
		public int ResistanceBonus { get; set; }
		public int MovementBonus { get; set; }
		public int ConstitutionBonus { get; set; }

		protected Equipment(string name, int price)
		{
			Name = name;
			Price = price;
		}
	}
}
