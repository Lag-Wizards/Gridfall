using System;

namespace Gridfall.Domain
{
	public partial class Armor : Equipment
	{
		public int Weight { get; init; }

		public Armor(string name, int price = 0, int weight = 0)
			: base(name, price)
		{
			Weight = weight;
		}
	}
}
