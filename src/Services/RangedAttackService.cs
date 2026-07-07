using Godot;

namespace Gridfall.Services
{
    public class RangedAttackService
    {
        public bool CanAttack(Vector2I attackerPosition, Vector2I targetPosition, int weaponRange)
        {
            int distance =
                Mathf.Abs(attackerPosition.X - targetPosition.X) +
                Mathf.Abs(attackerPosition.Y - targetPosition.Y);

            return distance <= weaponRange;
        }

        public bool IsRangedWeapon(int weaponRange)
        {
            return weaponRange > 1;
        }
    }
}