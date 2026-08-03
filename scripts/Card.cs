using Godot;
using System;

public partial class Card : ColorRect
{
	[Export]
	public Label ValueLabel1;
	[Export]
	public Label ValueLabel2;
	[Export]
	public Sprite2D SpecialIcon;
	public int CardValue;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void SetCardValue(int value)
	{
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
		ValueLabel1.Text = valueString;
		ValueLabel2.Text = valueString;
	}
}
