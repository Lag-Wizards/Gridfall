using Godot;
using System;
using Gridfall.Domain;
using Gridfall.Services;

namespace Gridfall.resources.scenes;

public partial class SaveSlot : Button
{
	// Signals to communicate back to the main menu
	[Signal] public delegate void SlotSelectedEventHandler(int slotNumber);
	[Signal] public delegate void SlotDeletedEventHandler(int slotNumber);
	GameManager gameManager;
	private int slotNumber;
	private Label slotNumberLabel;
	private Label detailsLabel;
	private Button deleteButton;

	public override void _Ready()
	{
		// Cache our node references
		slotNumberLabel = GetNode<Label>("HBoxContainer/SlotNumberLabel");
		detailsLabel = GetNode<Label>("HBoxContainer/DetailsLabel");
		deleteButton = GetNode<Button>("HBoxContainer/DeleteButton");

		// Wire up Godot's built-in button pressed signals
		this.Pressed += OnSlotPressed;
		deleteButton.Pressed += OnDeletePressed;
	}

	public void Setup(int slotNumber, SaveData saveData, bool isSelected = false)
	{
		this.slotNumber = slotNumber;
		slotNumberLabel.Text = $"Slot {slotNumber}";

		if (saveData != null)
		{
			int levelNum = saveData.LevelNumber > 0 ? saveData.LevelNumber : 1;
			double avgLevel = 0;
			if (saveData.Characters != null && saveData.Characters.Count > 0)
			{
				double totalLevel = 0;
				foreach (var c in saveData.Characters)
				{
					totalLevel += c.Level;
				}
				avgLevel = totalLevel / saveData.Characters.Count;
			}

			detailsLabel.Text = $"Game Level: {levelNum}\nCharacter Avg Lv.: {Math.Round(avgLevel, 1)}";
			deleteButton.Visible = true;
		}
		else
		{
			detailsLabel.Text = " Empty Slot ";
			deleteButton.Visible = false; // Hide delete if nothing exists
		}

		SetSelected(isSelected);
	}

	public void SetSelected(bool selected)
	{
		if (slotNumberLabel != null)
		{
			slotNumberLabel.Text = selected ? $"▶ Slot {slotNumber} [Selected]" : $"Slot {slotNumber}";
		}
		SelfModulate = selected ? new Color(0.4f, 0.8f, 1.0f) : Colors.White;
	}

	private void OnSlotPressed()
	{
		EmitSignal(SignalName.SlotSelected, slotNumber);
	}

	private void OnDeletePressed()
	{
		EmitSignal(SignalName.SlotDeleted, slotNumber);
	}
}
