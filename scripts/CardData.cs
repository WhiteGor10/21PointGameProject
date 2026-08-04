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

	public enum SpecialFunction
	{
		None = -1,
		Devil,
		AffectRight, AffectLeft, AffectNeighbor, AffectAll,


	}
	public CardData(int value)      //Basic card constructor
	{
		CardName = value.ToString();
		CardValue = value;
		SpecialFunctionIndex = SpecialFunction.None;
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
