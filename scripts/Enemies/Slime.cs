using System;

namespace Gridfall.Enemies
{
    public class Slime : EnemyBase
    {
        public Slime(int level = 1) : base("Slime", level, maxHp: 12 + (level-1)*4, attack: 2 + (level-1)*1, defense: 0 + (level-1)*0)
        {
        }

        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);
            // Slimes regenerate a bit when hit
            if (IsAlive)
                Heal(1);
        }
    }
}
