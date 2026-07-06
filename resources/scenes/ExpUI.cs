using Godot;
using System;
using Gridfall.Domain;
using Gridfall.Characters.Domain;

public partial class ExpUI : Control
{
	private ProgressBar ExpBar;
	private CharacterBase selectedCharacter;
	
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
	
	// Changes the value of Exp Bar
	private void UpdateCharacter(int exp)
	{
		ExpBar.Value = exp;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (selectedCharacter != null)
		{
			selectedCharacter.ExpChanged -= UpdateCharacter;
		}
	}
}
