using Godot;
using System;
using Gridfall.Contracts;
using Gridfall.Domain;
using Gridfall.Domain.Enums;

namespace Gridfall.Services;

// Class to manage grid state, storing tile states in a 2d array
public class GridManager : IGridManager
{
	public TileMapLayer GridMap { get; }

	private readonly ITileFactory _tileFactory;
	private TileState[,] _gridMatrix;
	
	private Vector2I _mapSize;
	private Vector2I _mapOffset;

	public GridManager(TileMapLayer gridMap, ITileFactory tileFactory)
	{
		GridMap = gridMap ?? throw new ArgumentNullException(nameof(gridMap), "GridMap cannot be null");
		_tileFactory = tileFactory ?? throw new ArgumentNullException(nameof(tileFactory), "TileFactory cannot be null");
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

				_gridMatrix[x, y] = _tileFactory.CreateTile(cellData);
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

	public Vector2I WorldToMap(Vector2 worldPosition)
	{
		Vector2 local = worldPosition - GridMap.GlobalPosition;
		Vector2 cellSize = GetCellSize();

		int x = Mathf.FloorToInt(local.X / cellSize.X);
		int y = Mathf.FloorToInt(local.Y / cellSize.Y);
		return new Vector2I(x, y);
	}

	public Vector2 MapToWorld(Vector2I godotCoords)
	{
		Vector2 relative = new Vector2(godotCoords.X, godotCoords.Y);
		Vector2 cellSize = GetCellSize();

		Vector2 world = GridMap.GlobalPosition + new Vector2(relative.X * cellSize.X, relative.Y * cellSize.Y) + (cellSize / 2f);
		return world;
	}

	private Vector2 GetCellSize()
	{
		if (GridMap.TileSet != null)
		{
			return (Vector2)GridMap.TileSet.TileSize;
		}
		GD.PushError("GridManager: TileSet is null. Falling back to default 16x16 tile size.");
		return new Vector2(16, 16);
	}
}
