
using Godot;
using Gridfall.Domain;

namespace Gridfall.Contracts;

public interface ITileFactory
{
	// Returns tile based on cell data
	public TileState CreateTile(TileData cellData);
}
