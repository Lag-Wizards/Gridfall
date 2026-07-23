using Godot;
using System;
using Gridfall.Characters.Domain;

public partial class Camera2d : Camera2D
{
	[Export]
	public float MoveSpeed = 400f;

	[Export]
	public TileMapLayer TilemapLayer { get; set; }

	public override void _Ready()
	{
		if (TilemapLayer != null)
		{
			SetCameraLimitsToTilemap();
		}
	}
	public override void _Process(double delta)
	{
		Vector2 direction = Vector2.Zero;
		if (Input.IsActionPressed("left"))
			direction.X--;

		if (Input.IsActionPressed("right"))
			direction.X++;

		if (Input.IsActionPressed("up"))
			direction.Y--;

		if (Input.IsActionPressed("down"))
			direction.Y++;

		if (direction != Vector2.Zero)
		{
			Position += direction.Normalized() * MoveSpeed * (float)delta;
		}
	}
	
	private void SetCameraLimitsToTilemap()
	{
		Rect2I mapRect = TilemapLayer.GetUsedRect();

		Vector2I cellSize = TilemapLayer.TileSet.TileSize;

		LimitLeft = mapRect.Position.X * cellSize.X;
		LimitTop = mapRect.Position.Y * cellSize.Y;
		LimitRight = mapRect.End.X * cellSize.X;
		LimitBottom = mapRect.End.Y * cellSize.Y;
	}
}
