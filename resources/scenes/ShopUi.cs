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
	public string[] ShopItemNames { get; set; } = new string[] { "Iron Sword", "Steel Sword", "Iron Lance", "Iron Armor" };

	private List<Equipment> _shopItems = new List<Equipment>();

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
		GD.Print("SHOP CREATED FROM:\n" + System.Environment.StackTrace);
		InitializeShopItems();
		RefreshShop();
	}

	private void InitializeShopItems()
	{
		_shopItems.Clear();
		if (ShopItemNames != null)
		{
			foreach (var name in ShopItemNames)
			{
				var item = ItemFactory.CreateEquipment(name);
				if (item != null)
				{
					_shopItems.Add(item);
				}
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

		_coinLabel.Text = $"Coins: {GameManager.Instance?.GetCoinCount() ?? 0}";
		_detailsLabel.Text = "Select an item.";

		foreach (Equipment item in _shopItems)
		{
			Button itemButton = new Button();
			itemButton.Text = $"{item.Name} - {item.Price} coins";
			itemButton.Pressed += () => SelectItem(item);
			_itemList.AddChild(itemButton);
		}
	}

	private void SelectItem(Equipment item)
	{
		_selectedItem = item;

		if (item is Weapon weapon)
		{
			string bonusesStr = GetBonusesString(weapon);
			_detailsLabel.Text =
				$"Selected: {weapon.Name} (Weapon)\n" +
				$"Price: {weapon.Price} coins\n" +
				$"Type: {weapon.Type}\n" +
				$"Damage: {weapon.Damage}\n" +
				$"Range: {weapon.Range}\n" +
				$"Hit: {weapon.HitRate}\n" +
				(string.IsNullOrEmpty(bonusesStr) ? "" : $"Bonuses: {bonusesStr}");
		}
		else if (item is Armor armor)
		{
			string bonusesStr = GetBonusesString(armor);
			_detailsLabel.Text =
				$"Selected: {armor.Name} (Armor)\n" +
				$"Price: {armor.Price} coins\n" +
				$"Weight: {armor.Weight}\n" +
				(string.IsNullOrEmpty(bonusesStr) ? "" : $"Bonuses: {bonusesStr}");
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

		// Recreate the item so buying multiple copies creates new distinct instances in inventory
		Equipment purchasedItem = ItemFactory.CreateEquipment(_selectedItem.Name);

		_selectedCharacter.AddEquipmentToInventory(purchasedItem);
		_coinLabel.Text = $"Coins: {GameManager.Instance.GetCoinCount()}";
		_detailsLabel.Text = $"Purchased: {_selectedItem.Name}";
	}

	private void OnClosePressed()
	{
		GetParent()?.QueueFree();
	}
}
