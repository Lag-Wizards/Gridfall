using Godot;
using System.Collections.Generic;
using Gridfall.Domain;

namespace Gridfall.Contracts;

public interface IMovementService
{
	// Returns the set of reachable tile coordinates (Godot map coords) from start given a movement budget.
	IEnumerable<Vector2I> GetReachableTiles(Vector2I start, int movementBudget);

	// Validates if a target can be reached from start within the given budget.
	bool CanReach(Vector2I start, Vector2I target, int movementBudget);
}
