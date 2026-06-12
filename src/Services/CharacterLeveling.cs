using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Domain.Enemies;

namespace Gridfall.Services;
public partial class CharacterLeveling : Node
{
	private ExperienceCalculator experienceCalculator;
	private LevelUpManager levelUpManager;

	public override void _Ready()
	{
		experienceCalculator = new ExperienceCalculator();
		levelUpManager = new LevelUpManager();
	}

	public void AwardKillExp(CharacterBase player, CharacterBase enemy)
	{
		int expGained = experienceCalculator.EnemyKilled(player, enemy);

		player.Exp += expGained;

		CheckLevelUp(player);
	}

	private void CheckLevelUp(CharacterBase player)
	{
		while (player.Exp >= 100)
		{
			player.Exp -= 100;

			levelUpManager.LevelUp(player);

		}
	}
}