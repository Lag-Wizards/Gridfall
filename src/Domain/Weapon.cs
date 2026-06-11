using Godot;
using System;
using Gridfall.Domain.Enums;

public partial class Weapon
{
	public WeaponType Type { get; init; }
	public int Damage { get; init; }
	public int Range { get; init; }
	
	public Weapon(WeaponType type, int damage, int range) 
	{
		Type = type;
		Damage = damage;
		Range = range;
	}
}
