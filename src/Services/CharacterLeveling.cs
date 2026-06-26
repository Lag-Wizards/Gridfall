using Godot;
using System;
using Gridfall.Domain;
using Gridfall.Characters.Domain;

namespace Gridfall.Services;
public partial class CharacterLeveling : Node
{
	private ExperienceCalculator experienceCalculator = new ExperienceCalculator();
	private LevelUpManager levelUpManager = new LevelUpManager();

	public override void _Ready()
	{
		experienceCalculator = new ExperienceCalculator();
		levelUpManager = new LevelUpManager();
	}

	public void AwardKillExp(CharacterBase player, CombatUnit enemy)
	{
		int expGained = experienceCalculator.EnemyKilled(player, enemy);
		
		player.AddExperience(expGained);
		
		CheckLevelUp(player);
	}

	public void SuccessfulHitExp(CharacterBase player, CombatUnit enemy)
	{
		int expGained = experienceCalculator.EnemyHitAlive(player, enemy);
		
		player.AddExperience(expGained);
		
		CheckLevelUp(player);
	}

	public void AwardMissedHitExp(CharacterBase player, CombatUnit enemy)
	{
		int expGained = experienceCalculator.MissedHit(player, enemy);

		player.AddExperience(expGained);
		
		CheckLevelUp(player);
	}

	public void BattleExpOutcome(CharacterBase player, CombatUnit enemy, int startingHealth)
	{
		if (enemy.Health == 0)
		{
			AwardKillExp(player, enemy);
		}
		else if (enemy.Health < startingHealth)
		{
			SuccessfulHitExp(player, enemy);
		}
		else
		{
			AwardMissedHitExp(player, enemy);
		}
	}

	private void CheckLevelUp(CharacterBase player)
	{
		while (player.Experience >= 100)
		{
			player.Experience -= 100;
			levelUpManager.LevelUp(player);
		}
	}
}
