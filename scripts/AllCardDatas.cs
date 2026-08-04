using Godot;
using System;

public partial class AllCardDatas : Node2D
{
	public CardData[] cardDatas;
	public static AllCardDatas self;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		cardDatas = new CardData[0];
		Godot.Collections.Array<Godot.Node> array = this.GetChildren();
		foreach (Node node in array)
		{
			CardData card = (CardData)node;
			cardDatas = Tool.AddElementToArray(cardDatas, card);
		}

		self = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
