using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Services;

public class BattleCalculation
{
	private CharacterLeveling characterLeveling = new CharacterLeveling();

	public CombatReport CalculateStats(CombatUnit attacker, CombatUnit attackee)
	{
		Weapon attackerWeapon = attacker.EquippedWeapon ?? new Weapon(WeaponType.Slash, "Fists", 3, 1, 0, 100);
		Weapon attackeeWeapon = attackee.EquippedWeapon ?? new Weapon(WeaponType.Slash, "Fists", 0, 1, 0, 100);

		// CALCULATE ATTACK SPEED
		// Weapons heavier than a unit's Constitution slow them down

		// WEAPON TRIANGLE MODIFIERS
		int triangleDamageBonus = 0;
		int triangleHitBonus = 0;

		/*
		if (HasTriangleAdvantage(attacker.EquippedWeapon.Type, attackee.EquippedWeapon.Type))
		{
			triangleDamageBonus = 1;
			triangleHitBonus = 15;
		}
		else if (HasTriangleDisadvantage(attacker.EquippedWeapon.Type, attackee.EquippedWeapon.Type))
		{
			triangleDamageBonus = -1;
			triangleHitBonus = -15;
		}
		*/
		int playerWeightPenalty = Math.Max(0, attackerWeapon.Weight - attacker.Constitution);
		int playerAS = attacker.Speed - playerWeightPenalty;

		int enemyWeightPenalty = Math.Max(0, attackeeWeapon.Weight - attackee.Constitution);
		int enemyAS = attackee.Speed - enemyWeightPenalty;

		// DETERMINE FINAL DAMAGE
		// Chooses Defense for physical, Resistance for magic
		int enemyMitigation = (attacker.IsMagic)
			? attackee.Resistance
			: attackee.Defense;

		int playerMitigation = (attackee.IsMagic)
			? attacker.Resistance
			: attacker.Defense;

		int playerAttack = attacker.Strength + attackerWeapon.Damage;
		int playerDamage = Math.Max(0, playerAttack - enemyMitigation);

		int enemyAttack = attackee.Strength + attackeeWeapon.Damage;
		int enemyDamage = Math.Max(0, enemyAttack - playerMitigation);

		// DETERMINE DISPLAYED HIT RATE
		int playerAccuracy =
			attackerWeapon.HitRate +
			(attacker.Skill * 2) +
			(attacker.Luck / 2);

		int enemyAvoid =
			(enemyAS * 2) +
			attackee.Luck;

		int playerFinalHit =
			Math.Clamp(playerAccuracy - enemyAvoid, 0, 100);

		int enemyAccuracy =
			attackeeWeapon.HitRate +
			(attackee.Skill * 2) +
			(attackee.Luck / 2);

		int playerAvoid =
			(playerAS * 2) +
			attacker.Luck;

		int enemyFinalHit =
			Math.Clamp(enemyAccuracy - playerAvoid, 0, 100);

		// DETERMINE DOUBLE ATTACK STATUS
		bool playerDoubles = (playerAS - enemyAS) >= 4;
		bool enemyDoubles = (enemyAS - playerAS) >= 4;
		
		return new CombatReport(
			playerDamage,
			playerFinalHit,
			playerDoubles,
			enemyDamage,
			enemyFinalHit,
			enemyDoubles
		);
	}

	public void ExecuteBattle(CombatUnit player, CombatUnit enemy)
	{
		int startingHealth = enemy.Health;

		CombatReport report = CalculateStats(player, enemy);

		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();

		GD.Print($"--- Combat Started: {player.UnitName} vs {enemy.UnitName} ---");

		bool PerformAttack(
			CombatUnit attacker,
			CombatUnit defender,
			int damage,
			int hitChance)
		{
			int roll = rng.RandiRange(0, 99);

			if (roll < hitChance)
			{
				defender.TakeDamage(damage);
				GD.Print($"{attacker.UnitName} hits {defender.UnitName} for {damage} damage! ({defender.UnitName} HP is now {defender.Health})");
				return true;
			}
			else
			{
				GD.Print($"{attacker.UnitName} missed {defender.UnitName}!");
				return false;
			}
		}

		// INITIATOR ATTACKS
		PerformAttack(
			player,
			enemy,
			report.PlayerDamage,
			report.PlayerFinalHit);

		if (enemy.Health <= 0)
		{
			if (player is CharacterBase playerChar)
			{
				characterLeveling.BattleExpOutcome(
					playerChar,
					enemy,
					startingHealth);
			}
			return;
		}

		// DEFENDER COUNTER-ATTACKS
		PerformAttack(
			enemy,
			player,
			report.EnemyDamage,
			report.EnemyFinalHit);

		if (player.Health <= 0)
		{
			return;
		}

		// FOLLOW-UP ATTACKS
		if (report.PlayerDoubles)
		{
			PerformAttack(
				player,
				enemy,
				report.PlayerDamage,
				report.PlayerFinalHit);
		}
		else if (report.EnemyDoubles)
		{
			PerformAttack(
				enemy,
				player,
				report.EnemyDamage,
				report.EnemyFinalHit);
		}

		if (player is CharacterBase playerCharFinal)
		{
			characterLeveling.BattleExpOutcome(
				playerCharFinal,
				enemy,
				startingHealth);
		}
	}
}
