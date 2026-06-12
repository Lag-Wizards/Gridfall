using System;
using Gridfall.Characters.Domain;

namespace Gridfall.Services;

public class LevelUpManager
{
    // CALL on Character level up
    public void LevelUp(CharacterBase player)
    {
        player.Level++;
        // random num gen
        Random random = new Random();
        
        // add dictonary for growthrates for character
        // adjust stats based on growthrates
        foreach (var (stat, growthRate) in player.GrowthRates)
        {
            int rn = random.Next(0, 99);
            if (rn < growthRate)
            {
                // STATS++ Note: add more stats later??
                switch (stat)
                {
                    case "HP":
                        player.MaxHealth++;
                        break;

                    case "STR":
                        player.Strength++;
                        break;

                    case "SPD":
                        player.Speed++;
                        break;

                    case "DEF":
                        player.Defense++;
                        break;
                }
            }
        }
    }
}