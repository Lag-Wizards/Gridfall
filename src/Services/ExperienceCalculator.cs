using Gridfall.Characters.Domain;
using Gridfall.Domain;

namespace Gridfall.Services;

public class ExperienceCalculator
{
	public int EnemyKilled(CharacterBase player, CombatUnit enemy)
	{
		return 50 + (enemy.Level * 10);
	}
	public int MissedHit(CharacterBase player, CombatUnit enemy)
	{
		return 7 + (enemy.Level);
	}
	public int EnemyHitAlive(CharacterBase player, CombatUnit enemy)
	{
		return 30 + (enemy.Level * 2);
	}
}
