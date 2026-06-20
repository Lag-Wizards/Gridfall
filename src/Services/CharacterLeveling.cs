using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Domain.Enemies;

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

	public void AwardKillExp(CharacterBase player, CharacterBase enemy)
	{
		int expGained = experienceCalculator.EnemyKilled(player, enemy);
		
		player.AddExperience(expGained);
		
		CheckLevelUp(player);
	}

	public void SuccessfulHitExp(CharacterBase player, CharacterBase enemy)
	{
		int expGained = experienceCalculator.EnemyHitAlive(player, enemy);
		
		player.AddExperience(expGained);
		
		CheckLevelUp(player);
	}

	public void AwardMissedHitExp(CharacterBase player, CharacterBase enemy)
	{

		int expGained = experienceCalculator.MissedHit(player, enemy);

		player.AddExperience(expGained);
		
		CheckLevelUp(player);
	}

	public void BattleExpOutcome(CharacterBase attacker, CharacterBase attackee, int startingHealth)
	{

		if (attackee.Health == 0)
		{
			AwardKillExp(attacker, attackee);
		}
		else if (attackee.Health < startingHealth)
		{
			SuccessfulHitExp(attacker, attackee);
		}
		else
		{
			AwardMissedHitExp(attacker, attackee);
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
