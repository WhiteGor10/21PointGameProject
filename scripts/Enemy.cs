using Godot;
using System;

public partial class Enemy : Node2D
{
	[Export]
	public int characterindex;      //determine the texture and sound
	[Export]
	public int[] CardsIndex;        //index of cardDatas in AllCardDatas
	[Export]
	public int[] NormalCards;
	[Export]
	public Vector2 BetMaxMin;

	public CardData[] GetCardDatas()
	{
		CardData[] cards = new CardData[0];
		if (CardsIndex != null)
		{
			for (int i = 0; i < CardsIndex.Length; i++)
			{
				cards = Tool.AddElementToArray(cards, new CardData(AllCardDatas.self.cardDatas[CardsIndex[i]], true));
			}
		}
		if (NormalCards != null)
		{
			for (int i = 0; i < NormalCards.Length; i++)
			{
				cards = Tool.AddElementToArray(cards, new CardData(NormalCards[i]));
			}
		}

		return cards;
	}
	
}
