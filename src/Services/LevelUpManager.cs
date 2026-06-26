using System;
using Godot;
using Gridfall.Characters.Domain;

public class LevelUpManager
{
	// random num gen
	Random random = new Random();

	// CALL on Character level up
	public void LevelUp(CharacterBase player)
	{
		player.ModifyStat("Level", 1);

		// adjust stats based on growthrates
		foreach (var (stat, growthRate) in player.GrowthRates)
		{
			int rn = random.Next(0, 99);
			if (rn < growthRate)
			{
				player.ModifyStat(stat, 1);
			}
		}
		player.Heal(player.MaxHealth);
	}
}
