using Godot;

namespace Gridfall.domain;

public class TileState
{
	public TerrainType Terrain { get; set; } = TerrainType.Grass;
	public int MovementCost { get; set; } = 1;
	public Node2D CurrentOccupant { get; set; } = null;
	public bool IsOccupied => CurrentOccupant != null;
}
