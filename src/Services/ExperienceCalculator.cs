using Gridfall.Characters.Domain;
using Gridfall.Domain;

namespace Gridfall.Services;

public class ExperienceCalculator
{
	public int EnemyKilled(CharacterBase player, CombatUnit enemy)
	{
		return 40;
	}
	public int MissedHit(CharacterBase player, CombatUnit enemy)
	{
		return 5;
	}
	public int EnemyHitAlive(CharacterBase player, CombatUnit enemy)
	{
		return 20;
	}
}
