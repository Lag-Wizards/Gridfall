using Godot;
using System.Collections.Generic;
using System.Linq;
using Gridfall.Contracts;
using Gridfall.Services;

namespace Gridfall.Controllers;

// Simple tile-based player controller. Use arrow keys to step around the grid.
public partial class PlayerController : Node2D
{
	[Export] public NodePath GridNodePath { get; set; }
	[Export] public NodePath MovementOverlayPath { get; set; }
	[Export] public NodePath MovementPhaseLabelPath { get; set; }
	[Export] public NodePath PlayerHealthLabelPath { get; set; }
	[Export] public NodePath MovementRemainingLabelPath { get; set; }
	[Export] public NodePath NextPhaseButtonPath { get; set; }
	[Export] public int MovementRange { get; set; } = 5;

	private GridNode _gridNode;
	private IGridManager _gridManager;
	private TileMapLayer _gridMap;
	private TileMapLayer _movementOverlay;
	private IMovementService _movementService;
	private Label _movementPhaseLabel;
	private Label _playerHealthLabel;
	private Label _movementRemainingLabel;
	private Button _nextPhaseButton;
	private List<Vector2I> _reachableTiles = new();
	private bool _movementPhaseActive;
	private int _remainingMovement;
	private int _currentHealth = 20;
	private int _maxHealth = 20;

	public Vector2I CurrentTile { get; private set; }

	private Sprite2D _debugMarker;

	public override void _Ready()
	{
		_gridNode = ResolveGridNode();
		if (_gridNode == null)
		{
			GD.PrintErr("PlayerController failed to resolve GridNode. Assign GridNodePath to the node with the GridNode script.");
			return;
		}

		_gridManager = _gridNode.GridManager;
		_gridMap = _gridNode.GridMap;
		_movementService = new MovementService(_gridManager);
		_movementOverlay = ResolveMovementOverlay();
		_movementPhaseLabel = ResolveMovementPhaseLabel();
		_playerHealthLabel = ResolveLabel(PlayerHealthLabelPath, "PlayerHealthLabel");
		_movementRemainingLabel = ResolveLabel(MovementRemainingLabelPath, "MovementRemainingLabel");
		_nextPhaseButton = ResolveButton(NextPhaseButtonPath, "NextPhaseButton");

		EnsureDebugMarker();
		UpdateCurrentTileFromPosition();
		SnapToCurrentTile();
		RegisterOccupantAt(CurrentTile);
		ResetCharacterStatus();
		ActivateMovementPhase();

		if (_nextPhaseButton != null)
		{
			_nextPhaseButton.Pressed += OnNextPhasePressed;
		}
	}

	private GridNode ResolveGridNode()
	{
		if (GridNodePath != null && !string.IsNullOrEmpty(GridNodePath.ToString()))
		{
			var assigned = GetNodeOrNull<GridNode>(GridNodePath);
			if (assigned != null)
				return assigned;
		}

		var sceneRoot = GetTree().CurrentScene;
		if (sceneRoot != null)
		{
			var found = FindGridNodeRecursive(sceneRoot);
			if (found != null)
			{
				GD.Print("PlayerController found GridNode by scanning the scene.");
				return found;
			}
		}

		return null;
	}

	private GridNode FindGridNodeRecursive(Node node)
	{
		if (node is GridNode gridNode)
			return gridNode;

		foreach (var child in node.GetChildren())
		{
			if (child is Node childNode)
			{
				var found = FindGridNodeRecursive(childNode);
				if (found != null)
					return found;
			}
		}

		return null;
	}

	private void UpdateCurrentTileFromPosition()
	{
		CurrentTile = _gridManager.WorldToMap(GlobalPosition);
	}

	private void SnapToCurrentTile()
	{
		if (_gridManager == null)
			return;

		var worldPosition = _gridManager.MapToWorld(CurrentTile);
		GlobalPosition = worldPosition;
		GD.Print($"PlayerController: snapped to current tile {CurrentTile} at {worldPosition}");
	}

	private void ActivateMovementPhase()
	{
		_movementPhaseActive = true;
		_remainingMovement = MovementRange;
		_updateReachableTiles();
		UpdateMovementOverlay();
		UpdateMovementPhaseLabel();
		UpdatePlayerHealthLabel();
		UpdateMovementRemainingLabel();
	}

	private void DeactivateMovementPhase()
	{
		_movementPhaseActive = false;
		_reachableTiles.Clear();
		UpdateMovementOverlay();
		UpdateMovementPhaseLabel();
	}

	private void ResetCharacterStatus()
	{
		_maxHealth = 20;
		_currentHealth = _maxHealth;
		_remainingMovement = MovementRange;
		UpdatePlayerHealthLabel();
		UpdateMovementRemainingLabel();
	}

	private void OnNextPhasePressed()
	{
		DeactivateMovementPhase();
	}

	private void ToggleMovementPhase()
	{
		if (_movementPhaseActive)
			DeactivateMovementPhase();
		else
			ActivateMovementPhase();
	}

	private void _updateReachableTiles()
	{
		_reachableTiles = _movementService.GetReachableTiles(CurrentTile, MovementRange).ToList();
	}

	private void UpdateMovementOverlay()
	{
		if (_movementOverlay == null)
			return;

		_movementOverlay.Clear();

		if (!_movementPhaseActive)
			return;

		foreach (var tile in _reachableTiles)
		{
			_movementOverlay.SetCell(new Vector2I(tile.X, tile.Y), 0);
		}
	}

	private void UpdateMovementPhaseLabel()
	{
		if (_movementPhaseLabel == null)
			return;

		_movementPhaseLabel.Text = _movementPhaseActive ? "Movement Phase: ACTIVE" : "Movement Phase: INACTIVE";
		_movementPhaseLabel.Modulate = _movementPhaseActive ? Colors.LimeGreen : Colors.LightGray;
	}

	private void UpdatePlayerHealthLabel()
	{
		if (_playerHealthLabel == null)
			return;

		_playerHealthLabel.Text = $"HP: {_currentHealth}/{_maxHealth}";
	}

	private void UpdateMovementRemainingLabel()
	{
		if (_movementRemainingLabel == null)
			return;

		_movementRemainingLabel.Text = $"Move: {_remainingMovement}/{MovementRange}";
	}

	private TileMapLayer ResolveMovementOverlay()
	{
		if (MovementOverlayPath != null && !string.IsNullOrEmpty(MovementOverlayPath.ToString()))
		{
			var node = GetNodeOrNull<TileMapLayer>(MovementOverlayPath);
			if (node != null)
				return node;

			var root = GetTree().CurrentScene;
			if (root != null)
			{
				node = root.GetNodeOrNull<TileMapLayer>(MovementOverlayPath);
				if (node != null)
					return node;
			}
		}

		var sceneRoot = GetTree().CurrentScene;
		return sceneRoot?.GetNodeOrNull<TileMapLayer>("GridOverlay");
	}

	private Label ResolveMovementPhaseLabel()
	{
		return ResolveLabel(MovementPhaseLabelPath, "MovementPhaseLabel");
	}

	private Label ResolveLabel(NodePath path, string fallbackName)
	{
		if (path != null && !string.IsNullOrEmpty(path.ToString()))
		{
			var node = GetNodeOrNull<Label>(path);
			if (node != null)
				return node;

			var root = GetTree().CurrentScene;
			if (root != null)
			{
				node = root.GetNodeOrNull<Label>(path);
				if (node != null)
					return node;
			}
		}

		var sceneRoot = GetTree().CurrentScene;
		return sceneRoot != null ? FindLabelRecursive(sceneRoot, fallbackName) : null;
	}

	private Button ResolveButton(NodePath path, string fallbackName)
	{
		if (path != null && !string.IsNullOrEmpty(path.ToString()))
		{
			var node = GetNodeOrNull<Button>(path);
			if (node != null)
				return node;

			var root = GetTree().CurrentScene;
			if (root != null)
			{
				node = root.GetNodeOrNull<Button>(path);
				if (node != null)
					return node;
			}
		}

		var sceneRoot = GetTree().CurrentScene;
		return sceneRoot != null ? FindButtonRecursive(sceneRoot, fallbackName) : null;
	}

	private Label FindLabelRecursive(Node node, string name)
	{
		if (node is Label label && node.Name == name)
			return label;

		foreach (var child in node.GetChildren())
		{
			if (child is Node childNode)
			{
				var found = FindLabelRecursive(childNode, name);
				if (found != null)
					return found;
			}
		}

		return null;
	}

	private Button FindButtonRecursive(Node node, string name)
	{
		if (node is Button button && node.Name == name)
			return button;

		foreach (var child in node.GetChildren())
		{
			if (child is Node childNode)
			{
				var found = FindButtonRecursive(childNode, name);
				if (found != null)
					return found;
			}
		}

		return null;
	}

	private ImageTexture CreateDebugTexture(int size, Color color)
	{
		var image = Image.CreateEmpty(size, size, false, Image.Format.Rgba8);
		image.Fill(color);
		return ImageTexture.CreateFromImage(image);
	}

	private void EnsureDebugMarker()
	{
		foreach (var child in GetChildren())
		{
			if (child is Sprite2D sprite && sprite.Name == "DebugMarker")
			{
				_debugMarker = sprite;
				return;
			}
		}

		_debugMarker = new Sprite2D
		{
			Name = "DebugMarker",
			Texture = CreateDebugTexture(24, Colors.Red),
			Centered = true,
			Position = Vector2.Zero
		};
		AddChild(_debugMarker);
	}

	private void RegisterOccupantAt(Vector2I tile)
	{
		var tileState = _gridManager.GetTileStateAt(tile);
		if (tileState != null)
			tileState.CurrentOccupant = this;
	}

	private void UnregisterOccupantAt(Vector2I tile)
	{
		var tileState = _gridManager.GetTileStateAt(tile);
		if (tileState != null && tileState.CurrentOccupant == this)
			tileState.CurrentOccupant = null;
	}

	public override void _Input(InputEvent @event)
	{
		if (!(@event is InputEventKey keyEvent) || !keyEvent.IsPressed())
			return;

		if (keyEvent.Keycode == Key.M)
		{
			ToggleMovementPhase();
			return;
		}

		if (!_movementPhaseActive)
		{
			GD.Print("PlayerController: movement blocked because phase is inactive.");
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

		TryMoveBy(dir);
	}

	private void TryMoveBy(Vector2I delta)
	{
		var target = CurrentTile + delta;
		GD.Print($"PlayerController: attempting move from {CurrentTile} to {target}");

		if (!_movementPhaseActive)
		{
			GD.Print("PlayerController: movement blocked because phase is inactive.");
			return;
		}

		if (_remainingMovement <= 0)
		{
			GD.Print("PlayerController: no movement remaining this phase.");
			return;
		}

		if (!_movementService.CanReach(CurrentTile, target, _remainingMovement))
		{
			GD.Print($"PlayerController: target {target} not reachable from {CurrentTile}");
			return;
		}

		var targetState = _gridManager.GetTileStateAt(target);
		if (targetState == null)
		{
			GD.Print($"PlayerController: no tile at {target}");
			return;
		}
		if (targetState.MovementCost >= 999)
		{
			GD.Print($"PlayerController: tile {target} impassable");
			return;
		}
		if (targetState.IsOccupied)
		{
			GD.Print($"PlayerController: tile {target} occupied");
			return;
		}

		UnregisterOccupantAt(CurrentTile);
		RegisterOccupantAt(target);
		CurrentTile = target;

		var worldPos = _gridManager.MapToWorld(target);
		GlobalPosition = worldPos;
		GD.Print($"PlayerController: moved to {target} at world {worldPos}");

		_remainingMovement -= targetState.MovementCost;
		if (_remainingMovement < 0)
			_remainingMovement = 0;

		_updateReachableTiles();
		UpdateMovementOverlay();
		UpdateMovementRemainingLabel();
	}
}
