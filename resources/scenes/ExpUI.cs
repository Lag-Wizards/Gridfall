using Godot;
using System;
using Gridfall.Characters.Domain;

public partial class ExpUI : Control
{
	private ProgressBar ExpBar;
	private CharacterBase selectedCharacter;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ExpBar = GetNode<ProgressBar>("ColorRect/ExpBar");
	}
	
	public void SetSelectedCharacter(CharacterBase newCharacter)
	{
		if (selectedCharacter != null)
		{
			// removes previous bound character
			selectedCharacter.ExpChanged -= UpdateCharacter;
		}
		// allocates new character
		selectedCharacter = newCharacter;
		
		if (selectedCharacter == null)
			return;

		selectedCharacter.ExpChanged += UpdateCharacter;
		// updates Exp bar to current value
		UpdateCharacter(selectedCharacter.Experience);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	// Changes the value of Exp Bar
	private void UpdateCharacter( int exp)
	{
		ExpBar.Value = exp;
	}
}
