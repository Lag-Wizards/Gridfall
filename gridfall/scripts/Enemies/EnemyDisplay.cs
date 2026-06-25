using Godot;

public partial class EnemyDisplay : Node2D
{
	[Export]
	public EnemyNode TargetEnemy;

	public override void _Process(double delta)
	{
		if (TargetEnemy != null)
		{
			GlobalPosition = TargetEnemy.GlobalPosition;
		}
	}
}
