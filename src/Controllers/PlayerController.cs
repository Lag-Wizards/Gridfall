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
        if (GridNodePath == null)
        {
            GD.PrintErr("GridNodePath not set on PlayerController");
            return;
        }

        _gridNode = GetNodeOrNull<GridNode>(GridNodePath);
        if (_gridNode == null)
        {
            GD.PrintErr("GridNode not found from PlayerController");
            return;
        }

        _gridManager = _gridNode.GridManager;
        _gridMap = _gridNode.GridMap;
        _movementService = new MovementService(_gridManager);

        UpdateCurrentTileFromPosition();
        RegisterOccupantAt(CurrentTile);
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
        if (!_movementService.CanReach(CurrentTile, target, MovementRange))
            return;

        var targetState = _gridManager.GetTileStateAt(target);
        if (targetState == null) return;
        if (targetState.MovementCost >= 999) return;
        if (targetState.IsOccupied) return;

        UnregisterOccupantAt(CurrentTile);
        RegisterOccupantAt(target);
        CurrentTile = target;

        // Move the node to the tile world position
        Vector2 worldPos = _gridManager.MapToWorld(target);
        GlobalPosition = worldPos;
    }
}
