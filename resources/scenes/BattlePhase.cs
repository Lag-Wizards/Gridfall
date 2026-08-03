using Godot;
using System;
using System.Collections.Generic;
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
	private List<EnemyNode> _enemiesInRange;
	private int _selectedEnemyIndex = 0;
	private Button prevEnemyButton;
	private Button nextEnemyButton;
	private Vector2 _originalCameraPosition;
	private bool _hasOriginalCameraPosition = false;

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
		cancelButton.Text = "End Phase";

		fightButton.Pressed += OnFightPressed;
		cancelButton.Pressed += OnCancelPressed;

		var fightOptionsContainer = GetNodeOrNull<VBoxContainer>("VBoxContainerFightOptions");
		if (fightOptionsContainer != null)
		{
			var arrowContainer = new HBoxContainer();
			arrowContainer.Name = "ArrowContainer";
			fightOptionsContainer.AddChild(arrowContainer);
			fightOptionsContainer.MoveChild(arrowContainer, 0);

			prevEnemyButton = new Button();
			prevEnemyButton.Name = "PrevEnemyButton";
			prevEnemyButton.Text = "< Prev";
			prevEnemyButton.SizeFlagsHorizontal = Control.SizeFlags.Expand | Control.SizeFlags.Fill;
			arrowContainer.AddChild(prevEnemyButton);
			prevEnemyButton.Pressed += OnPrevEnemyPressed;

			nextEnemyButton = new Button();
			nextEnemyButton.Name = "NextEnemyButton";
			nextEnemyButton.Text = "Next >";
			nextEnemyButton.SizeFlagsHorizontal = Control.SizeFlags.Expand | Control.SizeFlags.Fill;
			arrowContainer.AddChild(nextEnemyButton);
			nextEnemyButton.Pressed += OnNextEnemyPressed;

			prevEnemyButton.Visible = false;
			nextEnemyButton.Visible = false;
		}
	}

	private Camera2D FindCamera()
	{
		var sceneRoot = GetTree()?.CurrentScene;
		if (sceneRoot == null) return null;
		return sceneRoot.GetNodeOrNull<Camera2D>("Camera2D") ?? FindNodeRecursive<Camera2D>(sceneRoot);
	}

	private T FindNodeRecursive<T>(Node node) where T : Node
	{
		if (node is T target)
			return target;

		if (node != null)
		{
			foreach (var child in node.GetChildren())
			{
				if (child is Node childNode)
				{
					var found = FindNodeRecursive<T>(childNode);
					if (found != null)
						return found;
				}
			}
		}

		return null;
	}

	private void UpdateSelectedEnemy(int index)
	{
		if (_enemiesInRange == null || _enemiesInRange.Count == 0) return;

		if (index < 0) index = _enemiesInRange.Count - 1;
		if (index >= _enemiesInRange.Count) index = 0;

		_selectedEnemyIndex = index;
		var selectedEnemyNode = _enemiesInRange[_selectedEnemyIndex];
		
		if (GodotObject.IsInstanceValid(selectedEnemyNode))
		{
			this.defender = selectedEnemyNode.Stats;
			combatReport.SetSelectedCharacter(attacker, defender);

			var camera = FindCamera();
			if (GodotObject.IsInstanceValid(camera))
			{
				camera.GlobalPosition = selectedEnemyNode.GlobalPosition;
			}
		}

		bool showCycleButtons = _enemiesInRange.Count > 1;
		if (nextEnemyButton != null) nextEnemyButton.Visible = showCycleButtons;
		if (prevEnemyButton != null) prevEnemyButton.Visible = showCycleButtons;
	}

	private void OnPrevEnemyPressed()
	{
		UpdateSelectedEnemy(_selectedEnemyIndex - 1);
	}

	private void OnNextEnemyPressed()
	{
		UpdateSelectedEnemy(_selectedEnemyIndex + 1);
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		RestoreCamera();
	}

	private void RestoreCamera()
	{
		if (_hasOriginalCameraPosition)
		{
			var camera = FindCamera();
			if (GodotObject.IsInstanceValid(camera))
			{
				camera.GlobalPosition = _originalCameraPosition;
			}
		}
	}

	public void ShowBattle(
		CombatUnit attacker,
		List<EnemyNode> enemies)
	{
		this.attacker = attacker;
		this._enemiesInRange = enemies;
		
		if (attacker is CharacterBase player)
		{
			expUi.SetSelectedCharacter(player);
			levelupUi.SetSelectedCharacter(player);
		}

		var camera = FindCamera();
		if (GodotObject.IsInstanceValid(camera))
		{
			_originalCameraPosition = camera.GlobalPosition;
			_hasOriginalCameraPosition = true;
		}

		UpdateSelectedEnemy(0);
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
			expUi.RefreshBar();
			levelupUi.SetSelectedCharacter(player);
		}

		var options = GetNodeOrNull<Control>("VBoxContainerFightOptions");
		if (options != null)
		{
			options.Visible = false;
		}

		// Start combat automatically after 0.5 seconds
		if (IsInsideTree())
		{
			GetTree().CreateTimer(0.5f).Timeout += () =>
			{
				if (!GodotObject.IsInstanceValid(this) || !IsInsideTree())
					return;

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
					if (IsInsideTree())
					{
						// Automatically clean up overlay after 2.5 seconds
						GetTree().CreateTimer(2.5f).Timeout += () => {
							if (GodotObject.IsInstanceValid(this) && IsInsideTree())
							{
								GD.Print("BattlePhase: Closing automatic enemy attack overlay CanvasLayer.");
								GetParent()?.QueueFree();
							}
						};
					}
				}
			};
		}
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
			expUi.RefreshBar();
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
			if (IsInsideTree())
			{
				GetTree().CreateTimer(2.5f).Timeout += () => {
					if (GodotObject.IsInstanceValid(this) && IsInsideTree())
					{
						GD.Print("Closing combat overlay");
						GetParent()?.QueueFree();
					}
				};
			}
		}
	}

	private void OnCancelPressed()
	{
		OnPlayerChoice?.Invoke(false);
		GetParent()?.QueueFree();
	}
}
