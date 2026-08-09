using Godot;
using System;

public partial class AutoLoad : Node
{

	public int BetValue;
	public Vector2 BetMaxMin;
	public bool CanCardHover;       //For PlayerStorage only
	public string ReturnScene;

	public CardData[] PlayerCardStorage;
	public int Money;

	public int SelectedCardID;			//ID in PlayerCardStorage
	public static AutoLoad self;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		self = this;
		//test
		DefaultTheCards();
		BetMaxMin = new Vector2(0, 10);
		Money = 5;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void DefaultTheCards()
	{
		PlayerCardStorage = new CardData[0];
		PlayerCardStorage = AddBasicCardStorage(PlayerCardStorage);
	}

	public CardData[] AddBasicCardStorage(CardData[] storage)
	{
		for (int i = 1; i <= 13; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				storage = Tool.AddElementToArray(storage, new CardData(i));

			}
		}
		return storage;
	}
}
