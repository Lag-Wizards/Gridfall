using Godot;
using System;
using Gridfall.contracts;

namespace Gridfall.service;

// Class to manage grid state, storing tile states in a 2d array
public class GridManager : IGridManager
{
	public TileMapLayer GridMap { get; }

	private TileState[,] _gridMatrix;
	
	private Vector2I _mapSize;
	private Vector2I _mapOffset;

	public GridManager(TileMapLayer gridMap)
	{
		GridMap = gridMap ?? throw new ArgumentNullException(nameof(gridMap), "GridMap cannot be null");
		InitializeGrid();
	}

	private void InitializeGrid()
	{
		Rect2I mapBounds = GridMap.GetUsedRect();
		_mapSize = mapBounds.Size;
		_mapOffset = mapBounds.Position;

		_gridMatrix = new TileState[_mapSize.X, _mapSize.Y];

		for (int x = 0; x < _mapSize.X; x++)
		{
			for (int y = 0; y < _mapSize.Y; y++)
			{
				Vector2I godotTileCoords = new Vector2I(x + _mapOffset.X, y + _mapOffset.Y);
				TileData cellData = GridMap.GetCellTileData(godotTileCoords);

				_gridMatrix[x, y] = new TileState();

				if (cellData != null)
				{
					int rawInt = (int)cellData.GetCustomData("terrain_type");

					_gridMatrix[x, y].Terrain = (TerrainType)rawInt;

					_gridMatrix[x, y].MovementCost = _gridMatrix[x, y].Terrain switch
					{
						TerrainType.Grass => 1,
						_ => 999
					};
				}
				else
				{
					_gridMatrix[x, y].Terrain = TerrainType.Void;
					_gridMatrix[x, y].MovementCost = 999; 
				}
			}
		}
	}

	public TileState GetTileStateAt(Vector2I godotCoords)
	{
		int arrayX = godotCoords.X - _mapOffset.X;
		int arrayY = godotCoords.Y - _mapOffset.Y;

		if (arrayX >= 0 && arrayX < _mapSize.X && arrayY >= 0 && arrayY < _mapSize.Y)
		{
			return _gridMatrix[arrayX, arrayY];
		}

		return null;
	}
}
