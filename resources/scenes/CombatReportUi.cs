using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Services;

public partial class CombatReportUi : Control
{
	private Label attackerhpLabel;
	private Label attackerdamageLabel;
	private Label attackerhitrateLabel;
	private Label attackerdoubleLabel;
	private Label defenderhpLabel;
	private Label defenderdamageLabel;
	private Label defenderhitrateLabel;
	private Label defenderdoubleLabel;
	CharacterBase selectedAttackerCharacter;
	CharacterBase selectedDefenderCharacter;
	BattleCalculation battleCalculation = new BattleCalculation();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		attackerhpLabel = GetNode<Label>("HBoxContainer/AttackerParameters/HP");
		attackerdamageLabel = GetNode<Label>("HBoxContainer/AttackerParameters/Attack");
		attackerhitrateLabel = GetNode<Label>("HBoxContainer/AttackerParameters/HitRate"); 
		attackerdoubleLabel = GetNode<Label>("HBoxContainer/AttackerParameters/Double");

		defenderhpLabel  = GetNode<Label>("HBoxContainer/DefenderParameters/HP");
		defenderdamageLabel  = GetNode<Label>("HBoxContainer/DefenderParameters/Attack");
		defenderhitrateLabel  = GetNode<Label>("HBoxContainer/DefenderParameters/HitRate");
		defenderdoubleLabel  = GetNode<Label>("HBoxContainer/DefenderParameters/Double");
	}

	public void SetSelectedCharacter(CharacterBase attacker, CharacterBase defender)
	{

		selectedAttackerCharacter = attacker;
		selectedDefenderCharacter = defender;

		if (selectedAttackerCharacter == null || selectedDefenderCharacter == null)
		{
			ClearReport();
			return;
		}
		
		// Updates ui parameters
		CombatReport combatReport = battleCalculation.CalculateStats(selectedAttackerCharacter, selectedDefenderCharacter);
		
		attackerhpLabel.Text = selectedAttackerCharacter.Health.ToString();
		attackerdamageLabel.Text = combatReport.PlayerDamage.ToString();
		attackerhitrateLabel.Text = combatReport.PlayerFinalHit.ToString();
		attackerdoubleLabel.Text = combatReport.PlayerDoubles.ToString();
			
		defenderhpLabel.Text = selectedDefenderCharacter.Health.ToString();
		defenderdamageLabel.Text = combatReport.EnemyDamage.ToString();
		defenderhitrateLabel.Text = combatReport.EnemyFinalHit.ToString();
		defenderdoubleLabel.Text = combatReport.EnemyDoubles.ToString();
	}
	
	private void ClearReport()
	{
		attackerhpLabel.Text = "-";
		attackerdamageLabel.Text = "-";
		attackerhitrateLabel.Text = "-";
		attackerdoubleLabel.Text = "-";

		defenderhpLabel.Text = "-";
		defenderdamageLabel.Text = "-";
		defenderhitrateLabel.Text = "-";
		defenderdoubleLabel.Text = "-";
	}
}
