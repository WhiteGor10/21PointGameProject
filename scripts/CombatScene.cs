using Godot;
using System;
using System.Threading.Tasks;

public partial class CombatScene : Control
{
	[Export]
	public HBoxContainer OpponentCardContainer;
	[Export]
	public HBoxContainer PlayerCardContainer;
	[Export]
	public Label OpponentTotalValueLabel;
	[Export]
	public Label PlayerTotalValueLabel;
	[Export]
	public Button GetCardButton;
	[Export]
	public Button StopButton;
	[Export]
	public PackedScene CardPrefab;
	[Export]
	public PanelContainer WinLosePanel;
	[Export]
	public Label WinLoseLabel;


	public CardData[] OpponentCardStorge = new CardData[0];
	public CardData[] PlayerCardStorage = new CardData[0];
	public Card[] OpponentCards;
	public Card[] PlayerCards;
	private bool IsOpponentStop = false;
	private int EndStatus = -1;         // -1: not end, 0 lose, 1 draw, 2 win
	private Random random = new Random();
	public override void _Ready()
	{
		InjectCardStorage();
		OpponentCardStorge = AddBasicCardStorage(OpponentCardStorge);
		PlayerCardStorage = AddBasicCardStorage(PlayerCardStorage);

		GameStart();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public async void GameStart()
	{
		WinLosePanel.Visible = false;
		OpponentCards = new Card[0];
		PlayerCards = new Card[0];
		await OpponentDrawCard();
		await PlayerDrawCard();
		updateUI();
	}
	public void InjectCardStorage()         //should be from some autoload script, but just test for now
	{
		CardData[] TempCardDatas = new CardData[0];       //for test
		for (int i = 0; i < 220; i++)        //temp add 50 Devil, -ve card for test
		{
			TempCardDatas = Tool.AddElementToArray(TempCardDatas, AllCardDatas.self.cardDatas[random.Next(1, 25)]);
			//TempCardDatas = Tool.AddElementToArray(TempCardDatas, new CardData(-1 * random.Next(1,11)));
		}

		foreach (CardData cardData in TempCardDatas)
		{
			CardData CopyCard = new CardData(cardData);
			if (cardData.SpecialFunctionIndex != CardData.SpecialFunction.None)
			{
				if (cardData.IsRandomCardValue)
				{
					CopyCard.CardValue = random.Next(1, 11);
					if (CopyCard.CardValue == 1)
					{
						CopyCard.IsAce = true;
					}
				}
			}
			PlayerCardStorage = Tool.AddElementToArray(PlayerCardStorage, CopyCard);
		}
	}
	public async void OnPressStop()
	{
		GetCardButton.Disabled = true;
		StopButton.Disabled = true;

		while (!IsOpponentStop)
		{
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
			await OpponentAction();
			updateUI();
		}

		if (EndStatus == -1)     //Both player and opponent Cards value must be <= 21
		{
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
			int playerValue = TotalValue(PlayerCards);
			int opponentValue = TotalValue(OpponentCards);
			if (playerValue > opponentValue)
			{
				ShowWinLosePanel("你赢了！");
				EndStatus = 2;
			}
			else if (playerValue < opponentValue)
			{
				ShowWinLosePanel("你输了！");
				EndStatus = 0;
			}
			else
			{
				ShowWinLosePanel("平局！");
				EndStatus = 1;
			}
		}
	}
	public async void OnPressGetCard()
	{

		GetCardButton.Disabled = true;
		StopButton.Disabled = true;

		await PlayerDrawCard();

		if (TotalValue(PlayerCards) > 21)
		{
			updateUI();
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
			ShowWinLosePanel("你输了！");
			IsOpponentStop = true;
			EndStatus = 0;
		}

		if (!IsOpponentStop)
		{
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
			await OpponentAction();
		}

		GetCardButton.Disabled = false;
		StopButton.Disabled = false;
		updateUI();
	}
	public async Task OpponentAction()
	{
		int AffordableDifference = 5;   //later may differ
		int SelfTotalValue = TotalValue(OpponentCards);
		if (SelfTotalValue >= 21 - AffordableDifference && TotalValue(OpponentCards) > TotalValue(PlayerCards))
		{
			IsOpponentStop = true;
		}
		else
		{
			await OpponentDrawCard();
			if (TotalValue(OpponentCards) > 21)
			{
				updateUI();
				await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
				ShowWinLosePanel("你赢了！");
				IsOpponentStop = true;
				EndStatus = 2;
			}
		}
	}
	public async Task PlayerDrawCard()
	{
		int PyId = random.Next(0, PlayerCardStorage.Length);
		PlayerCards = Tool.AddElementToArray(PlayerCards, AddACard(PlayerCardStorage[PyId], PlayerCardContainer, PlayerCards.Length + 1));
		PlayerCardStorage = Tool.DeleteElementFromArray(PyId, PlayerCardStorage);
		await ActsSpecialActionsAfterDraw(true);
	}
	public async Task OpponentDrawCard()
	{
		int OpId = random.Next(0, OpponentCardStorge.Length);
		OpponentCards = Tool.AddElementToArray(OpponentCards, AddACard(OpponentCardStorge[OpId], OpponentCardContainer, OpponentCards.Length + 1));
		OpponentCardStorge = Tool.DeleteElementFromArray(OpId, OpponentCardStorge);
		await ActsSpecialActionsAfterDraw(false);
	}
	public async Task ActsSpecialActionsAfterDraw(bool IsPlayer)
	{
		Card[] Array;
		if (IsPlayer)
		{
			Array = PlayerCards;
		}
		else
		{
			Array = OpponentCards;
		}
		int Count = Array.Length;
		if (Count > 0)          //Affect All
		{
			if (Array[Count - 1].specialFunction == CardData.SpecialFunction.AffectAll)
			{
				for (int i = 0; i < Count; i++)
				{
					await Array[i].AnimateValueChange(Array[Count - 1].SpecialFunctionValue);
				}
			}
		}
		await ToSignal(GetTree().CreateTimer(0.1f), SceneTreeTimer.SignalName.Timeout);
		if (Count > 1)         //AffectRight, left and neighbor
		{
			if (Array[Count - 2].specialFunction == CardData.SpecialFunction.AffectRight || Array[Count - 2].specialFunction == CardData.SpecialFunction.AffectNeighbor)
			{
				await Array[Count - 1].AnimateValueChange(Array[Count - 2].SpecialFunctionValue);
			}
			if (Array[Count - 1].specialFunction == CardData.SpecialFunction.AffectLeft || Array[Count - 1].specialFunction == CardData.SpecialFunction.AffectNeighbor)
			{
				await Array[Count - 2].AnimateValueChange(Array[Count - 1].SpecialFunctionValue);
			}
		}
	}

	public int TotalValue(Card[] cards)
	{
		int TotalValue = 0;
		int NumOfAces = 0, NumOfDevil = 0;
		foreach (Card card in cards)
		{
			TotalValue += card.CardValue;
			if (card.IsAce)
			{
				NumOfAces++;
			}
			else if (card.specialFunction == CardData.SpecialFunction.Devil)
			{
				TotalValue += 10;
				NumOfDevil++;
			}
		}
		if (TotalValue > 21 && (NumOfAces > 0 || NumOfDevil > 0))
		{
			while (TotalValue > 21 && NumOfAces > 0)
			{
				TotalValue -= 10;
				NumOfAces--;
			}
			while (TotalValue > 21 && NumOfDevil > 0)
			{
				int DevilValue = 10;        //initial = 10
				while (TotalValue > 21 && DevilValue > 1)
				{
					TotalValue -= 1;
					DevilValue -= 1;
				}
				NumOfDevil--;
			}
		}

		return TotalValue;
	}
	public void updateUI()
	{
		OpponentTotalValueLabel.Text = TotalValue(OpponentCards).ToString();
		PlayerTotalValueLabel.Text = TotalValue(PlayerCards).ToString();
	}
	public void ShowWinLosePanel(string result)
	{
		WinLosePanel.Visible = true;
		WinLoseLabel.Text = result;
	}
	public void OnPressContinue()       //Exit the Scene
	{
		GetTree().ChangeSceneToFile("res://scenes/MainScene.tscn");
	}
	public Card AddACard(CardData cardData, HBoxContainer parent, int NumOfCard)    //NumOfCardinclude the card to be added
	{
		Card card = CardPrefab.Instantiate<Card>();
		card.SetCardValue(cardData);
		parent.AddChild(card);

		if (NumOfCard >= 4)
		{
			float sep = (620 - (NumOfCard * card.Size.X)) / (NumOfCard - 1);
			sep = Mathf.FloorToInt(sep);
			parent.AddThemeConstantOverride("separation", (int)sep);
		}
		return card;
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
