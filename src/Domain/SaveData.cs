using System;

namespace Gridfall.Domain
{
	// DTO used for persisting and loading simple character save data.
	public class SaveData
	{
		public string CharacterName { get; set; }
		public int Level { get; set; }
		public int Health { get; set; }
		public int MaxHealth { get; set; }
		public int Strength { get; set; }
		public int Defense { get; set; }
		public int Speed { get; set; }
		public int MovementRange { get; set; }

		// Weapon fields are serialized as ints/values to keep the DTO simple.
		public int WeaponType { get; set; }
		public int WeaponDamage { get; set; }
		public int WeaponRange { get; set; }
	}
}
