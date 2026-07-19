using Godot;
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
	private bool healthPotionSelected;

	public override void _Ready()
	{
		itemList = GetNode<VBoxContainer>("Panel/VBoxContainer/ItemList");
		detailsLabel = GetNode<Label>("Panel/VBoxContainer/DetailsLabel");
		equipButton = GetNode<Button>("Panel/VBoxContainer/EquipButton");
		closeButton = GetNode<Button>("Panel/VBoxContainer/CloseButton");

		equipButton.Pressed += OnActionPressed;
		closeButton.Pressed += OnClosePressed;
	}

	public void SetSelectedCharacter(CharacterBase character)
	{
		selectedCharacter = character;
		selectedWeapon = null;
		healthPotionSelected = false;
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
			equipButton.Disabled = true;
			return;
		}

		bool hasWeapons = selectedCharacter.Inventory.Count > 0;
		bool hasPotions = selectedCharacter.HealthPotionCount > 0;

		if (!hasWeapons && !hasPotions)
		{
			detailsLabel.Text = "Inventory is empty.";
			equipButton.Disabled = true;
			return;
		}

		foreach (Weapon weapon in selectedCharacter.Inventory)
		{
			Button itemButton = new Button();
			itemButton.Text =
				$"{weapon.Name} | DMG: {weapon.Damage} | RNG: {weapon.Range}";
			itemButton.Pressed += () => SelectWeapon(weapon);
			itemList.AddChild(itemButton);
		}

		if (hasPotions)
		{
			Button potionButton = new Button();
			potionButton.Text =
				$"Health Potion x{selectedCharacter.HealthPotionCount}";
			potionButton.Pressed += SelectHealthPotion;
			itemList.AddChild(potionButton);
		}

		selectedWeapon = null;
		healthPotionSelected = false;
		equipButton.Text = "Equip";
		equipButton.Disabled = true;
		detailsLabel.Text = "Select an item.";
	}

	private void SelectWeapon(Weapon weapon)
	{
		selectedWeapon = weapon;
		healthPotionSelected = false;

		equipButton.Text = "Equip";
		equipButton.Disabled = false;

		detailsLabel.Text =
			$"Selected: {weapon.Name}\n" +
			$"Type: {weapon.Type}\n" +
			$"Damage: {weapon.Damage}\n" +
			$"Range: {weapon.Range}\n" +
			$"Hit: {weapon.HitRate}";
	}

	private void SelectHealthPotion()
	{
		selectedWeapon = null;
		healthPotionSelected = true;

		equipButton.Text = "Use";
		equipButton.Disabled = false;

		detailsLabel.Text =
			$"Selected: Health Potion\n" +
			$"Quantity: {selectedCharacter.HealthPotionCount}\n" +
			"Restores 10 HP.";
	}

	private void OnActionPressed()
	{
		if (selectedCharacter == null)
			return;

		if (healthPotionSelected)
		{
			bool potionUsed = selectedCharacter.UseHealthPotion();

			if (potionUsed)
			{
				detailsLabel.Text =
					$"Health potion used.\n" +
					$"HP: {selectedCharacter.Health}/{selectedCharacter.MaxHealth}";
				RefreshInventory();
			}
			else
			{
				detailsLabel.Text =
					"Potion could not be used. Health may already be full.";
			}

			return;
		}

		if (selectedWeapon == null)
			return;

		selectedCharacter.EquipWeapon(selectedWeapon);
		detailsLabel.Text = $"Equipped: {selectedWeapon.Name}";
	}

	private void OnClosePressed()
	{
		GetParent()?.QueueFree();
	}
}