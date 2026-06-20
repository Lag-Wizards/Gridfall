using Godot;
using System;
using System.Collections.Generic;
using Gridfall.Characters.Domain;
using Gridfall.Domain.Enums;
using Gridfall.Services;

public partial class Battletest : Node2D
{
	public override void _Ready()
	{
		GD.Print("BattleTest started");
		// Create weapons
		Weapon ironSword = new Weapon(
			WeaponType.Melee,
			"Iron Sword",
			5,
			1,
			5,
			90
		);

		Weapon ironAxe = new Weapon(
			WeaponType.Melee,
			"Iron Axe",
			8,
			1,
			10,
			75
		);

		// Hero growth rates
		var heroGrowths = new Dictionary<string, int>
		{
			{ "HP", 80 },
			{ "STR", 55 },
			{ "SPD", 65 },
			{ "DEF", 35 },
			{ "RES", 30 },
			{ "LUCK", 45 },
			{ "SKILL", 60 },
		};

		// Bandit growth rates
		var banditGrowths = new Dictionary<string, int>
		{
			{ "HP", 90 },
			{ "STR", 70 },
			{ "SPD", 30 },
			{ "DEF", 45 },
			{ "RES", 10 },
			{ "LUCK", 20 },
			{ "SKILL", 35 },
		};

		// Create hero
		CharacterBase hero = new CharacterBase();
		hero.SetupCharacter(
			"Hero",
			5,
			ironSword,
			heroGrowths
		);

		hero.Skill = 8;
		hero.Luck = 6;
		hero.Constitution = 7;
		hero.Resistance = 4;

		// Create enemy
		CharacterBase bandit = new CharacterBase();
		bandit.SetupCharacter(
			"Bandit",
			4,
			ironAxe,
			banditGrowths
		);

		bandit.Skill = 4;
		bandit.Luck = 2;
		bandit.Constitution = 10;
		bandit.Resistance = 1;

		AddChild(hero);
		AddChild(bandit);

		GD.Print($"Attacker: {hero.CharacterName}");
		GD.Print($"Defender: {bandit.CharacterName}");
		
		BattleSystem battleSystem = new BattleSystem();
		battleSystem.InitiateCombat(hero, bandit);
	}
}
