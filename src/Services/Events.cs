using Godot;
using System;

namespace Gridfall.Services;

public static class Events
{
	public static event Action OnPlayerDeath;
	public static event Action OnEnemyDeath;
	public static event Action OnEnemySpawn;
	public static void EmitEnemySpawned() => OnEnemySpawn?.Invoke();
	public static void EmitPlayerDied() => OnPlayerDeath?.Invoke();
	public static void EmitEnemyDied() => OnEnemyDeath?.Invoke();
}
