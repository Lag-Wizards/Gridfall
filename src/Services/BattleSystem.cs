using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Domain;

namespace Gridfall.Services;
public class BattleSystem
{
	private BattleCalculation battleCalculation = new BattleCalculation();

	public void InitiateCombat(CombatUnit attacker, CombatUnit defender)
	{
		GD.Print("Executing battle");
		battleCalculation.ExecuteBattle(attacker, defender);
	}
}
