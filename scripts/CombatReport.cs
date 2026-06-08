namespace Gridfall.scripts;

public class CombatReport
{
    public int PlayerDamage { get; set; }
    public int PlayerFinalHit { get; set; }
    public bool PlayerDoubles { get; set; }
    public int EnemyDamage { get; set; }
    public int EnemyFinalHit { get; set; }
    public bool EnemyDoubles { get; set; }

    public CombatReport(int playerDamage, int playerFinalHit, bool playerDoubles, int enemyDamage, int enemyFinalHit, bool enemyDoubles)
    {
        PlayerDamage = playerDamage;
        PlayerFinalHit = playerFinalHit;
        PlayerDoubles = playerDoubles;
        EnemyDamage = enemyDamage;
        EnemyFinalHit = enemyFinalHit;
        EnemyDoubles = enemyDoubles;
    }

}