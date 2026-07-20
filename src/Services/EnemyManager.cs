using System;
using System.Collections.Generic;
using Gridfall.Domain.Enemies;

namespace Gridfall.Services;
	public static class EnemyManager
	{
		public static EnemyBase CreateEnemy(string type, int level = 1)
		{
			switch (type?.ToLower())
			{
				case "goblin":
					return new Goblin(level);
				case "slime":
					return new Slime(level);
				case "normalslime":
				case "normal slime":
					return new NormalSlime(level);
				default:
					throw new ArgumentException($"Unknown enemy type '{type}'");
			}
		}

		public static List<EnemyBase> SpawnWave(Dictionary<string, int> composition)
		{
			var list = new List<EnemyBase>();
			foreach (var kv in composition)
			{
				for (int i = 0; i < kv.Value; i++)
					list.Add(CreateEnemy(kv.Key));
			}
			return list;
		}
	}
