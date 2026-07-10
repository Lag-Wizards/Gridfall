using Godot;
using Gridfall.Domain.Enemies;
using Gridfall.Domain;

namespace Gridfall.Services;

public partial class EnemyStatSheet : Control
{
	private Label _titleLabel;
	private Label _levelLabel;
	private Label _hpLabel;
	private Label _attackLabel;
	private Label _defenseLabel;
	private Label _speedLabel;
	private Label _skillLabel;
	private Label _constitutionLabel;
	private Label _resistanceLabel;
	private Label _luckLabel;
	private Label _weaponLabel;
	private Button _closeButton;

	public override void _Ready()
	{
		Position = new Vector2(28, 28);
		Size = new Vector2(300, 320);

		var panel = new Panel
		{
			Size = new Vector2(300, 320),
			CustomMinimumSize = new Vector2(300, 320),
			Theme = new Theme()
		};
		AddChild(panel);

		var vbox = CreateVBox();
		vbox.SizeFlagsHorizontal = Control.SizeFlags.Fill;
		vbox.SizeFlagsVertical = Control.SizeFlags.Fill;
		vbox.CustomMinimumSize = new Vector2(300, 320);
		panel.AddChild(vbox);
	}

	private VBoxContainer CreateVBox()
	{
		var vbox = new VBoxContainer();
		vbox.CustomMinimumSize = new Vector2(280, 300);

		_titleLabel = new Label { Text = "ENEMY STAT SHEET", HorizontalAlignment = HorizontalAlignment.Center };
		vbox.AddChild(_titleLabel);

		_levelLabel = CreateStatLabel();
		_hpLabel = CreateStatLabel();
		_attackLabel = CreateStatLabel();
		_defenseLabel = CreateStatLabel();
		_speedLabel = CreateStatLabel();
		_skillLabel = CreateStatLabel();
		_constitutionLabel = CreateStatLabel();
		_resistanceLabel = CreateStatLabel();
		_luckLabel = CreateStatLabel();
		_weaponLabel = CreateStatLabel();

		vbox.AddChild(_levelLabel);
		vbox.AddChild(_hpLabel);
		vbox.AddChild(_attackLabel);
		vbox.AddChild(_defenseLabel);
		vbox.AddChild(_speedLabel);
		vbox.AddChild(_skillLabel);
		vbox.AddChild(_constitutionLabel);
		vbox.AddChild(_resistanceLabel);
		vbox.AddChild(_luckLabel);
		vbox.AddChild(_weaponLabel);

		_closeButton = new Button { Text = "Close", SizeFlagsHorizontal = Control.SizeFlags.Fill }; 
		_closeButton.Pressed += () => GetParent()?.QueueFree();
		vbox.AddChild(_closeButton);

		return vbox;
	}

	private Label CreateStatLabel()
	{
		return new Label { Text = "", HorizontalAlignment = HorizontalAlignment.Left };
	}

	public void SetEnemy(EnemyBase enemy)
	{
		if (enemy == null)
			return;

		_titleLabel.Text = $"{enemy.UnitName} L{enemy.Level}";
		_levelLabel.Text = $"Level: {enemy.Level}";
		_hpLabel.Text = $"HP: {enemy.Health}/{enemy.MaxHealth}";
		_attackLabel.Text = $"Attack: {enemy.Strength}";
		_defenseLabel.Text = $"Defense: {enemy.Defense}";
		_speedLabel.Text = $"Speed: {enemy.Speed}";
		_skillLabel.Text = $"Skill: {enemy.Skill}";
		_constitutionLabel.Text = $"Constitution: {enemy.Constitution}";
		_resistanceLabel.Text = $"Resistance: {enemy.Resistance}";
		_luckLabel.Text = $"Luck: {enemy.Luck}";

		_weaponLabel.Text = enemy.EquippedWeapon != null
			? $"Weapon: {enemy.EquippedWeapon.Name} (DMG {enemy.EquippedWeapon.Damage}, RNG {enemy.EquippedWeapon.Range})"
			: "Weapon: None";
	}
}
