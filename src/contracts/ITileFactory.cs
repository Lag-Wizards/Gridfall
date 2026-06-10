
using Godot;
using Gridfall.domain;

namespace Gridfall.contracts;

public interface ITileFactory
{
    // Returns tile based on cell data
    public TileState CreateTile(TileData cellData);
}