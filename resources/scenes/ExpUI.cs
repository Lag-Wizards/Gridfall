using Godot;
using System;
using Gridfall.Domain;
using Gridfall.Characters.Domain;

public partial class ExpUI : Control
{
	private ProgressBar _expBar;
	private CharacterBase selectedCharacter;
	private ProgressBar ExpBar => _expBar ??= GetNodeOrNull<ProgressBar>("ColorRect/ExpBar");
	public override void _Ready()
	{
		_expBar = GetNode<ProgressBar>("ColorRect/ExpBar");
		RefreshBar();
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
		RefreshBar();
	}
	
	// Changes the value of Exp Bar
	private void UpdateCharacter(int exp)
	{
		if (ExpBar == null)
		{
			return;
		}
		RefreshBar();
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		if (selectedCharacter != null)
		{
			selectedCharacter.ExpChanged -= UpdateCharacter;
		}
	}
	
	public void RefreshBar()
	{
		if (selectedCharacter == null || ExpBar == null)
			return;

		ExpBar.MaxValue = 100; 

		ExpBar.Value = selectedCharacter.Experience;

	}
}
