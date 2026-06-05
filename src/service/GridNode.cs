using Godot;
using System;
using Gridfall.contracts;

namespace Gridfall.service;
// Node within Godot to access tilemap for GridManager to use.
public partial class GridNode : Node
{
	[Export] public TileMapLayer GridMap { get; set; }

	public IGridManager GridManager { get; private set; }

	public override void _Ready()
	{
		if (GridMap == null)
		{
			GD.PrintErr("GridMap is null within GridNode");
			return;
		}

		GridManager = new GridManager(GridMap);
		var tileState = GridManager.GetTileStateAt(new Vector2I(0, 0));
		if (tileState != null)
		{
			GD.Print("Tile move cost at (0, 0): " + tileState.MovementCost);
		}
	}
}
