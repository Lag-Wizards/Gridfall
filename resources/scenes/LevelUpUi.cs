using Godot;
using System;
using Gridfall.Characters.Domain;

public partial class LevelUpUi : Control
{
	private Label levelLabel;
	private Label hpLabel;
	private Label strLabel;
	private Label spdLabel;
	private Label defLabel;
	private CharacterBase selectedCharacter;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		levelLabel = GetNode<Label>("StatsUI/AttributeValues/Level");
		hpLabel = GetNode<Label>("StatsUI/AttributeValues/HP");
		strLabel = GetNode<Label>("StatsUI/AttributeValues/STR");
		spdLabel = GetNode<Label>("StatsUI/AttributeValues/SPD");
		defLabel = GetNode<Label>("StatsUI/AttributeValues/DEF");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	public void SetSelectedCharacter(CharacterBase newCharacter)
	{
		// removes previous character
		// thoughts on putting this in exittree?
		if (selectedCharacter != null)
		{
			selectedCharacter.StatChanged -= OnStatChanged;
		}

		selectedCharacter = newCharacter;

		if (selectedCharacter == null)
			return;
		// adds new character
		selectedCharacter.StatChanged += OnStatChanged;
		// Updates all stats
		levelLabel.Text = selectedCharacter.Level.ToString();
		hpLabel.Text = selectedCharacter.Health.ToString();
		strLabel.Text = selectedCharacter.Strength.ToString();
		spdLabel.Text = selectedCharacter.Speed.ToString();
		defLabel.Text = selectedCharacter.Defense.ToString();
	}
	private void OnStatChanged(string stat, int value)
	{
		switch (stat)
		{
			case "Level":
				levelLabel.Text = value.ToString();
				break;
			
			case "STR":
				strLabel.Text = value.ToString();
				break;

			case "SPD":
				spdLabel.Text = value.ToString();
				break;

			case "DEF":
				defLabel.Text = value.ToString();
				break;

			case "HP":
				hpLabel.Text = value.ToString();
				break;
		}
	}
}
