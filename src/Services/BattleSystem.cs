using Godot;
using System;
using Gridfall.Characters.Domain;

namespace Gridfall.Services;
public class BattleSystem
{
	private BattleCalculation battleCalculation = new BattleCalculation();

	public void InitiateCombat(CharacterBase attacker, CharacterBase defender)
	{
		battleCalculation.ExecuteBattle(attacker, defender);
	}
}
