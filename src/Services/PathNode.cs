using Godot;

namespace Gridfall.Services;

public class PathNode
{
	public Vector2I Position;
	
	// Distance from start
	public int GCost;
	// Estimated distance to goal
	public int HCost;
	public int FCost => GCost + HCost;

	public PathNode Parent;

	public PathNode(Vector2I position)
	{
		Position = position;
	}
}
