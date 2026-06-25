using Godot;

public partial class EnemyAI : Node
{
	public string DetermineAction(Vector2I enemyPosition, Vector2I playerPosition)
	{
		int distance =
			Mathf.Abs(enemyPosition.X - playerPosition.X) +
			Mathf.Abs(enemyPosition.Y - playerPosition.Y);

		if (distance <= 1)
		{
			return "Attack";
		}

		return "Move";
	}

	public Vector2I DetermineMove(Vector2I enemyPosition, Vector2I playerPosition)
	{
		Vector2I direction = playerPosition - enemyPosition;

		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
		{
			return new Vector2I(Mathf.Sign(direction.X), 0);
		}

		return new Vector2I(0, Mathf.Sign(direction.Y));
	}

	public string DetermineAbility()
	{
		return "Attack";
	}
}
