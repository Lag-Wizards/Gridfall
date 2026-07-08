using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Services;
using Gridfall.Domain.Enums;
using Gridfall.Domain;
namespace Gridfall.Controllers;

public partial class PlayerController : Node2D
{
	[Export] public int MovementRange { get; set; } = 5;

	private CharacterNode _parentUnit;
	private bool _movementActive;
	private int _remainingMovement;

	private PackedScene _inventoryScene = GD.Load<PackedScene>("res://resources/scenes/inventory_ui.tscn");
	private CanvasLayer _inventoryCanvasLayer;

	private PackedScene _shopScene = GD.Load<PackedScene>("res://resources/scenes/shop_ui.tscn");
	private CanvasLayer _shopCanvasLayer;

	public int RemainingMovement => _remainingMovement;

	public override void _Ready()
	{
		_parentUnit = GetParent<CharacterNode>();
		if (_parentUnit == null)
		{
			GD.PrintErr("PlayerController must be a child of CharacterNode.");
			return;
		}

		GameManager.Instance?.RegisterPlayerController(this);
	}

	public void StartMovementPhase()
	{
		_movementActive = true;
		_remainingMovement = _parentUnit?.Stats?.MovementRange ?? MovementRange;
		GameManager.Instance?.UpdateHud();
	}

	public void EndMovementPhase()
	{
		_movementActive = false;
	}

	public override void _Input(InputEvent @event)
	{
		if (!(@event is InputEventKey keyEvent) || !keyEvent.IsPressed())
			return;

		if (keyEvent.Keycode == Key.I)
		{
			ToggleInventory();
			return;
		}

		if (keyEvent.Keycode == Key.S)
		{
			ToggleShop();
			return;
		}

		if (!_movementActive || GameManager.Instance?.CurrentPhase != GamePhase.PlayerMovement)
			return;

		if (keyEvent.Keycode == Key.M)
		{
			EndMovementPhase();
			GameManager.Instance?.EndPlayerMovementPhase();
			return;
		}

		Vector2I dir = keyEvent.Keycode switch
		{
			Key.Up => new Vector2I(0, -1),
			Key.Down => new Vector2I(0, 1),
			Key.Left => new Vector2I(-1, 0),
			Key.Right => new Vector2I(1, 0),
			_ => default
		};

		if (dir == default)
			return;

		TryMove(dir);
	}

	private void ToggleInventory()
	{
		if (_inventoryCanvasLayer != null && IsInstanceValid(_inventoryCanvasLayer))
		{
			_inventoryCanvasLayer.QueueFree();
			_inventoryCanvasLayer = null;
			return;
		}

		OpenInventory();
	}

	private void OpenInventory()
	{
		if (_inventoryScene == null || _parentUnit == null)
			return;

		_inventoryCanvasLayer = new CanvasLayer();
		InventoryUi inventoryUi = _inventoryScene.Instantiate<InventoryUi>();

		_inventoryCanvasLayer.AddChild(inventoryUi);
		GetTree().CurrentScene.AddChild(_inventoryCanvasLayer);

		inventoryUi.SetSelectedCharacter(_parentUnit.Stats);
	}

	private void ToggleShop()
	{
		if (_shopCanvasLayer != null && IsInstanceValid(_shopCanvasLayer))
		{
			_shopCanvasLayer.QueueFree();
			_shopCanvasLayer = null;
			return;
		}

		OpenShop();
	}

	private void OpenShop()
	{
		if (_shopScene == null || _parentUnit == null)
			return;

		_shopCanvasLayer = new CanvasLayer();
		ShopUi shopUi = _shopScene.Instantiate<ShopUi>();

		_shopCanvasLayer.AddChild(shopUi);
		GetTree().CurrentScene.AddChild(_shopCanvasLayer);

		shopUi.SetSelectedCharacter(_parentUnit.Stats);
	}

	private void TryMove(Vector2I direction)
	{
		if (_parentUnit == null)
			return;

		if (_parentUnit.TryMove(direction, _remainingMovement, out int cost))
		{
			_remainingMovement -= cost;
			GameManager.Instance?.UpdateHud();

			TileState currentTile = GameManager.Instance?.GridManager?.GetTileStateAt(_parentUnit.CurrentTile);

			if (currentTile != null && currentTile.Terrain == TerrainType.Shop)
			{
				OpenShop();
			}

			if (_remainingMovement <= 0)
			{
				EndMovementPhase();
				GameManager.Instance?.EndPlayerMovementPhase();
			}
		}
	}
}