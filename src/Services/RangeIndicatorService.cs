using Godot;
using System;
using System.Collections.Generic;
using Gridfall.Contracts;
using Gridfall.Domain;

namespace Gridfall.Services;

public class RangeIndicatorService
{
    public HashSet<Vector2I> GetMovementRange(Vector2I start, int maxMovement, IGridManager grid)
    {
        Dictionary<Vector2I, int> visited = new();
        List<PathNode> openList = new();

        PathNode startNode = new(start) { GCost = 0 };
        openList.Add(startNode);
        visited[start] = 0;

        while (openList.Count > 0)
        {
            PathNode current = openList[0];
            foreach (var node in openList)
            {
                if (node.GCost < current.GCost)
                    current = node;
            }

            openList.Remove(current);

            foreach (Vector2I neighbor in GetNeighbors(current.Position))
            {
                TileState state = grid.GetTileStateAt(neighbor);
                
                if (state == null || !state.IsWalkable || state.MovementCost >= 999)
                    continue;
                
                if (state.CurrentOccupant != null)
                    continue;

                int costToNeighbor = current.GCost + state.MovementCost;
                
                if (costToNeighbor > maxMovement)
                    continue;

                if (!visited.ContainsKey(neighbor) || costToNeighbor < visited[neighbor])
                {
                    visited[neighbor] = costToNeighbor;

                    PathNode existing = openList.Find(n => n.Position == neighbor);
                    if (existing == null)
                    {
                        openList.Add(new PathNode(neighbor) { GCost = costToNeighbor });
                    }
                    else
                    {
                        existing.GCost = costToNeighbor;
                    }
                }
            }
        }

        return new HashSet<Vector2I>(visited.Keys);
    }

    public HashSet<Vector2I> GetAttackRange(HashSet<Vector2I> movementTiles, int minRange, int maxRange)
    {
        HashSet<Vector2I> attackTiles = new();
        foreach (Vector2I tile in movementTiles)
        {
            for (int dx = -maxRange; dx <= maxRange; dx++)
            {
                for (int dy = -maxRange; dy <= maxRange; dy++)
                {
                    int distance = Mathf.Abs(dx) + Mathf.Abs(dy);

                    if (distance >= minRange && distance <= maxRange)
                    {
                        attackTiles.Add(new Vector2I(tile.X + dx, tile.Y + dy));
                    }
                }
            }
        }
        return attackTiles;
    }

    private List<Vector2I> GetNeighbors(Vector2I tile)
    {
        return new()
        {
            tile + Vector2I.Up,
            tile + Vector2I.Down,
            tile + Vector2I.Left,
            tile + Vector2I.Right
        };
    }
}