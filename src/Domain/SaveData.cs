namespace Gridfall.Domain;

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

	public int WeaponType { get; set; }
	public int WeaponDamage { get; set; }
	public int WeaponRange { get; set; }
}
