using Godot;
using System;
using Gridfall.Contracts;
using Gridfall.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Services;

// Class to create tile states based on tile data
public class TileFactory : ITileFactory
{
	
	public TileState CreateTile(TileData cellData)
	{
		if (cellData != null)
			{
				TileState tileState = new TileState();

				int rawInt = cellData.GetCustomData("terrain_type").AsInt32();

				tileState.Terrain = (TerrainType)rawInt;

				tileState.MovementCost = tileState.Terrain switch
				{
					TerrainType.Grass => 1,
					_ => 999
				};

				return tileState;
			}
			else
			{
				return new TileState
				{
					Terrain = TerrainType.Void,
					MovementCost = 999
				};
			}
	}
}
