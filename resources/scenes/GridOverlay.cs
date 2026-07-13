using Godot;
using System;
using System.Collections.Generic;

namespace Gridfall.resources.scenes;

public partial class GridOverlay : TileMapLayer
{
	[Export]
	public int BlueSourceId = 0;
	[Export]
	public Vector2I BlueAtlasCoords = Vector2I.Zero;
	[Export]
	public int RedSourceId = 1;
	[Export]
	public Vector2I RedAtlasCoords = Vector2I.Right;

	// Drawing movement tiles
	public void DrawMovement(HashSet<Vector2I> tiles)
	{
		foreach (Vector2I tile in tiles)
		{
			SetCell(tile, BlueSourceId, BlueAtlasCoords);
		}
	}
	// Drawing attack tiles
	public void DrawAttack(HashSet<Vector2I> tiles)
	{
		foreach (Vector2I tile in tiles)
		{
			SetCell(tile, RedSourceId, RedAtlasCoords);
		}
	}
	// Clear the overlay
	public void ClearRanges()
	{
		Clear();
	}
}
