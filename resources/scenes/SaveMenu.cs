using Godot;
using System;
using Gridfall.Services;
using Gridfall.Domain;

namespace Gridfall.resources.scenes;

public partial class SaveMenu : Control
{
	[Export] private PackedScene saveSlotScene;
	[Export] private int totalSlots = 3;
	[Export] public bool IsSaveMode { get; set; } = false;
	private VBoxContainer slotContainer;
	private SaveService saveService;
	private Button backButton;

	public override void _Ready()
	{
		slotContainer = GetNode<VBoxContainer>("PanelContainer/VBoxContainer/SlotContainer");
		backButton = GetNode<Button>("PanelContainer/VBoxContainer/BackButton");
		saveService = new SaveService();
		backButton.Pressed += OnBackPressed;

		if (IsSaveMode)
		{
			backButton.Text = "Close";
		}
		else
		{
			backButton.Text = "Back to Main Menu";
		}

		PopulateSlots();
	}

	private void PopulateSlots()
	{
		// Clear old slots
		foreach (Node child in slotContainer.GetChildren())
		{
			child.QueueFree();
		}

		int activeSlot = GameManager.Instance != null ? GameManager.Instance.CurrentSaveSlot : 1;

		for (int i = 1; i <= totalSlots; i++)
		{
			// Add new slot UI row
			SaveSlot slotUI = saveSlotScene.Instantiate<SaveSlot>();
			slotContainer.AddChild(slotUI);
			slotUI.SlotSelected += OnSlotSelected;
			slotUI.SlotDeleted += OnSlotDeleted;

			// Get file data if it exists
			SaveData data = saveService.SaveExists(i) ? saveService.Load(i) : null;
			bool isSelected = (i == activeSlot);
			slotUI.Setup(i, data, isSelected);
		}
	}

	private void OnSlotSelected(int slotNumber)
	{
		GD.Print($"User chose Slot {slotNumber}");
		
		// Tell GameManager which slot is active
		GameManager.Instance.CurrentSaveSlot = slotNumber;
		if (IsSaveMode)
		{
			// Save game
			GD.Print($"Saving current character data to Slot {slotNumber}");
			GameManager.Instance.SaveCurrentCharacter();
		}
		else
		{
			// Load mode: select slot & load data into memory
			GD.Print($"Selected Save Slot {slotNumber}");
			if (saveService.SaveExists(slotNumber))
			{
				GameManager.Instance.LoadCharacterData();
			}
			else
			{
				GameManager.Instance.CurrentLevelNumber = 1;
			}
		}

		PopulateSlots(); 
	}

	private void OnSlotDeleted(int slotNumber)
	{
		GD.Print($"Deleting data in Slot {slotNumber}");
		saveService.DeleteSave(slotNumber);
		PopulateSlots();
	}
	
	private void OnBackPressed()
	{
		if (IsSaveMode)
		{
			QueueFree();
			GD.Print("Save Menu overlay closed.");
		}
		else
		{
			GetTree().ChangeSceneToFile("res://resources/scenes/main_menu.tscn");
		}
	}
	

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			GetViewport().SetInputAsHandled();
			if (IsSaveMode)
			{
				QueueFree();
				GD.Print("Save Menu overlay closed.");
			}
			else
			{
				GetTree().ChangeSceneToFile("res://resources/scenes/main_menu.tscn");
			}
		}
	}
}
