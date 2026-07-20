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
		PopulateSlots();
	}

	private void PopulateSlots()
	{
		// Clear old slots
		foreach (Node child in slotContainer.GetChildren())
		{
			child.QueueFree();
		}

		for (int i = 1; i <= totalSlots; i++)
		{
			// Add new slot UI row
			SaveSlot slotUI = saveSlotScene.Instantiate<SaveSlot>();
			slotContainer.AddChild(slotUI);
			slotUI.SlotSelected += OnSlotSelected;
			slotUI.SlotDeleted += OnSlotDeleted;

			// Get file data if it exists
			SaveData data = saveService.SaveExists(i) ? saveService.Load(i) : null;
			slotUI.Setup(i, data);
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
			
			PopulateSlots(); 
		}
		else
		{
			// Load Game if save exists
			if (saveService.SaveExists(slotNumber))
			{
				GD.Print($"Loading Save Slot {slotNumber} and entering world...");
				GameManager.Instance.LoadCharacterData();
				GetTree().ChangeSceneToFile("res://resources/scenes/level-1.tscn");
			}
		}
	}

	private void OnSlotDeleted(int slotNumber)
	{
		GD.Print($"Deleting data in Slot {slotNumber}");
		saveService.DeleteSave(slotNumber);
		PopulateSlots();
	}
	
	private void OnBackPressed()
	{
		GetTree().ChangeSceneToFile("res://resources/scenes/main_menu.tscn");
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
