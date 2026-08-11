using Godot;
using System;

public partial class CardData : Node
{
	[Export]
	public string CardName;
	[Export]
	public string CardDescription;
	[Export]
	public Texture2D CardTexture = null;
	[Export]
	public int CardValue;			//0 is Devil
	[Export]
	public bool IsRandomCardValue = false;
	[Export]
	public SpecialFunction SpecialFunctionIndex = SpecialFunction.None;           //-1 no special, ...
	[Export]
	public int SpecialFunctionValue;
	public bool IsAce = false;

	public enum SpecialFunction
	{
		None = -1,
		Devil,
		AffectRight, AffectLeft, AffectNeighbor, AffectAll,
		Absolute, UpperBounds, SelfChange, Assimilation,
		Delete, summon, derive, Nirvana, Deprave, Chain,
		ConvertOnDraw, ConvertOnDelete


	}
	public CardData(int value)      //Basic card constructor
	{
		CardName = value.ToString();
		CardValue = value;
		SpecialFunctionIndex = SpecialFunction.None;
		if (value == 1)
		{
			IsAce = true;
		}
	}
	public CardData(int value, SpecialFunction specialFunction, int specialFunctionValue)     
	{
		CardName = value.ToString();
		CardValue = value;
		this.SpecialFunctionIndex = specialFunction;
		this.SpecialFunctionValue = specialFunctionValue;
	}
	public CardData(CardData cardData, bool Isfollowrandom)      //Copy Consturtor
	{
		CardName = cardData.CardName;
		CardDescription = cardData.CardDescription;
		CardTexture = cardData.CardTexture;
		CardValue = cardData.CardValue;
		
		IsRandomCardValue = false;
		if (Isfollowrandom)
		{
			IsRandomCardValue = cardData.IsRandomCardValue;
		}

		SpecialFunctionIndex = cardData.SpecialFunctionIndex;
		SpecialFunctionValue = cardData.SpecialFunctionValue;
		IsAce = cardData.IsAce;
	}
	public CardData()
	{

	}
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
