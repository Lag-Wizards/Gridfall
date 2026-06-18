using Godot;
using Gridfall.Domain;

namespace Gridfall.Contracts;
// Interface for class that stores state of current tiles in 2d array
public interface IGridManager
{
	// Retrieves the tile state at the given coordinates.
	TileState GetTileStateAt(Vector2I godotCoords);

	// Convert a world position to Godot tile coordinates (global map coords)
	Vector2I WorldToMap(Vector2 worldPosition);

	// Convert Godot tile coordinates to a world position (center of tile)
	Vector2 MapToWorld(Vector2I godotCoords);
}
