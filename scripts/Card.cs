using Godot;
using System;
using System.Threading.Tasks;

public partial class Card : ColorRect
{
	[Export]
	public Label ValueLabel1;
	[Export]
	public Label ValueLabel2;
	[Export]
	public Label ValueChangeLabel;
	[Export]
	public Label DescriptionLabel;
	[Export]
	public Sprite2D SpecialIcon;
	[Export]
	public AnimationPlayer animationPlayer;

	public CardData.SpecialFunction specialFunction;
	public int SpecialFunctionValue;
	public bool IsAce = false;
	public int CardValue;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public async Task AnimateDeletion()
	{
		animationPlayer.Play("Delete");
		double time = animationPlayer.GetSectionEndTime();
		await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);

	}
	public async Task AnimateValueChange(int valueChange)
	{
		if (IsAce || specialFunction == CardData.SpecialFunction.Devil)
		{
			return;
		}
		ValueChangeLabel.Text = valueChange.ToString();
		if (valueChange > 0)
		{
			ValueChangeLabel.Text = "+" + valueChange.ToString();
		}

		animationPlayer.Play("ValueChange");
		double time = animationPlayer.GetSectionEndTime();
		await ToSignal(GetTree().CreateTimer(time), SceneTreeTimer.SignalName.Timeout);


		CardValue += valueChange;
		string valueString = CardValue.ToString();
		ValueLabel1.Text = valueString;
		ValueLabel2.Text = valueString;
	}
	public void SetCardValue(CardData cardData)
	{
		DescriptionLabel.Text = cardData.CardDescription;
		this.SpecialIcon.Texture = cardData.CardTexture;
		specialFunction = cardData.SpecialFunctionIndex;
		SpecialFunctionValue = cardData.SpecialFunctionValue;
		IsAce = cardData.IsAce;
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
	public void ChangeCardValue(int tovalue)
	{
		int value = tovalue;

		string valueString;
		CardValue = value;

		valueString = value.ToString();

		ValueLabel1.Text = valueString;
		ValueLabel2.Text = valueString;
	}
}
