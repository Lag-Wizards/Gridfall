using Godot;
using System;
using Gridfall.Controllers;
using Gridfall.Contracts;
using Gridfall.Domain;
using Gridfall.Services;

namespace Gridfall.Services;
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

		GridManager = new GridManager(GridMap, new TileFactory());
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

		if (FindPlayerControllerRecursive(sceneRoot) != null)
			return;

		var controller = new PlayerController
		{
			Name = "PlayerController",
			GridNodePath = GetPath(),
			MovementRange = 5
		};

		sceneRoot.AddChild(controller);
		GD.Print("GridNode: auto-created PlayerController node.");
	}

	private PlayerController FindPlayerControllerRecursive(Node node)
	{
		if (node is PlayerController controller)
			return controller;

		foreach (var child in node.GetChildren())
		{
			if (child is Node childNode)
			{
				var found = FindPlayerControllerRecursive(childNode);
				if (found != null)
					return found;
			}
		}

		return null;
	}
}
