using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gridfall.Domain;
using Gridfall.Domain.Enums;
using Gridfall.Services;
using Gridfall.Characters.Domain;
using Gridfall.Contracts;
using Gridfall.Controllers;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }

	private EnemyAI _enemyAI = new EnemyAI();

	public GamePhase CurrentPhase { get; private set; } = GamePhase.PlayerMovement;

	private int _enemyCount = 0;
	public CharacterBase CurrentCharacter { get; private set; }
	public IGridManager GridManager { get; private set; }

	private SaveService _saveService = new SaveService();
	private Control _pauseMenu;

	private PlayerController _playerController;
	private CharacterNode _playerCharacterNode;
	private CanvasLayer _playerUi;
	private Label _movementPhaseLabel;
	private Label _playerHealthLabel;
	private Label _playerLevelLabel;
	private Label _movementRemainingLabel;
	private Label _playerCoinLabel;
	private Button _nextPhaseButton;
	private int _coinCount = 0;
	private RandomNumberGenerator _coinRandomizer = new RandomNumberGenerator();
	private string _currentPhaseText = "Phase: Player Movement";
	private Color _currentPhaseColor = Colors.LimeGreen;

	public override void _Ready()
	{
		Instance = this;
		_coinRandomizer.Randomize();

		Events.OnEnemySpawn += HandleEnemySpawn;
		Events.OnPlayerDeath += HandlePlayerDeath;
		Events.OnEnemyDeath += HandleEnemyDeath;

		ProcessMode = ProcessModeEnum.Always;

		GD.Print("!!!!!!!! GAME MANAGER READY !!!!!!!!");
		LoadCharacterData();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			TogglePauseMenu();
		}

		if (GodotObject.IsInstanceValid(_playerUi) && _playerUi.Visible)
		{
			var currentScene = GetTree().CurrentScene;
			if (currentScene != null && (currentScene.SceneFilePath == "res://resources/scenes/main_menu.tscn" ||
				currentScene.Name == "MainMenu" ||
				currentScene.Name == "Victory" ||
				currentScene.Name == "GameOver"))
			{
				_playerUi.Visible = false;
			}
		}
	}

	public int CurrentLevelNumber { get; set; } = 1;

	public void RegisterGridManager(IGridManager gridManager)
	{
		GridManager = gridManager;
		_enemyCount = 0;

		var currentScene = GetTree()?.CurrentScene;
		if (currentScene != null && !string.IsNullOrEmpty(currentScene.SceneFilePath))
		{
			string path = currentScene.SceneFilePath;
			int dashIndex = path.LastIndexOf('-');
			int dotIndex = path.LastIndexOf('.');
			if (dashIndex != -1 && dotIndex != -1 && dotIndex > dashIndex + 1)
			{
				string numStr = path.Substring(dashIndex + 1, dotIndex - dashIndex - 1);
				if (int.TryParse(numStr, out int levelNum))
				{
					CurrentLevelNumber = levelNum;
					GD.Print($"Current level number: {CurrentLevelNumber}");
				}
			}
		}

		var playerNode = FindPlayerCharacterNode();
		if (playerNode != null)
		{
			playerNode.InitializeGridPosition();
		}

		StartGamePhases();
	}

	public void HandleEnemySpawn()
	{
		_enemyCount++;
		GD.Print("Enemies alive: ", _enemyCount);
	}

	private void TogglePauseMenu()
	{
		if (_pauseMenu == null || !GodotObject.IsInstanceValid(_pauseMenu))
		{
			if (_pauseMenu == null)
			{
				var pauseScene = GD.Load<PackedScene>("res://resources/scenes/pause_menu.tscn");
				if (pauseScene != null)
				{
					var canvas = new CanvasLayer
					{
						Name = "PauseUI",
						Layer = 20,
						ProcessMode = ProcessModeEnum.Always
					};
					AddChild(canvas);

					var pauseNode = pauseScene.Instantiate<Control>();
					canvas.AddChild(pauseNode);

					pauseNode.Visible = false;
					_pauseMenu = pauseNode;
				}
				else
				{
					GD.PrintErr("Failed to load res://resources/scenes/pause_menu.tscn");
				}
			}
		}

		if (_pauseMenu == null)
		{
			GD.Print("PauseMenu not found.");
			return;
		}

		bool shouldPause = !GetTree().Paused;
		GetTree().Paused = shouldPause;
		_pauseMenu.Visible = shouldPause;
	}

	public void LoadCharacterData()
	{
		if (_saveService.SaveExists())
		{
			SaveData saveData = _saveService.Load();

			CurrentCharacter = new CharacterBase();
			CurrentCharacter.LoadFromSave(saveData);

			GD.Print("GameManager loaded saved character data.");
			GD.Print(CurrentCharacter.CharacterName);
			GD.Print(CurrentCharacter.Level);
		}
		else
		{
			GD.Print("GameManager found no save data.");
		}
	}

	public void SetCurrentCharacter(CharacterBase character)
	{
		CurrentCharacter = character;
		GD.Print("GameManager current character set.");
	}

	public void SaveCurrentCharacter()
	{
		if (CurrentCharacter == null)
		{
			GD.Print("No current character to save.");
			return;
		}

		_saveService.Save(CurrentCharacter);
		GD.Print("Current character saved.");
	}

	public void HandlePlayerDeath()
	{
		_coinCount = 0;
		UpdateHud();
		GetTree().ChangeSceneToFile("res://resources/scenes/game_over.tscn");
	}

	public void HandleEnemyDeath()
	{
		int coinsEarned = _coinRandomizer.RandiRange(1, 10);
		_coinCount += coinsEarned;
		GD.Print($"Enemy defeated! Earned {coinsEarned} coins. Total coins: {_coinCount}");

		_enemyCount--;
		if (_enemyCount <= 0)
		{
			if (CurrentCharacter != null)
			{
				CurrentCharacter.Heal(CurrentCharacter.MaxHealth);
				GD.Print("GameManager: Player character fully healed upon victory!");
			}
			var error = GetTree().ChangeSceneToFile("res://resources/scenes/victory.tscn");
			if (error != Error.Ok)
			{
				GD.PrintErr($"Failed to change scene to victory.tscn, Error: {error}");
			}
		}
		UpdateHud();
	}

	public void RegisterPlayerController(PlayerController pc)
	{
		_playerController = pc;
		GD.Print("GameManager: PlayerController registered.");
		if (CurrentPhase == GamePhase.PlayerMovement)
		{
			pc.StartMovementPhase();
		}
		UpdateHud();
	}

	public void RegisterPlayerCharacterNode(CharacterNode node)
	{
		_playerCharacterNode = node;
		GD.Print("GameManager: Player CharacterNode registered.");
		UpdateHud();
	}

	private Node GetSceneRoot()
	{
		var sceneRoot = GetTree().CurrentScene;
		if (sceneRoot != null)
			return sceneRoot;

		var root = GetTree().Root;
		if (root != null && root.GetChildCount() > 0)
		{
			return root.GetChild(root.GetChildCount() - 1);
		}
		return null;
	}

	private PlayerController FindPlayerController()
	{
		if (_playerController == null || !GodotObject.IsInstanceValid(_playerController))
		{
			var sceneRoot = GetSceneRoot();
			_playerController = FindNodeRecursive<PlayerController>(sceneRoot);
		}
		return _playerController;
	}

	private CharacterNode FindPlayerCharacterNode()
	{
		if (_playerCharacterNode == null || !GodotObject.IsInstanceValid(_playerCharacterNode))
		{
			var sceneRoot = GetSceneRoot();
			_playerCharacterNode = sceneRoot?.GetNodeOrNull<CharacterNode>("Character");
		}
		return _playerCharacterNode;
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

	private void StartGamePhases()
	{
		EnsureUiNodes();
		TransitionToPhase(GamePhase.PlayerMovement);
	}

	public async void TransitionToPhase(GamePhase nextPhase)
	{
		if (!GodotObject.IsInstanceValid(GridManager?.GridMap))
			return;

		CurrentPhase = nextPhase;

		switch (CurrentPhase)
		{
			case GamePhase.PlayerMovement:
				GD.Print("GameManager: Entering Player Movement Phase");
				SetPhaseLabel("Phase: Player Movement", Colors.LimeGreen);

				var pc = FindPlayerController();
				if (pc != null)
				{
					pc.StartMovementPhase();
				}
				UpdateHud();
				if (GodotObject.IsInstanceValid(_nextPhaseButton))
					_nextPhaseButton.Visible = true;
				break;

			case GamePhase.PlayerBattle:
				GD.Print("GameManager: Entering Player Battle Phase");
				SetPhaseLabel("Phase: Player Battle", Colors.OrangeRed);
				if (GodotObject.IsInstanceValid(_nextPhaseButton))
					_nextPhaseButton.Visible = false;

				var pcBattle = FindPlayerController();
				if (pcBattle != null)
					pcBattle.EndMovementPhase();

				var playerNode = FindPlayerCharacterNode();
				if (playerNode != null)
				{
					var enemies = playerNode.GetEnemiesInRange();
					if (enemies != null && enemies.Count > 0)
					{
						bool playerFought = await PromptPlayerBattlePhase(playerNode.Stats, enemies);
						GD.Print($"GameManager: Player finished battle (fought = {playerFought})");
					}
					else
					{
						GD.Print("GameManager: No enemy in range for player attack.");
					}
				}

				if (!GodotObject.IsInstanceValid(GridManager?.GridMap))
					return;

				TransitionToPhase(GamePhase.EnemyBattle);
				break;

			case GamePhase.EnemyBattle:
				GD.Print("GameManager: Entering Enemy Battle Phase");
				SetPhaseLabel("Phase: Enemy Battle", Colors.Red);
				if (GodotObject.IsInstanceValid(_nextPhaseButton))
					_nextPhaseButton.Visible = false;

				var pcEnemy = FindPlayerController();
				if (pcEnemy != null)
					pcEnemy.EndMovementPhase();

				await RunEnemyTurn();

				if (!GodotObject.IsInstanceValid(GridManager?.GridMap))
					return;

				TransitionToPhase(GamePhase.PlayerMovement);
				break;
		}
	}

	public void EndPlayerMovementPhase()
	{
		TransitionToPhase(GamePhase.PlayerBattle);
	}

	private void OnNextPhasePressed()
	{
		if (CurrentPhase == GamePhase.PlayerMovement)
		{
			var pc = FindPlayerController();
			if (pc != null)
				pc.EndMovementPhase();
			EndPlayerMovementPhase();
		}
	}

	private void EnsureUiNodes()
	{
		GD.Print("Ensuring UI Nodes");

		if (GodotObject.IsInstanceValid(_playerUi))
		{
			_playerUi.Visible = true;
			UpdateHud();
			return;
		}

		_playerUi = new CanvasLayer
		{
			Name = "PlayerUi",
			Layer = 1
		};
		AddChild(_playerUi);

		var panel = new Panel
		{
			Name = "PlayerUiPanel",
			Size = new Vector2(240, 160),
			Position = new Vector2(8, 8)
		};
		_playerUi.AddChild(panel);

		_movementPhaseLabel = new Label
		{
			Name = "MovementPhaseLabel",
			Text = "Movement Phase: ACTIVE",
			Position = new Vector2(10, 10)
		};
		panel.AddChild(_movementPhaseLabel);

		_playerLevelLabel = new Label
		{
			Name = "PlayerLevelLabel",
			Text = "Level: 1",
			Position = new Vector2(10, 38)
		};
		panel.AddChild(_playerLevelLabel);

		_playerHealthLabel = new Label
		{
			Name = "PlayerHealthLabel",
			Text = "HP: 20/20",
			Position = new Vector2(10, 66)
		};
		panel.AddChild(_playerHealthLabel);

		_movementRemainingLabel = new Label
		{
			Name = "MovementRemainingLabel",
			Text = "Move: 5/5",
			Position = new Vector2(10, 94)
		};
		panel.AddChild(_movementRemainingLabel);

		_playerCoinLabel = new Label
		{
			Name = "PlayerCoinLabel",
			Text = "Coins: 0",
			Position = new Vector2(10, 122)
		};
		panel.AddChild(_playerCoinLabel);

		_nextPhaseButton = new Button
		{
			Name = "NextPhaseButton",
			Text = "Next Phase",
			Position = new Vector2(10, 122)
		};
		panel.AddChild(_nextPhaseButton);
		_nextPhaseButton.Pressed += OnNextPhasePressed;

		GD.Print("GameManager: EnsureUiNodes completed.");
	}

	public void UpdateHud()
	{
		var playerNode = FindPlayerCharacterNode();
		var pc = FindPlayerController();

		if (GodotObject.IsInstanceValid(_playerLevelLabel) && playerNode?.Stats != null)
		{
			_playerLevelLabel.Text = $"Level: {playerNode.Stats.Level}";
		}

		if (GodotObject.IsInstanceValid(_playerHealthLabel) && playerNode?.Stats != null)
		{
			_playerHealthLabel.Text = $"HP: {playerNode.Stats.Health}/{playerNode.Stats.MaxHealth}";
		}

		if (GodotObject.IsInstanceValid(_movementRemainingLabel) && pc != null && playerNode?.Stats != null)
		{
			_movementRemainingLabel.Text = $"Move: {pc.RemainingMovement}/{playerNode.Stats.MovementRange}";
		}

		if (GodotObject.IsInstanceValid(_playerCoinLabel))
		{
			_playerCoinLabel.Text = $"Coins: {_coinCount}";
		}

		if (GodotObject.IsInstanceValid(_movementPhaseLabel))
		{
			_movementPhaseLabel.Text = _currentPhaseText;
			_movementPhaseLabel.Modulate = _currentPhaseColor;
		}
	}

	public int GetCoinCount()
	{
		return _coinCount;
	}

	public bool SpendCoins(int amount)
	{
		if (_coinCount < amount)
			return false;

		_coinCount -= amount;
		UpdateHud();
		return true;
	}

	public void AddCoins(int amount)
	{
		_coinCount += amount;
		UpdateHud();
	}

	private void SetPhaseLabel(string text, Color color)
	{
		_currentPhaseText = text;
		_currentPhaseColor = color;
		UpdateHud();
	}

	// Combat UI Prompt Handlers
	private Task<bool> PromptPlayerBattlePhase(CombatUnit player, List<EnemyNode> enemies)
	{
		var tcs = new TaskCompletionSource<bool>();

		var battlePhaseScene = GD.Load<PackedScene>("res://resources/scenes/BattlePhase.tscn");
		if (battlePhaseScene == null)
		{
			GD.PrintErr("GameManager: Failed to load BattlePhase.tscn");
			tcs.SetResult(false);
			return tcs.Task;
		}

		var canvasLayer = new CanvasLayer
		{
			Name = "BattlePhaseCanvas",
			Layer = 2
		};

		var battlePhaseNode = battlePhaseScene.Instantiate<BattlePhase>();
		canvasLayer.AddChild(battlePhaseNode);

		bool playerChoiceMade = false;
		bool playerFought = false;

		battlePhaseNode.OnPlayerChoice += (fought) =>
		{
			playerChoiceMade = true;
			playerFought = fought;
		};

		canvasLayer.TreeExited += () =>
		{
			tcs.TrySetResult(playerChoiceMade && playerFought);
			UpdateHud();
		};

		GetTree().CurrentScene.AddChild(canvasLayer);
		battlePhaseNode.ShowBattle(player, enemies);

		return tcs.Task;
	}

	private Task PromptEnemyAttack(CombatUnit enemy, CombatUnit player)
	{
		var tcs = new TaskCompletionSource<bool>();

		var battlePhaseScene = GD.Load<PackedScene>("res://resources/scenes/BattlePhase.tscn");
		if (battlePhaseScene == null)
		{
			GD.PrintErr("GameManager: Failed to load BattlePhase.tscn");
			tcs.SetResult(false);
			return tcs.Task;
		}

		var canvasLayer = new CanvasLayer
		{
			Name = "BattlePhaseCanvas",
			Layer = 2
		};

		var battlePhaseNode = battlePhaseScene.Instantiate<BattlePhase>();
		canvasLayer.AddChild(battlePhaseNode);

		canvasLayer.TreeExited += () =>
		{
			tcs.TrySetResult(true);
			UpdateHud();
		};

		GetTree().CurrentScene.AddChild(canvasLayer);
		battlePhaseNode.ShowEnemyAttack(enemy, player);

		return tcs.Task;
	}

	private async Task RunEnemyTurn()
	{
		GD.Print("GameManager: Starting Enemy Turn");

		var sceneRoot = GetTree().CurrentScene;
		var enemies = FindEnemyNodesRecursive(sceneRoot);
		var playerNode = FindPlayerCharacterNode();

		if (playerNode == null)
			return;

		foreach (var enemyNode in enemies)
		{
			if (!GodotObject.IsInstanceValid(GridManager?.GridMap))
				return;

			if (!GodotObject.IsInstanceValid(playerNode) || !playerNode.Stats.IsAlive())
				return;

			if (!GodotObject.IsInstanceValid(enemyNode) ||
				enemyNode.Stats == null ||
				!enemyNode.Stats.IsAlive())
			{
				continue;
			}

			Vector2I enemyTile = GridManager.WorldToMap(enemyNode.GlobalPosition);

			Vector2I playerTile = playerNode.CurrentTile;

			string action = _enemyAI.DetermineAction(enemyTile, playerTile);

			if (action == "Attack")
			{
				GD.Print(
					$"{enemyNode.Stats.UnitName} attacks the player.");

				await PromptEnemyAttack(enemyNode.Stats, playerNode.Stats);
			}
			else if (action == "Move")
			{
				List<Vector2I> path = _enemyAI.FindPath(enemyTile, playerTile, GridManager);

				if (path.Count <= 1)
					continue;

				int movement = enemyNode.Stats.MoveDistance;

				int steps = Mathf.Min(movement, path.Count - 1);

				for (int i = 1; i <= steps; i++)
				{
					Vector2I destination = path[i];

					TileState tileState = GridManager.GetTileStateAt(destination);

					if (tileState == null)
						break;

					TileState currentTile = GridManager.GetTileStateAt(enemyTile);

					if (currentTile != null)
						currentTile.CurrentOccupant = null;

					tileState.CurrentOccupant = enemyNode;

					enemyTile = destination;

					enemyNode.GlobalPosition = GridManager.MapToWorld(enemyTile);

					await ToSignal(GetTree().CreateTimer(0.15f), SceneTreeTimer.SignalName.Timeout);

					int distance = Mathf.Abs(playerTile.X - enemyTile.X) + Mathf.Abs(playerTile.Y - enemyTile.Y);

					if (distance <= 1)
					{
						await PromptEnemyAttack(enemyNode.Stats, playerNode.Stats);
						break;
					}
				}
			}
			GD.Print("GameManager: Enemy Turn completed.");
		}
	}

	private List<EnemyNode> FindEnemyNodesRecursive(Node node)
	{
		var list = new List<EnemyNode>();
		if (node is EnemyNode enemy)
			list.Add(enemy);

		if (node != null)
		{
			foreach (var child in node.GetChildren())
			{
				list.AddRange(FindEnemyNodesRecursive(child));
			}
		}
		return list;
	}
}
