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

	public void Setup(int slotNumber, SaveData saveData)
	{
		this.slotNumber = slotNumber;
		slotNumberLabel.Text = $"Slot {slotNumber}";

		if (saveData != null)
		{
			detailsLabel.Text = $"{saveData.CharacterName} - Lv. {saveData.Level}";
			deleteButton.Visible = true;
		}
		else
		{
			detailsLabel.Text = " Empty Slot ";
			deleteButton.Visible = false; // Hide delete if nothing exists
		}
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
