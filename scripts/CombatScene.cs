using Godot;
using System;

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


	public int[] OpponentCardStorge = new int[0];
	public int[] PlayerCardStorage = new int[0];
	public Card[] OpponentCards;
	public Card[] PlayerCards;
	private bool IsOpponentStop = false;
	private Random random = new Random();
	public override void _Ready()
	{
		OpponentCardStorge = AddBasicCardStorage(OpponentCardStorge);
		PlayerCardStorage = AddBasicCardStorage(PlayerCardStorage);

		GameStart();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public async void OnPressStop()
	{
		GetCardButton.Disabled = true;
		StopButton.Disabled = true;

		while (!IsOpponentStop)
		{
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
			OpponentAction();
			updateUI();
		}
	}
	public async void OnPressGetCard()
	{
		PlayerDrawCard();
		GetCardButton.Disabled = true;
		StopButton.Disabled = true;

		if (!IsOpponentStop)
		{
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
			OpponentAction();
		}

		GetCardButton.Disabled = false;
		StopButton.Disabled = false;
		updateUI();
	}
	public void OpponentAction()
	{
		int AffordableDifference = 5;   //later may differ
		int SelfTotalValue = TotalValue(OpponentCards);
		if (SelfTotalValue < 21 - AffordableDifference)
		{
			OpponentDrawCard();
		}
		else
		{
			IsOpponentStop = true;
		}
	}
	public void PlayerDrawCard()
	{
		int PyId = random.Next(0, PlayerCardStorage.Length);
		PlayerCards = Tool.AddElementToArray(PlayerCards, AddACard(PlayerCardStorage[PyId], PlayerCardContainer));
		PlayerCardStorage = Tool.DeleteElementFromArray(PyId, PlayerCardStorage);
	}
	public void OpponentDrawCard()
	{
		int OpId = random.Next(0, OpponentCardStorge.Length);
		OpponentCards = Tool.AddElementToArray(OpponentCards, AddACard(OpponentCardStorge[OpId], OpponentCardContainer));
		OpponentCardStorge = Tool.DeleteElementFromArray(OpId, OpponentCardStorge);
	}
	public void GameStart()
	{
		OpponentCards = new Card[0];
		PlayerCards = new Card[0];
		OpponentDrawCard();
		PlayerDrawCard();
		updateUI();
	}
	public int TotalValue(Card[] cards)
	{
		int TotalValue = 0;
		int NumOfAces = 0;
		foreach (Card card in cards)
		{
			TotalValue += card.CardValue;
			if (card.CardValue == 11)
			{
				NumOfAces++;
			}
		}
		if(TotalValue > 21 && NumOfAces > 0)
		{
			while (TotalValue > 21 && NumOfAces > 0)
			{
				TotalValue -= 10;
				NumOfAces--;
			}
		}

		return TotalValue;
	}
	public void updateUI()
	{
		OpponentTotalValueLabel.Text = TotalValue(OpponentCards).ToString();
		PlayerTotalValueLabel.Text = TotalValue(PlayerCards).ToString();
	}
	public Card AddACard(int value, HBoxContainer parent)
	{
		Card card = CardPrefab.Instantiate<Card>();
		card.SetCardValue(value);
		parent.AddChild(card);
		return card;
	}
	public int[] AddBasicCardStorage(int[] storage)
	{
		for (int i = 1; i <= 13; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				storage = Tool.AddElementToArray(storage, i);
			}
		}
		return storage;
	}
}
