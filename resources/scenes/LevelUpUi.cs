using Godot;
using System;
using Gridfall.Domain;

public partial class LevelUpUi : Control
{
	private Label levelLabel;
	private Label hpLabel;
	private Label strLabel;
	private Label spdLabel;
	private Label defLabel;
	private Label skillLabel;
	private Label conLabel;
	private Label resLabel;
	private Label luckLabel;
	private CharacterBase selectedCharacter;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		levelLabel = GetNode<Label>("StatsUI/AttributeValues/Level");
		hpLabel = GetNode<Label>("StatsUI/AttributeValues/HP");
		strLabel = GetNode<Label>("StatsUI/AttributeValues/STR");
		spdLabel = GetNode<Label>("StatsUI/AttributeValues/SPD");
		defLabel = GetNode<Label>("StatsUI/AttributeValues/DEF");
		skillLabel = GetNode<Label>("StatsUI/AttributeValues/SKILL");
		conLabel = GetNode<Label>("StatsUI/AttributeValues/CON");
		luckLabel = GetNode<Label>("StatsUI/AttributeValues/LUCK");
		resLabel = GetNode<Label>("StatsUI/AttributeValues/RES");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	public void SetSelectedCharacter(CharacterBase newCharacter)
	{
		// removes previous character
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
		hpLabel.Text = selectedCharacter.MaxHealth.ToString();
		strLabel.Text = selectedCharacter.Strength.ToString();
		spdLabel.Text = selectedCharacter.Speed.ToString();
		defLabel.Text = selectedCharacter.Defense.ToString();
		skillLabel.Text = selectedCharacter.Skill.ToString();
		luckLabel.Text = selectedCharacter.Luck.ToString();
		resLabel.Text = selectedCharacter.Resistance.ToString();
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
				
			case "SKILL":
				skillLabel.Text = value.ToString();
				break;
				
			case "LUCK":
				luckLabel.Text = value.ToString();
				break;
				
			case "RES":
				resLabel.Text = value.ToString();
				break;
		}
	}
}
