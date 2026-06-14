using Godot;
using System.Collections.Generic;
using System.Linq;
using Gridfall.Contracts;
using Gridfall.Domain;

namespace Gridfall.Services;

// Simple grid movement service implementing BFS/Dijkstra over small maps.
public class MovementService : IMovementService
{
    private readonly IGridManager _gridManager;

    public MovementService(IGridManager gridManager)
    {
        _gridManager = gridManager;
    }

    public IEnumerable<Vector2I> GetReachableTiles(Vector2I start, int movementBudget)
    {
        var results = new List<Vector2I>();
        var visited = new Dictionary<Vector2I, int>(new Vector2IComparer());
        var queue = new Queue<Vector2I>();

        visited[start] = 0;
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            int currentCost = visited[current];

            if (current != start)
                results.Add(current);

            foreach (var dir in GetOrthogonalDirections())
            {
                var next = current + dir;
                var tile = _gridManager.GetTileStateAt(next);
                if (tile == null) continue;
                if (tile.MovementCost >= 999) continue; // impassable
                if (tile.IsOccupied) continue;

                int nextCost = currentCost + tile.MovementCost;
                if (nextCost > movementBudget) continue;

                if (!visited.TryGetValue(next, out var knownCost) || nextCost < knownCost)
                {
                    visited[next] = nextCost;
                    queue.Enqueue(next);
                }
            }
        }

        return results.Distinct(new Vector2IComparer());
    }

    public bool CanReach(Vector2I start, Vector2I target, int movementBudget)
    {
        if (start == target) return true;
        var reachable = GetReachableTiles(start, movementBudget);
        return reachable.Contains(target, new Vector2IComparer());
    }

    private IEnumerable<Vector2I> GetOrthogonalDirections()
    {
        yield return new Vector2I(1, 0);
        yield return new Vector2I(-1, 0);
        yield return new Vector2I(0, 1);
        yield return new Vector2I(0, -1);
    }

    // Simple comparer for Vector2I to be used in dictionaries and LINQ.
    private class Vector2IComparer : IEqualityComparer<Vector2I>
    {
        public bool Equals(Vector2I a, Vector2I b) => a.X == b.X && a.Y == b.Y;
        public int GetHashCode(Vector2I v) => (v.X * 397) ^ v.Y;
    }
}
