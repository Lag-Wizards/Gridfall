using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Services;
using Gridfall.Domain;

public partial class BattlePhase : Control
{
	public event Action<bool> OnPlayerChoice;

	private Button fightButton;
	private Button cancelButton;
	private ExpUI expUi;
	private LevelUpUi levelupUi;
	private CombatUnit attacker;
	private CombatUnit defender;
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
		CombatUnit attacker,
		CombatUnit defender)
	{
		this.attacker = attacker;
		this.defender = defender;
		
		combatReport.SetSelectedCharacter(attacker, defender);
		if (attacker is CharacterBase player)
		{
			expUi.SetSelectedCharacter(player);
			levelupUi.SetSelectedCharacter(player);
		}
	}

	public void ShowEnemyAttack(
		CombatUnit attacker,
		CombatUnit defender)
	{
		this.attacker = attacker;
		this.defender = defender;
		
		combatReport.SetSelectedCharacter(attacker, defender);
		if (defender is CharacterBase player)
		{
			expUi.SetSelectedCharacter(player);
			levelupUi.SetSelectedCharacter(player);
		}

		var options = GetNodeOrNull<Control>("VBoxContainerFightOptions");
		if (options != null)
		{
			options.Visible = false;
		}

		// Start combat automatically after 0.5 seconds
		GetTree().CreateTimer(0.5f).Timeout += () =>
		{
			GD.Print($"BattlePhase: Running automatic enemy attack for {attacker.UnitName} against {defender.UnitName}...");
			try
			{
				battleSystem.InitiateCombat(attacker, defender);

				// Show EXP & LevelUp HUD
				expUi.Visible = true;
				levelupUi.Visible = true;
			}
			catch (Exception ex)
			{
				GD.PrintErr($"BattlePhase Exception in ShowEnemyAttack: {ex}");
			}
			finally
			{
				// Automatically clean up overlay after 2.5 seconds
				GetTree().CreateTimer(2.5f).Timeout += () => {
					GD.Print("BattlePhase: Closing automatic enemy attack overlay CanvasLayer.");
					GetParent()?.QueueFree();
				};
			}
		};
	}

	private void OnFightPressed()
	{
		GD.Print("Fight button pressed initiating combat...");
		OnPlayerChoice?.Invoke(true);

		try
		{
			// Execute battle
			battleSystem.InitiateCombat(attacker, defender);
			
			// Show EXP & LevelUp HUD
			expUi.Visible = true;
			levelupUi.Visible = true;
		}
		catch (Exception ex)
		{
			GD.PrintErr($"BattlePhase Exception in OnFightPressed: {ex}");
		}
		finally
		{
			// Hide fight/cancel controls during animation
			var options = GetNodeOrNull<Control>("VBoxContainerFightOptions");
			if (options != null)
			{
				options.Visible = false;
			}

			// Automatically clean up overlay after 2.5 seconds
			GetTree().CreateTimer(2.5f).Timeout += () => {
				GD.Print("Closing combat overlay");
				GetParent()?.QueueFree();
			};
		}
	}

	private void OnCancelPressed()
	{
		OnPlayerChoice?.Invoke(false);
		GetParent()?.QueueFree();
	}
}
