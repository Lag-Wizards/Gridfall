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
	private ShopItem _selectedItem;

	private List<ShopItem> _shopItems = new List<ShopItem>
	{
		new ShopItem("Iron Sword", WeaponType.Slash, 8, 1, 2, 80, 10),
		new ShopItem("Steel Sword", WeaponType.Slash, 12, 1, 4, 75, 20),
		new ShopItem("Iron Lance", WeaponType.Pierce, 10, 1, 3, 75, 15)
	};

	public override void _Ready()
	{
		_titleLabel = GetNode<Label>("Panel/VBoxContainer/TitleLabel");
		_coinLabel = GetNode<Label>("Panel/VBoxContainer/CoinLabel");
		_itemList = GetNode<VBoxContainer>("Panel/VBoxContainer/ItemList");
		_detailsLabel = GetNode<Label>("Panel/VBoxContainer/DetailsLabel");
		_buyButton = GetNode<Button>("Panel/VBoxContainer/BuyButton");
		_closeButton = GetNode<Button>("Panel/VBoxContainer/CloseButton");

		_titleLabel.Text = "Equipment Shop";
		_buyButton.Text = "Buy";
		_closeButton.Text = "Close";

		_buyButton.Pressed += OnBuyPressed;
		_closeButton.Pressed += OnClosePressed;

		RefreshShop();
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

		_coinLabel.Text = $"Coins: {GameManager.Instance?.GetCoinCount() ?? 0}";
		_detailsLabel.Text = "Select an item.";

		foreach (ShopItem item in _shopItems)
		{
			Button itemButton = new Button();
			itemButton.Text = $"{item.Name} - {item.Price} coins";
			itemButton.Pressed += () => SelectItem(item);
			_itemList.AddChild(itemButton);
		}
	}

	private void SelectItem(ShopItem item)
	{
		_selectedItem = item;

		_detailsLabel.Text =
			$"Selected: {item.Name}\n" +
			$"Price: {item.Price} coins\n" +
			$"Type: {item.Type}\n" +
			$"Damage: {item.Damage}\n" +
			$"Range: {item.Range}\n" +
			$"Hit: {item.HitRate}";
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

		if (GameManager.Instance == null || !GameManager.Instance.SpendCoins(_selectedItem.Price))
		{
			_detailsLabel.Text = "Not enough coins.";
			return;
		}

		Weapon purchasedWeapon = new Weapon(
			_selectedItem.Type,
			_selectedItem.Name,
			_selectedItem.Damage,
			_selectedItem.Range,
			_selectedItem.Weight,
			_selectedItem.HitRate
		);

		_selectedCharacter.AddWeaponToInventory(purchasedWeapon);
		_coinLabel.Text = $"Coins: {GameManager.Instance.GetCoinCount()}";
		_detailsLabel.Text = $"Purchased: {_selectedItem.Name}";
	}

	private void OnClosePressed()
	{
		GetParent()?.QueueFree();
	}

	private class ShopItem
	{
		public string Name { get; }
		public WeaponType Type { get; }
		public int Damage { get; }
		public int Range { get; }
		public int Weight { get; }
		public int HitRate { get; }
		public int Price { get; }

		public ShopItem(string name, WeaponType type, int damage, int range, int weight, int hitRate, int price)
		{
			Name = name;
			Type = type;
			Damage = damage;
			Range = range;
			Weight = weight;
			HitRate = hitRate;
			Price = price;
		}
	}
}
