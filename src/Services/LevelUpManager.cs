using System;
using Gridfall.Characters.Domain;

namespace Gridfall.Services;


public class LevelUpManager
{
    // random num gen
    Random random = new Random();
    
    // CALL on Character level up
    public void LevelUp(CharacterBase player)
    {
        // add dictonary for growthrates for character??
        // adjust stats based on growthrates
        foreach (var (stat, growthRate) in player.GrowthRates)
        {
            int rn = random.Next(0, 99);
            if (rn < growthRate)
            {
                player.ModifyStat(stat, 1);
            }
        }
    }
}