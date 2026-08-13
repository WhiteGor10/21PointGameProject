using Godot;
using System;

public partial class BuyCard : Control
{
	[Export]
	public Button RefreshButton;
	[Export]
	public Label MoneyLabel;
	[Export]
	public Label[] Prices;
	[Export]
	public HBoxContainer CardParent;
	[Export]
	public PackedScene CardPrefab;
	[Export]
	public PackedScene CardStoragePrefab;

	public PlayerCardStorage playerCardStorage;
	public Card[] Cards = new Card[0];
	public CardData[] cardDatas;
	public int[] prices;
	public int SelectedCardId = -1;
	private int RefreshPrice;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		prices = new int[3];
		Refresh();
		RefreshPrice =  4 * AutoLoad.self.diffculty *( AutoLoad.self.diffculty - 2 ) + 5;
		RefreshButton.Text = "刷新($" + RefreshPrice + ")";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		CheckIfBuy();
		FinishBuyProcess();
	}
	public void FinishBuyProcess()
	{
		if (AutoLoad.self.SelectedCardID != -1)
		{
			AutoLoad.self.Money -= prices[SelectedCardId];
			AutoLoad.self.PlayerCardStorage = Tool.DeleteElementFromArray(AutoLoad.self.SelectedCardID, AutoLoad.self.PlayerCardStorage );
			AutoLoad.self.PlayerCardStorage = Tool.AddElementToArray(AutoLoad.self.PlayerCardStorage, cardDatas[SelectedCardId]);
			GD.Print("CardValue : " + cardDatas[SelectedCardId].CardValue);
			Refresh();
			SelectedCardId = -1;
			AutoLoad.self.SelectedCardID = -1;
		}
	}
	public void CheckIfBuy()
	{
		if (Input.IsActionJustReleased("Click") )
		{
			for (int i = 0; i < 3; i++)
			{
				if (Cards[i].IsHover)
				{
					if (AutoLoad.self.Money >= prices[i])
					{
						SelectedCardId = i;
						AutoLoad.self.CanCardHover = true;
						playerCardStorage = CardStoragePrefab.Instantiate<PlayerCardStorage>();
						GetTree().CurrentScene.AddChild(playerCardStorage);
					}
				}
			}
		}

	}
	public void Refresh()
	{
		Random random = new Random();
		foreach (Card card in Cards)
		{
			Cards = Tool.DeleteElementFromArray(Cards, card);
			card.QueueFree();
		}
		cardDatas = new CardData[3];
		for (int i = 0; i < 3; i++)
		{
			GenACards(i);
			int BasePrice = 3 + 6 * AutoLoad.self.diffculty * (AutoLoad.self.diffculty - 1) ;
			prices[i] = (int)(random.Next(5, 16) / 10f * BasePrice);
			Prices[i].Text = "$" + prices[i];
		}
		MoneyLabel.Text = "$" + AutoLoad.self.Money;
	}
	public void GenACards(int id)
	{
		Random random = new Random();
		int p = random.Next(0, AllCardDatas.self.cardDatas.Length + 13 * ( 5 - AutoLoad.self.diffculty));        //+13 for Normal cards
		if (p >= AllCardDatas.self.cardDatas.Length)        //Normal Cards
		{
			p = random.Next(1, 14);
			CardData cardData = new CardData(p);        //p is [1,13]
			cardDatas[id] = cardData;
			AddACard(cardData, CardParent);
		}
		else
		{
			CardData CopyCard = new CardData(AllCardDatas.self.cardDatas[p], true);
			if (CopyCard.IsRandomCardValue)
			{
				CopyCard.CardValue = random.Next(1, 11);
				if (CopyCard.CardValue == 1)
				{
					CopyCard.IsAce = true;
				}
				CopyCard.IsRandomCardValue = false;
			}
			cardDatas[id] = CopyCard;
			AddACard(CopyCard, CardParent);

		}
	}
	public void AddACard(CardData cardData, Control parent)
	{
		Card card = CardPrefab.Instantiate<Card>();

		card.SetCardValue(cardData);
		card.CanHover = true;
		parent.AddChild(card);
		Cards = Tool.AddElementToArray(Cards, card);
	}

	public void OnFinish()
	{
		SelectedCardId = -1;
		GetTree().ChangeSceneToFile("res://scenes/ExploreScene.tscn");
	}
	public void OnRefresh()
	{
		if (AutoLoad.self.Money >= RefreshPrice)
		{
			AutoLoad.self.Money -= RefreshPrice;
			Refresh();
		}
	}
}
