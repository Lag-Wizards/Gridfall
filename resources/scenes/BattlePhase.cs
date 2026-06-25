using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Services;

public partial class BattlePhase : Control
{
	private Button fightButton;
	private Button cancelButton;
	private ExpUI expUi;
	private LevelUpUi levelupUi;
	private CharacterBase attacker;
	private CharacterBase defender;
	private CombatReportUi combatReport;
	BattleSystem battleSystem = new BattleSystem();
	public override void _Ready()
	{
		expUi = GetNode<ExpUI>("ExpUI");
		levelupUi = GetNode<LevelUpUi>("LevelUpUi");
		combatReport = GetNode<CombatReportUi>("CombatReport");
		
		expUi.Visible = false;
		levelupUi.Visible = false;
		fightButton = GetNode<Button>("VBoxContainerFightOptions/FightButton");
		cancelButton = GetNode<Button>("VBoxContainerFightOptions/CancelButton");

		fightButton.Pressed += OnFightPressed;
		cancelButton.Pressed += OnCancelPressed;
	}

	public void ShowBattle(
		CharacterBase attacker,
		CharacterBase defender)
	{
		// Populate CombatReport
		this.attacker = attacker;
		this.defender = defender;
		
		combatReport.SetSelectedCharacter(attacker, defender);
		expUi.SetSelectedCharacter(attacker);
		levelupUi.SetSelectedCharacter(attacker);
	}

	private void OnFightPressed()
	{
		// Execute battle
		battleSystem.InitiateCombat(attacker, defender);
		// Show EXP
		expUi.Visible = true;
		// Show level up
		levelupUi.Visible = true;
	}

	private void OnCancelPressed()
	{
		Hide();
	}
}
