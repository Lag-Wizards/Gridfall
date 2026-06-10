using Godot;
using Gridfall.Domain.Enums;

namespace Gridfall.Domain;

public class TileState
{
	public TerrainType Terrain { get; set; } = TerrainType.Grass;
	public int MovementCost { get; set; } = 1;
	public Node2D CurrentOccupant { get; set; } = null;
	public bool IsOccupied => CurrentOccupant != null;
}
