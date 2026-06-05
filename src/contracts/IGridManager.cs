using Godot;

namespace Gridfall.contracts;
// Interface for class that stores state of current tiles in 2d array
public interface IGridManager
{
	// Retrieves the tile state at the given coordinates.
	TileState GetTileStateAt(Vector2I godotCoords);
}
