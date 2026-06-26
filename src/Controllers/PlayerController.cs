using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Services;
using Gridfall.Domain.Enums;

namespace Gridfall.Controllers;

public partial class PlayerController : Node2D
{
	[Export] public int MovementRange { get; set; } = 5;

	private CharacterNode _parentUnit;
	private bool _movementActive;
	private int _remainingMovement;

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

	private void TryMove(Vector2I direction)
	{
		if (_parentUnit == null)
			return;

		if (_parentUnit.TryMove(direction, _remainingMovement, out int cost))
		{
			_remainingMovement -= cost;
			GameManager.Instance?.UpdateHud();

			if (_remainingMovement <= 0)
			{
				EndMovementPhase();
				GameManager.Instance?.EndPlayerMovementPhase();
			}
		}
	}
}
