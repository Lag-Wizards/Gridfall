using System;

namespace Gridfall.Enemies
{
    public class Goblin : EnemyBase
    {
        public Goblin(int level = 1) : base("Goblin", level, maxHp: 20 + (level-1)*5, attack: 4 + (level-1)*1, defense: 1 + (level-1)*1)
        {
        }

        public override int CalculateDamageTo(IEnemy target)
        {
            // Goblins have a small chance to crit
            var baseDmg = base.CalculateDamageTo(target);
            var rand = new Random();
            if (rand.NextDouble() < 0.12) // 12% crit
                return (int)(baseDmg * 1.5);
            return baseDmg;
        }
    }
}
