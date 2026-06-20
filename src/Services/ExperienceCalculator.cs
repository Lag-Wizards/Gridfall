using Gridfall.Characters.Domain;

namespace Gridfall.Services;

public class ExperienceCalculator
{
    // CURRENTLY PARAMETERS DO NOTHING, IF APPLICABLE DOWN THE LINE, ADJUST CORRESPONDING EXP GAINED BASED ON LEVEL DISPARITY?
    // On kill gain 31 exp
    public int EnemyKilled(CharacterBase player, CharacterBase enemy)
    {
        return 31;
    }
    // On miss hit gain 3 exp
    public int MissedHit(CharacterBase player, CharacterBase enemy)
    {
        return 3;
    }
    // If successful hit gain 10 exp
    public int EnemyHitAlive(CharacterBase player, CharacterBase enemy)
    {
        return 10;
    }
}