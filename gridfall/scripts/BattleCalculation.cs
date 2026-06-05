using Godot;
using System;
namespace Gridfall.scripts;

public partial class BattleCalculation
{

	public CombatReport CalculateStats(Unit player, Unit enemy)
	{

		// CALCULATE ATTACK SPEED
		// Weapons heavier than a unit's Constitution slow them down
		int playerWeightPenalty = Math.Max(0, player.WeaponWeight - player.Con);
		int playerAS = player.Speed - playerWeightPenalty;

		int enemyWeightPenalty = Math.Max(0, enemy.WeaponWeight - enemy.Con);
		int enemyAS = enemy.Speed - enemyWeightPenalty;

		// WEAPON TRIANGLE MODIFIERS
		/*int triangleDamageBonus = 0;
		int triangleHitBonus = 0;

		if (HasTriangleAdvantage(player.WeaponType, enemy.WeaponType))
		{
			triangleDamageBonus = 1;
			triangleHitBonus = 15;
		}
		else if (HasTriangleDisadvantage(player.WeaponType, enemy.WeaponType))
		{
			triangleDamageBonus = -1;
			triangleHitBonus = -15;
		}*/

		// DETERMINE FINAL DAMAGE
		// Chooses Defense for physical, Resistance for magic
		int enemyMitigation = (player.IsMagic) ? enemy.Res : (enemy.Def);
		int playerMitigation = (enemy.IsMagic) ? player.Res : player.Def;

		int playerAttack = player.Strength + player.WeaponAttack; // + triangleDamageBonus;
		int playerDamage = Math.Max(0, playerAttack - enemyMitigation);

		int enemyAttack = enemy.Strength + enemy.WeaponAttack; // - triangleDamageBonus;
		int enemyDamage = Math.Max(0, enemyAttack - playerMitigation);

		// DETERMINE DISPLAYED HIT RATE
		int playerAccuracy = player.WeaponHitRate + (player.Skill * 2) + (player.Luck / 2); // + triangleHitBonus;
		int enemyAvoid = (enemyAS * 2) + enemy.Luck;
		int playerFinalHit = Math.Clamp(playerAccuracy - enemyAvoid, 0, 100);

		int enemyAccuracy = enemy.WeaponHitRate + (enemy.Skill * 2) + (enemy.Luck / 2); // - triangleHitBonus;
		int playerAvoid = (playerAS * 2) + player.Luck;
		int enemyFinalHit = Math.Clamp(enemyAccuracy - playerAvoid, 0, 100);

		// DETERMINE DOUBLE ATTACK STATUS
		bool playerDoubles = (playerAS - enemyAS) >= 4;
		bool enemyDoubles = (enemyAS - playerAS) >= 4;

		
		return new CombatReport(playerDamage, playerFinalHit, playerDoubles, enemyDamage, enemyFinalHit, enemyDoubles);
	}

	public void ExecuteBattle(Unit player, Unit enemy)
    {
        CombatReport report = CalculateStats(player, enemy);
        RandomNumberGenerator rng = new RandomNumberGenerator();
        rng.Randomize(); 
        
        bool PerformAttack(Unit attacker, Unit defender, int damage, int hitChance)
        {
            int roll = rng.RandiRange(0, 99);
            
            if (roll < hitChance)
            {
                defender.Hp = Math.Max(0, defender.Hp - damage); // Prevent HP from dropping below 0
                return true; 
            }
            else
            {
                return false; 
            }
        }
        

        // INITIATOR ATTACKS
        PerformAttack(player, enemy, report.PlayerDamage, report.PlayerFinalHit);
        if (enemy.Hp <= 0)
        {
            return; // End battle early if defender dies
        }

        // DEFENDER COUNTER-ATTACKS
        PerformAttack(enemy, player, report.EnemyDamage, report.EnemyFinalHit);
        if (player.Hp <= 0)
        {
            return; // End battle early if initiator dies
        }

        // FOLLOW-UP ATTACKS
        if (report.PlayerDoubles)
        {
            PerformAttack(player, enemy, report.PlayerDamage, report.PlayerFinalHit);
        }
        else if (report.EnemyDoubles)
        {
            PerformAttack(enemy, player, report.EnemyDamage, report.EnemyFinalHit);
        }
        
    }
}