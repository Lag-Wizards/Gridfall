using System.Collections.Generic;
using Godot;
using Gridfall.Contracts;
using Gridfall.Domain;

namespace Gridfall.Services;

public class EnemyAI
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
	
	private List<Vector2I> GetNeighbors(Vector2I tile)
	{
		return new List<Vector2I>
		{
			tile + Vector2I.Up,
			tile + Vector2I.Down,
			tile + Vector2I.Left,
			tile + Vector2I.Right
		};
	}
	
	private bool IsWalkable( Vector2I tile, Vector2I goal, IGridManager grid)
	{
		TileState state = grid.GetTileStateAt(tile);

		// Outside the map
		if (state == null)
			return false;

		// Wall or blocked terrain
		if (!state.IsWalkable)
			return false;

		// Allow the goal tile even if occupied
		if (tile != goal && state.CurrentOccupant != null)
			return false;

		return true;
	}
	
	private int Heuristic(Vector2I a, Vector2I b)
	{
		return Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y);
	}

	private List<Vector2I> TracePath(PathNode goalNode)
	{
		List<Vector2I> path = new();

		PathNode current = goalNode;

		while (current != null)
		{
			path.Add(current.Position);
			current = current.Parent;
		}

		path.Reverse();

		return path;
	}
	
	public List<Vector2I> FindPath(Vector2I start, Vector2I goal, IGridManager grid)
        {
            List<PathNode> openList = new();
            HashSet<Vector2I> closedList = new();

            PathNode startNode = new(start)
            {
                GCost = 0,
                HCost = Heuristic(start, goal)
            };

            openList.Add(startNode);

            while (openList.Count > 0)
            {
                // Find node with the lowest F cost
                PathNode current = openList[0];

                foreach (PathNode node in openList)
                {
                    if (node.FCost < current.FCost || (node.FCost == current.FCost && node.HCost < current.HCost))
                    {
                        current = node;
                    }
                }
                
                if (current.Position == goal)
                {
                    return TracePath(current);
                }

                openList.Remove(current);
                closedList.Add(current.Position);

                foreach (Vector2I neighbor in GetNeighbors(current.Position))
                {
                    // Already searched
                    if (closedList.Contains(neighbor))
                        continue;

                    // Ignore walls or occupied tiles
                    if (!IsWalkable(neighbor, goal, grid))
                        continue;

                    TileState neighborState = grid.GetTileStateAt(neighbor);
                    int costToNeighbor = current.GCost + neighborState.MovementCost;

                    PathNode existing = openList.Find(n => n.Position == neighbor);

                    if (existing == null)
                    {
                        existing = new PathNode(neighbor)
                        {
                            Parent = current,
                            GCost = costToNeighbor,
                            HCost = Heuristic(neighbor, goal)
                        };

                        openList.Add(existing);
                    }
                    else if (costToNeighbor < existing.GCost)
                    {
                        existing.Parent = current;
                        existing.GCost = costToNeighbor;
                    }
                }
            }
            // No path found
            return new List<Vector2I>();
        }
}
