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
		// Convert world position to local position relative to the TileMap
		Vector2 local = worldPosition - GridMap.GlobalPosition;
		Vector2 cellSize = GetCellSize();

		int x = Mathf.FloorToInt(local.X / cellSize.X) + _mapOffset.X;
		int y = Mathf.FloorToInt(local.Y / cellSize.Y) + _mapOffset.Y;

		return new Vector2I(x, y);
	}

	public Vector2 MapToWorld(Vector2I godotCoords)
	{
		Vector2 cellSize = GetCellSize();
		Vector2 relative = new Vector2(godotCoords.X - _mapOffset.X, godotCoords.Y - _mapOffset.Y);

		// Position at cell top-left + half cell to center
		Vector2 world = GridMap.GlobalPosition + new Vector2(relative.X * cellSize.X, relative.Y * cellSize.Y) + (cellSize / 2f);

		return world;
	}

	// Helper to obtain tile cell size from the TileMap. Uses reflection to be resilient
	// to different TileMap-like node types. Falls back to 16x16 if not available.
	private Vector2 GetCellSize()
	{
		// Try property "CellSize"
		var type = GridMap.GetType();
		var prop = type.GetProperty("CellSize");
		if (prop != null)
		{
			object val = prop.GetValue(GridMap);
			if (val is Vector2 v) return v;
		}

		// Try method "GetCellSize()"
		var method = type.GetMethod("GetCellSize");
		if (method != null)
		{
			object val = method.Invoke(GridMap, null);
			if (val is Vector2 v2) return v2;
		}

		// Try accessing TileSet tile size via TileSet.TileSize or similar
		var tileSetProp = type.GetProperty("TileSet") ?? type.GetProperty("Tileset");
		if (tileSetProp != null)
		{
			var tileSet = tileSetProp.GetValue(GridMap);
			if (tileSet != null)
			{
				var tsType = tileSet.GetType();
				var tileSizeProp = tsType.GetProperty("TileSize") ?? tsType.GetProperty("Size");
				if (tileSizeProp != null)
				{
					object val = tileSizeProp.GetValue(tileSet);
					if (val is Vector2 v3) return v3;
				}
			}
		}

		// Final fallback
		return new Vector2(16, 16);
	}
}
