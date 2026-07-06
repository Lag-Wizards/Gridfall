using Godot;
using System;
using Gridfall.Characters.Domain;
using Gridfall.Domain;

public partial class InventoryUi : Control
{
	private VBoxContainer itemList;
	private Label detailsLabel;
	private Button equipButton;
	private Button closeButton;
	private CharacterBase selectedCharacter;
	private Weapon selectedWeapon;

	public override void _Ready()
	{
		itemList = GetNode<VBoxContainer>("Panel/VBoxContainer/ItemList");
		detailsLabel = GetNode<Label>("Panel/VBoxContainer/DetailsLabel");
		equipButton = GetNode<Button>("Panel/VBoxContainer/EquipButton");
		closeButton = GetNode<Button>("Panel/VBoxContainer/CloseButton");

		equipButton.Pressed += OnEquipPressed;
		closeButton.Pressed += OnClosePressed;
	}

	public void SetSelectedCharacter(CharacterBase character)
	{
		selectedCharacter = character;
		selectedWeapon = null;
		RefreshInventory();
	}

	private void RefreshInventory()
	{
		foreach (Node child in itemList.GetChildren())
		{
			child.QueueFree();
		}

		if (selectedCharacter == null)
		{
			detailsLabel.Text = "No character selected.";
			return;
		}

		if (selectedCharacter.Inventory.Count == 0)
		{
			detailsLabel.Text = "Inventory is empty.";
			return;
		}

		foreach (Weapon weapon in selectedCharacter.Inventory)
		{
			Button itemButton = new Button();
			itemButton.Text = $"{weapon.Name} | DMG: {weapon.Damage} | RNG: {weapon.Range}";
			itemButton.Pressed += () => SelectWeapon(weapon);
			itemList.AddChild(itemButton);
		}

		detailsLabel.Text = "Select an item.";
	}

	private void SelectWeapon(Weapon weapon)
	{
		selectedWeapon = weapon;
		detailsLabel.Text = $"Selected: {weapon.Name}\nType: {weapon.Type}\nDamage: {weapon.Damage}\nRange: {weapon.Range}\nHit: {weapon.HitRate}";
	}

	private void OnEquipPressed()
	{
		if (selectedCharacter == null || selectedWeapon == null)
			return;

		selectedCharacter.EquipWeapon(selectedWeapon);
		detailsLabel.Text = $"Equipped: {selectedWeapon.Name}";
	}

	private void OnClosePressed()
	{
		GetParent()?.QueueFree();
	}
}
