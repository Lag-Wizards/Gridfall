using System;

namespace Gridfall.Domain
{
	public partial class Consumable : Equipment
	{
		public string Description { get; set; }

		public Consumable(string name, int price = 0, string description = "")
			: base(name, price)
		{
			Description = description;
		}
	}
}
