using Godot;
using System;
using Gridfall.Contracts;
using Gridfall.Services;

namespace Gridfall.Controllers;

// Simple tile-based player controller. Use arrow keys to step around the grid.
public partial class PlayerController : Node2D
{
    [Export] public NodePath GridNodePath { get; set; }
    [Export] public int MovementRange { get; set; } = 5;

    private GridNode _gridNode;
    private IGridManager _gridManager;
    private TileMapLayer _gridMap;
    private IMovementService _movementService;

    public Vector2I CurrentTile { get; private set; }

    public override void _Ready()
    {
        _gridNode = ResolveGridNode();
        if (_gridNode == null)
        {
            GD.PrintErr("PlayerController failed to resolve GridNode. Assign GridNodePath to the node with the GridNode script.");
            return;
        }

        _gridManager = _gridNode.GridManager;
        _gridMap = _gridNode.GridMap;
        _movementService = new MovementService(_gridManager);

        UpdateCurrentTileFromPosition();
        RegisterOccupantAt(CurrentTile);
    }

    private GridNode ResolveGridNode()
    {
        if (GridNodePath != null)
        {
            var assigned = GetNodeOrNull<GridNode>(GridNodePath);
            if (assigned != null)
                return assigned;
        }

        var sceneRoot = GetTree().CurrentScene;
        if (sceneRoot != null)
        {
            var found = FindGridNodeRecursive(sceneRoot);
            if (found != null)
            {
                GD.Print("PlayerController found GridNode by scanning the scene.");
                return found;
            }
        }

        return null;
    }

    private GridNode FindGridNodeRecursive(Node node)
    {
        if (node is GridNode gridNode)
            return gridNode;

        foreach (var child in node.GetChildren())
        {
            if (child is Node childNode)
            {
                var found = FindGridNodeRecursive(childNode);
                if (found != null)
                    return found;
            }
        }

        return null;
    }

    private void UpdateCurrentTileFromPosition()
    {
        CurrentTile = _gridManager.WorldToMap(GlobalPosition);
    }

    private void RegisterOccupantAt(Vector2I tile)
    {
        var tileState = _gridManager.GetTileStateAt(tile);
        if (tileState != null)
            tileState.CurrentOccupant = this;
    }

    private void UnregisterOccupantAt(Vector2I tile)
    {
        var tileState = _gridManager.GetTileStateAt(tile);
        if (tileState != null && tileState.CurrentOccupant == this)
            tileState.CurrentOccupant = null;
    }

    public override void _Input(InputEvent @event)
    {
        if (!(@event is InputEventKey keyEvent) || !keyEvent.IsPressed())
            return;

        Vector2I dir = keyEvent.Keycode switch
        {
            Key.Up => new Vector2I(0, -1),
            Key.Down => new Vector2I(0, 1),
            Key.Left => new Vector2I(-1, 0),
            Key.Right => new Vector2I(1, 0),
            _ => default
        };

        if (dir == default) return;

        TryMoveBy(dir);
    }

    private void TryMoveBy(Vector2I delta)
    {
        var target = CurrentTile + delta;
        GD.Print($"PlayerController: attempting move from {CurrentTile} to {target}");

        if (!_movementService.CanReach(CurrentTile, target, MovementRange))
        {
            GD.Print($"PlayerController: target {target} not reachable from {CurrentTile}");
            return;
        }

        var targetState = _gridManager.GetTileStateAt(target);
        if (targetState == null)
        {
            GD.Print($"PlayerController: no tile at {target}");
            return;
        }
        if (targetState.MovementCost >= 999)
        {
            GD.Print($"PlayerController: tile {target} impassable");
            return;
        }
        if (targetState.IsOccupied)
        {
            GD.Print($"PlayerController: tile {target} occupied");
            return;
        }

        UnregisterOccupantAt(CurrentTile);
        RegisterOccupantAt(target);
        CurrentTile = target;

        Vector2 worldPos = _gridManager.MapToWorld(target);
        GlobalPosition = worldPos;
        GD.Print($"PlayerController: moved to {target} at world {worldPos}");
    }
}
