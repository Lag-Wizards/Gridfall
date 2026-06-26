using Godot;
using System;
using Gridfall.Controllers;
using Gridfall.Contracts;
using Gridfall.Domain;
using Gridfall.Services;

namespace Gridfall.Services;

public partial class GridNode : Node
{
	[Export] public TileMapLayer GridMap { get; set; }

	public IGridManager GridManager { get; private set; }

	public override void _Ready()
	{
		EnsureGridMap();
		if (GridMap == null)
		{
			GD.PrintErr("GridMap is null within GridNode");
			return;
		}

		EnsurePlayerController();

		GridManager = new GridManager(GridMap, new TileFactory());
		GameManager.Instance?.RegisterGridManager(GridManager);

		var tileState = GridManager.GetTileStateAt(new Vector2I(0, 0));
		if (tileState != null)
		{
			GD.Print("Tile move cost at (0, 0): " + tileState.MovementCost);
		}

		EnsurePlayerController();
	}

	private void EnsurePlayerController()
	{
		var sceneRoot = GetTree().CurrentScene;
		if (sceneRoot == null)
			return;
		var characterNode = sceneRoot.GetNodeOrNull<Node2D>("Character");
		if (characterNode == null)
		{
			GD.PrintErr("Character node not found in scene. Cannot attach PlayerController.");
			return;
		}

		var controller = new PlayerController
		{
			Name = "PlayerController",
			MovementRange = 5
		};

		characterNode.AddChild(controller);
	}

	private void EnsureGridMap()
	{
		if (GridMap != null)
			return;

		var sceneRoot = GetTree().CurrentScene;
		if (sceneRoot == null)
			return;

		var gridMap = sceneRoot.GetNodeOrNull<TileMapLayer>("GridMap");
		if (gridMap != null)
			GridMap = gridMap;
	}
}
