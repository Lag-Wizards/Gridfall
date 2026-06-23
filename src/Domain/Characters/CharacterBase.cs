using Godot;
using System;
using System.Collections.Generic;
using Gridfall.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Domain;
public partial class CharacterBase : Node2D
{
	public string CharacterName { get; set; }
	public int Level { get; set; }

	public int Health { get; private set; }
	public int MaxHealth { get; private set; }

	public int Strength { get; set; }
	public int Defense { get; set; }
	public int Speed { get; set; }
	public int MovementRange { get; set; } 
	
	public static event Action OnPlayerDeath;
	
	public Dictionary<string, int> GrowthRates { get; set; } = new Dictionary<string, int>
	{
		{"HP", 50},
		{"STR", 50},
		{"DEF", 50},
		{"SPD", 50}
	};

	[Signal]
	public delegate void ExpChangedEventHandler(int exp);

	[Signal]
	public delegate void StatChangedEventHandler(string stat, int value);

	public int Experience { get; set; }
	
	public Weapon EquippedWeapon { get; set; }

	public void SetupCharacter(string characterName, int level, Weapon weapon)
	{
		CharacterName = characterName;
		Level = level;

		MaxHealth = 25 + (level * 5);
		Health = MaxHealth;

		Strength = 5 + level;
		Defense = 3 + level;
		Speed = 4 + level;
		MovementRange = 5;
		
		EquippedWeapon = weapon;
	}

	public void LoadFromSave(SaveData saveData)
	{
		CharacterName = saveData.CharacterName;
		Level = saveData.Level;
		Health = saveData.Health;
		MaxHealth = saveData.MaxHealth;
		Strength = saveData.Strength;
		Defense = saveData.Defense;
		Speed = saveData.Speed;
		MovementRange = saveData.MovementRange;

		EquippedWeapon = new Weapon(
			(WeaponType)saveData.WeaponType,
			saveData.WeaponDamage,
			saveData.WeaponRange
		);
	}
	
	public override async void _Ready()
	{
		Weapon weapon = new Weapon(WeaponType.Slash, 10, 1);
		SetupCharacter("Character A", 1, weapon);
		GD.Print(CharacterName);
		GD.Print(Level);
		GD.Print(Health);
		
		GD.Print("Game over in 3 seconds (Triggered in Characterbase _Ready)");
		await ToSignal(GetTree().CreateTimer(3.0f), SceneTreeTimer.SignalName.Timeout);
		TakeDamage(34);
		GD.Print(Health);
	}

	public void TakeDamage(int damageAmount)
	{
		int finalDamage = damageAmount - Defense;

		if (finalDamage < 1)
		{
			finalDamage = 1;
		}

		Health -= finalDamage;

		if (Health <= 0)
		{
			Health = 0;
			OnPlayerDeath.Invoke();
		}
	}

	public void Heal(int healAmount)
	{
		Health += healAmount;

		if (Health > MaxHealth)
		{
			Health = MaxHealth;
		}
	}

	public bool IsAlive()
	{
		return Health > 0;
	}

	public void AddExperience(int amount)
	{
		Experience += amount;
		EmitSignal(SignalName.ExpChanged, Experience);
	}
	
	public void ModifyStat(string stat, int amount)
	{
		switch (stat)
		{
			case "Level":
				Level += amount;
				EmitSignal(SignalName.StatChanged, stat, Level);
				break;
			
			case "HP":
				MaxHealth += amount;
				EmitSignal(SignalName.StatChanged, stat, MaxHealth);
				break;

			case "STR":
				Strength += amount;
				EmitSignal(SignalName.StatChanged, stat, Strength);
				break;

			case "SPD":
				Speed += amount;
				EmitSignal(SignalName.StatChanged, stat, Speed);
				break;

			case "DEF":
				Defense += amount;
				EmitSignal(SignalName.StatChanged, stat, Defense);
				break;
		}
	}
}