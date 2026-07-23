using Godot;
using Gridfall.Characters.Domain;
using Gridfall.Domain;
using Gridfall.Domain.Enums;
using Gridfall.Services;
using System.Collections.Generic;

public partial class ShopUi : Control
{
	private Label _titleLabel;
	private Label _coinLabel;
	private Label _detailsLabel;
	private VBoxContainer _itemList;
	private Button _buyButton;
	private Button _closeButton;

	private CharacterBase _selectedCharacter;
	private Equipment _selectedItem;

	[Export]
	public string[] ShopItemNames { get; set; } =
	{
		"Iron Sword",
		"Steel Sword",
		"Iron Lance",
		"Iron Armor"
	};

	private readonly List<Equipment> _shopItems = new();

	public override void _Ready()
	{
		_titleLabel = GetNode<Label>(
			"Panel/VBoxContainer/TitleLabel"
		);

		_coinLabel = GetNode<Label>(
			"Panel/VBoxContainer/CoinLabel"
		);

		_itemList = GetNode<VBoxContainer>(
			"Panel/VBoxContainer/ItemList"
		);

		_detailsLabel = GetNode<Label>(
			"Panel/VBoxContainer/DetailsLabel"
		);

		_buyButton = GetNode<Button>(
			"Panel/VBoxContainer/ButtonRow/BuyButton"
		);

		_closeButton = GetNode<Button>(
			"Panel/VBoxContainer/ButtonRow/CloseButton"
		);

		_titleLabel.Text = "Equipment Shop";
		_buyButton.Text = "Buy";
		_closeButton.Text = "Close";

		_buyButton.Pressed += OnBuyPressed;
		_closeButton.Pressed += OnClosePressed;

		InitializeShopItems();
		RefreshShop();
	}

	private void InitializeShopItems()
	{
		_shopItems.Clear();

		if (ShopItemNames == null)
		{
			return;
		}

		foreach (string itemName in ShopItemNames)
		{
			Equipment item = ItemFactory.CreateEquipment(itemName);

			if (item != null)
			{
				_shopItems.Add(item);
			}
		}
	}

	public void SetSelectedCharacter(CharacterBase character)
	{
		_selectedCharacter = character;
		RefreshShop();
	}

	private void RefreshShop()
	{
		foreach (Node child in _itemList.GetChildren())
		{
			child.QueueFree();
		}

		int coinCount = GameManager.Instance?.GetCoinCount() ?? 0;

		_coinLabel.Text = $"Coins: {coinCount}";
		_detailsLabel.Text = "Select an item.";
		_selectedItem = null;

		foreach (Equipment item in _shopItems)
		{
			Button itemButton = new Button
			{
				Text = $"{item.Name} - {item.Price} coins",
				CustomMinimumSize = new Vector2(0, 36)
			};

			Equipment selectedItem = item;

			itemButton.Pressed += () => SelectItem(selectedItem);

			_itemList.AddChild(itemButton);
		}
	}

	private void SelectItem(Equipment item)
	{
		_selectedItem = item;

		if (item is Weapon weapon)
		{
			string bonuses = GetBonusesString(weapon);

			_detailsLabel.Text =
				$"Selected: {weapon.Name} (Weapon)\n" +
				$"Price: {weapon.Price} coins\n" +
				$"Type: {weapon.Type}\n" +
				$"Damage: {weapon.Damage}\n" +
				$"Range: {weapon.Range}\n" +
				$"Hit: {weapon.HitRate}\n" +
				(string.IsNullOrEmpty(bonuses)
					? ""
					: $"Bonuses: {bonuses}");
		}
		else if (item is Armor armor)
		{
			string bonuses = GetBonusesString(armor);

			_detailsLabel.Text =
				$"Selected: {armor.Name} (Armor)\n" +
				$"Price: {armor.Price} coins\n" +
				$"Weight: {armor.Weight}\n" +
				(string.IsNullOrEmpty(bonuses)
					? ""
					: $"Bonuses: {bonuses}");
		}
	}

	private string GetBonusesString(Equipment item)
	{
		List<string> bonuses = new();

		if (item.HpBonus != 0)
		{
			bonuses.Add($"HP {FormatBonus(item.HpBonus)}");
		}

		if (item.StrengthBonus != 0)
		{
			bonuses.Add($"STR {FormatBonus(item.StrengthBonus)}");
		}

		if (item.DefenseBonus != 0)
		{
			bonuses.Add($"DEF {FormatBonus(item.DefenseBonus)}");
		}

		if (item.SpeedBonus != 0)
		{
			bonuses.Add($"SPD {FormatBonus(item.SpeedBonus)}");
		}

		if (item.LuckBonus != 0)
		{
			bonuses.Add($"LUCK {FormatBonus(item.LuckBonus)}");
		}

		if (item.SkillBonus != 0)
		{
			bonuses.Add($"SKILL {FormatBonus(item.SkillBonus)}");
		}

		if (item.ResistanceBonus != 0)
		{
			bonuses.Add($"RES {FormatBonus(item.ResistanceBonus)}");
		}

		if (item.MovementBonus != 0)
		{
			bonuses.Add($"MOV {FormatBonus(item.MovementBonus)}");
		}

		if (item.ConstitutionBonus != 0)
		{
			bonuses.Add($"CON {FormatBonus(item.ConstitutionBonus)}");
		}

		return string.Join(", ", bonuses);
	}

	private string FormatBonus(int value)
	{
		return value > 0 ? $"+{value}" : value.ToString();
	}

	private void OnBuyPressed()
	{
		if (_selectedCharacter == null)
		{
			_detailsLabel.Text = "No character selected.";
			return;
		}

		if (_selectedItem == null)
		{
			_detailsLabel.Text = "Select an item first.";
			return;
		}

		if (GameManager.Instance == null)
		{
			_detailsLabel.Text = "Game manager unavailable.";
			return;
		}

		if (!GameManager.Instance.SpendCoins(_selectedItem.Price))
		{
			_detailsLabel.Text = "Not enough coins.";
			return;
		}

		Equipment purchasedItem =
			ItemFactory.CreateEquipment(_selectedItem.Name);

		if (purchasedItem == null)
		{
			_detailsLabel.Text = "Unable to purchase this item.";
			return;
		}

		_selectedCharacter.AddEquipmentToInventory(purchasedItem);

		_coinLabel.Text =
			$"Coins: {GameManager.Instance.GetCoinCount()}";

		_detailsLabel.Text =
			$"Purchased: {_selectedItem.Name}";
	}

	private void OnClosePressed()
	{
		QueueFree();
	}
}
