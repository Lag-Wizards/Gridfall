using Godot;
using System;
using Gridfall.Characters.Domain;

namespace Gridfall.Services;

public class BattleCalculation
{
	private CharacterLeveling characterLeveling = new CharacterLeveling();

	public CombatReport CalculateStats(CharacterBase attacker, CharacterBase attackee)
	{

		// CALCULATE ATTACK SPEED
		// Weapons heavier than a unit's Constitution slow them down
		int playerWeightPenalty = Math.Max(0, attacker.EquippedWeapon.Weight - attacker.Constitution);
		int playerAS = attacker.Speed - playerWeightPenalty;

		int enemyWeightPenalty = Math.Max(0, attackee.EquippedWeapon.Weight - attackee.Constitution);
		int enemyAS = attackee.Speed - enemyWeightPenalty;

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

		// DETERMINE FINAL DAMAGE
		// Chooses Defense for physical, Resistance for magic
		int enemyMitigation = (attacker.IsMagic)
			? attackee.Resistance
			: attackee.Defense;

		int playerMitigation = (attackee.IsMagic)
			? attacker.Resistance
			: attacker.Defense;

		int playerAttack = attacker.Strength + attacker.EquippedWeapon.Damage;
		int playerDamage = Math.Max(0, playerAttack - enemyMitigation);

		int enemyAttack = attackee.Strength + attackee.EquippedWeapon.Damage;
		int enemyDamage = Math.Max(0, enemyAttack - playerMitigation);

		// DETERMINE DISPLAYED HIT RATE
		int playerAccuracy =
			attacker.EquippedWeapon.HitRate +
			(attacker.Skill * 2) +
			(attacker.Luck / 2);

		int enemyAvoid =
			(enemyAS * 2) +
			attackee.Luck;

		int playerFinalHit =
			Math.Clamp(playerAccuracy - enemyAvoid, 0, 100);

		int enemyAccuracy =
			attackee.EquippedWeapon.HitRate +
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

	public void ExecuteBattle(CharacterBase player, CharacterBase enemy)
	{
		int startingHealth = enemy.Health;

		CombatReport report = CalculateStats(player, enemy);

		RandomNumberGenerator rng = new RandomNumberGenerator();
		rng.Randomize();

		bool PerformAttack(
			CharacterBase attacker,
			CharacterBase defender,
			int damage,
			int hitChance)
		{

			int roll = rng.RandiRange(0, 99);

			if (roll < hitChance)
			{
				defender.TakeDamage(damage);

				return true;
			}
			else
			{
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

		characterLeveling.BattleExpOutcome(
			player,
			enemy,
			startingHealth);
	}
}
