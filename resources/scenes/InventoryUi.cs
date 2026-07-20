using Godot;
using System;
using System.Collections.Generic;
using Gridfall.Characters.Domain;
using Gridfall.Domain;

public partial class InventoryUi : Control
{
	private VBoxContainer itemList;
	private Label detailsLabel;
	private Button equipButton;
	private Button closeButton;
	private CharacterBase selectedCharacter;
	private Equipment selectedItem;

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
		selectedItem = null;
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

		GD.Print($"[Inventory UI] Refreshing inventory for {selectedCharacter.CharacterName}. Total items: {selectedCharacter.Inventory.Count}");
		foreach (var item in selectedCharacter.Inventory)
		{
			GD.Print($" - Item: {item.Name} | Equipped Weapon: {selectedCharacter.EquippedWeapon?.Name} | Equipped Armor: {selectedCharacter.EquippedArmor?.Name}");
		}

		if (selectedCharacter.Inventory.Count == 0)
		{
			detailsLabel.Text = "Inventory is empty.";
			return;
		}

		foreach (Equipment item in selectedCharacter.Inventory)
		{
			Button itemButton = new Button();
			bool isEquipped = item == selectedCharacter.EquippedWeapon || item == selectedCharacter.EquippedArmor;
			
			if (item is Weapon weapon)
			{
				itemButton.Text = $"{weapon.Name} (Weapon) [DMG: {weapon.Damage}]" + (isEquipped ? " *Equipped*" : "");
			}
			else if (item is Armor armor)
			{
				itemButton.Text = $"{armor.Name} (Armor) [DEF: {armor.DefenseBonus}]" + (isEquipped ? " *Equipped*" : "");
			}
			else
			{
				itemButton.Text = item.Name + (isEquipped ? " *Equipped*" : "");
			}

			itemButton.Pressed += () => SelectItem(item);
			itemList.AddChild(itemButton);
		}

		detailsLabel.Text = "Select an item.";
	}

	private void SelectItem(Equipment item)
	{
		selectedItem = item;
		bool isEquipped = item == selectedCharacter.EquippedWeapon || item == selectedCharacter.EquippedArmor;

		if (item is Weapon weapon)
		{
			string bonusesStr = GetBonusesString(weapon);
			detailsLabel.Text = 
				$"Selected: {weapon.Name} (Weapon)\n" +
				$"Type: {weapon.Type}\n" +
				$"Damage: {weapon.Damage}\n" +
				$"Range: {weapon.Range}\n" +
				$"Hit: {weapon.HitRate}\n" +
				(string.IsNullOrEmpty(bonusesStr) ? "" : $"Bonuses: {bonusesStr}\n") +
				(isEquipped ? "Status: Currently Equipped" : "Status: Unequipped");
		}
		else if (item is Armor armor)
		{
			string bonusesStr = GetBonusesString(armor);
			detailsLabel.Text = 
				$"Selected: {armor.Name} (Armor)\n" +
				$"Weight: {armor.Weight}\n" +
				(string.IsNullOrEmpty(bonusesStr) ? "" : $"Bonuses: {bonusesStr}\n") +
				(isEquipped ? "Status: Currently Equipped" : "Status: Unequipped");
		}
	}

	private string GetBonusesString(Equipment item)
	{
		List<string> list = new List<string>();
		if (item.HpBonus != 0) list.Add($"HP {(item.HpBonus > 0 ? "+" : "")}{item.HpBonus}");
		if (item.StrengthBonus != 0) list.Add($"STR {(item.StrengthBonus > 0 ? "+" : "")}{item.StrengthBonus}");
		if (item.DefenseBonus != 0) list.Add($"DEF {(item.DefenseBonus > 0 ? "+" : "")}{item.DefenseBonus}");
		if (item.SpeedBonus != 0) list.Add($"SPD {(item.SpeedBonus > 0 ? "+" : "")}{item.SpeedBonus}");
		if (item.LuckBonus != 0) list.Add($"LUCK {(item.LuckBonus > 0 ? "+" : "")}{item.LuckBonus}");
		if (item.SkillBonus != 0) list.Add($"SKILL {(item.SkillBonus > 0 ? "+" : "")}{item.SkillBonus}");
		if (item.ResistanceBonus != 0) list.Add($"RES {(item.ResistanceBonus > 0 ? "+" : "")}{item.ResistanceBonus}");
		if (item.MovementBonus != 0) list.Add($"MOV {(item.MovementBonus > 0 ? "+" : "")}{item.MovementBonus}");
		if (item.ConstitutionBonus != 0) list.Add($"CON {(item.ConstitutionBonus > 0 ? "+" : "")}{item.ConstitutionBonus}");
		return string.Join(", ", list);
	}

	private void OnEquipPressed()
	{
		if (selectedCharacter == null || selectedItem == null)
			return;

		if (selectedItem is Weapon weapon)
		{
			selectedCharacter.EquipWeapon(weapon);
			detailsLabel.Text = $"Equipped Weapon: {weapon.Name}";
		}
		else if (selectedItem is Armor armor)
		{
			selectedCharacter.EquipArmor(armor);
			detailsLabel.Text = $"Equipped Armor: {armor.Name}";
		}

		RefreshInventory();
		SelectItem(selectedItem);
	}

	private void OnClosePressed()
	{
		GetParent()?.QueueFree();
	}
}
