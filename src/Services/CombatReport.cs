namespace Gridfall.Services;

public struct CombatReport
{
	public int PlayerDamage;
	public int PlayerFinalHit;
	public bool PlayerDoubles;

	public int EnemyDamage;
	public int EnemyFinalHit;
	public bool EnemyDoubles;

	public CombatReport(
		int playerDamage,
		int playerFinalHit,
		bool playerDoubles,
		int enemyDamage,
		int enemyFinalHit,
		bool enemyDoubles)
	{
		PlayerDamage = playerDamage;
		PlayerFinalHit = playerFinalHit;
		PlayerDoubles = playerDoubles;

		EnemyDamage = enemyDamage;
		EnemyFinalHit = enemyFinalHit;
		EnemyDoubles = enemyDoubles;
	}
}
