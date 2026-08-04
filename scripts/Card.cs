using Godot;
using System;

public partial class Card : ColorRect
{
	[Export]
	public Label ValueLabel1;
	[Export]
	public Label ValueLabel2;
	[Export]
	public Label DescriptionLabel;
	[Export]
	public Sprite2D SpecialIcon;
	public int CardValue;			// 0 is Devil
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void SetCardValue(CardData cardData)
	{
		DescriptionLabel.Text = cardData.CardDescription;
		this.SpecialIcon.Texture = cardData.CardTexture;
		int value = cardData.CardValue;

		string valueString;
		CardValue = value;
		
		if (value == 1)
		{
			valueString = "A";
			CardValue = 11;
		}
		else if (value == 11)
		{
			valueString = "J";
			CardValue = 10;
		}
		else if (value == 12)
		{
			valueString = "Q";
			CardValue = 10;
		}
		else if (value == 13)
		{
			valueString = "K";
			CardValue = 10;
		}
		else
		{
			valueString = value.ToString();
		}
		if (cardData.SpecialFunctionIndex == CardData.SpecialFunction.Devil)
		{
			valueString = "?";
		}
		ValueLabel1.Text = valueString;
		ValueLabel2.Text = valueString;
	}
}
