using Godot;
using System;
using Gridfall.Domain.Enums;

public partial class Weapon
{
	public WeaponType Type { get; init; }
	public  string Name { get; init; }
	public int Damage { get; init; }
	public int Range { get; init; }
	
	public int Weight { get; init; }
	
	public int HitRate { get; init; }
	
	
	public Weapon(WeaponType type, string name, int damage, int range, int weight, int hitRate) 
	{
		Type = type;
		Name = name;
		Damage = damage;
		Range = range;
		Weight = weight;
		HitRate = hitRate;
	}
}
