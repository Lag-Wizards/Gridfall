using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Services;
using Gridfall.Domain;

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
	private Label attackerTitleLabel;
	private Label defenderTitleLabel;
	CombatUnit selectedAttackerCharacter;
	CombatUnit selectedDefenderCharacter;
	BattleCalculation battleCalculation = new BattleCalculation();

	public override void _Ready()
	{
		var attackerPanel = GetNodeOrNull<PanelContainer>("AttackerPanel");
		var defenderPanel = GetNodeOrNull<PanelContainer>("DefenderPanel");

		if (attackerPanel != null && defenderPanel != null)
		{
			// Attacker VBox
			var attackerVBox = new VBoxContainer();
			attackerPanel.AddChild(attackerVBox);
			
			attackerTitleLabel = new Label { Text = "PLAYER PREVIEW", HorizontalAlignment = HorizontalAlignment.Center };
			attackerVBox.AddChild(attackerTitleLabel);

			attackerhpLabel = new Label();
			attackerdamageLabel = new Label();
			attackerhitrateLabel = new Label();
			attackerdoubleLabel = new Label();

			attackerVBox.AddChild(attackerhpLabel);
			attackerVBox.AddChild(attackerdamageLabel);
			attackerVBox.AddChild(attackerhitrateLabel);
			attackerVBox.AddChild(attackerdoubleLabel);

			// Defender VBox
			var defenderVBox = new VBoxContainer();
			defenderPanel.AddChild(defenderVBox);

			defenderTitleLabel = new Label { Text = "ENEMY PREVIEW", HorizontalAlignment = HorizontalAlignment.Center };
			defenderVBox.AddChild(defenderTitleLabel);

			defenderhpLabel = new Label();
			defenderdamageLabel = new Label();
			defenderhitrateLabel = new Label();
			defenderdoubleLabel = new Label();

			defenderVBox.AddChild(defenderhpLabel);
			defenderVBox.AddChild(defenderdamageLabel);
			defenderVBox.AddChild(defenderhitrateLabel);
			defenderVBox.AddChild(defenderdoubleLabel);
		}
		else
		{
			GD.PrintErr("CombatReportUi: AttackerPanel or DefenderPanel not found in scene tree!");
		}
	}

	public void SetSelectedCharacter(CombatUnit attacker, CombatUnit defender)
	{
		selectedAttackerCharacter = attacker;
		selectedDefenderCharacter = defender;

		if (selectedAttackerCharacter == null || selectedDefenderCharacter == null)
		{
			ClearReport();
			return;
		}

		if (selectedAttackerCharacter is CharacterBase)
		{
			if (attackerTitleLabel != null) attackerTitleLabel.Text = "PLAYER PREVIEW";
			if (defenderTitleLabel != null) defenderTitleLabel.Text = "ENEMY PREVIEW";
		}
		else
		{
			if (attackerTitleLabel != null) attackerTitleLabel.Text = "ENEMY PREVIEW";
			if (defenderTitleLabel != null) defenderTitleLabel.Text = "PLAYER PREVIEW";
		}
		
		// Updates ui parameters
		var combatReport = battleCalculation.CalculateStats(selectedAttackerCharacter, selectedDefenderCharacter);
		
		attackerhpLabel.Text = $"HP: {selectedAttackerCharacter.Health}";
		attackerdamageLabel.Text = $"DMG: {combatReport.PlayerDamage}";
		attackerhitrateLabel.Text = $"HIT: {combatReport.PlayerFinalHit}%";
		attackerdoubleLabel.Text = $"DBL: {combatReport.PlayerDoubles}";
			
		defenderhpLabel.Text = $"HP: {selectedDefenderCharacter.Health}";
		defenderdamageLabel.Text = $"DMG: {combatReport.EnemyDamage}";
		defenderhitrateLabel.Text = $"HIT: {combatReport.EnemyFinalHit}%";
		defenderdoubleLabel.Text = $"DBL: {combatReport.EnemyDoubles}";
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
