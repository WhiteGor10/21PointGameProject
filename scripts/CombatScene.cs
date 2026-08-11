using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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
	public WinLosePanel WinLosePanel;
	[Export]
	public Label WinLoseLabel;
	[Export]
	public Button SpeedButton;
	[Export]
	public TextureRect OpponentTexture;


	public CardData[] OpponentCardStorge = new CardData[0];
	public CardData[] PlayerCardStorage = new CardData[0];
	public Card[] OpponentCards;
	public Card[] PlayerCards;
	private bool IsOpponentStop = false;
	private int EndStatus = -1;         // -1: not end, 0 lose, 1 draw, 2 win
	private int AnimationSpeed = 1;
	private Random random = new Random();

	public override void _Ready()
	{
		InjectCardStorage();
		InjectEnemyCardStorage();
		ImportOpponentDetails();

		GameStart();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public async void GameStart()
	{
		LoadAnimationSpeed();
		GetCardButton.Disabled = true;
		StopButton.Disabled = true;
		WinLosePanel.Visible = false;
		OpponentCards = new Card[0];
		PlayerCards = new Card[0];
		await OpponentDrawCard();
		await PlayerDrawCard();
		updateUI();
		GetCardButton.Disabled = false;
		StopButton.Disabled = false;
	}
	public void LoadAnimationSpeed()
	{
		AnimationSpeed = AutoLoad.self.SavedSpeed - 1;
		
		OnPressChangeSpeed();
	}
	public void ImportOpponentDetails()
	{
		OpponentTexture.Texture = AllEnemy.self.OpponentTextures[AutoLoad.self.enemy.characterindex];
		switch (AutoLoad.self.enemy.characterindex)
		{
			case 0:
				SoundManager.self.PlayList = ConstantSaver.AudioFD;
				SoundManager.self.LoseList = ConstantSaver.AudioFDL;
				break;
			case 1:
				SoundManager.self.PlayList = ConstantSaver.AudioHnery;
				SoundManager.self.LoseList = ConstantSaver.AudioHneryL;
				break;
			case 2:
				SoundManager.self.PlayList = ConstantSaver.AudioJonSnow;
				SoundManager.self.LoseList = ConstantSaver.AudioJonSnowL;
				break;
			case 3:
				SoundManager.self.PlayList = ConstantSaver.AudioVertin;
				SoundManager.self.LoseList = ConstantSaver.AudioVertinL;
				break;
			case 4:
				SoundManager.self.PlayList = ConstantSaver.AudioDoctorStrange;
				SoundManager.self.LoseList = ConstantSaver.AudioDoctorStrangeL;
				break;
			default:
				SoundManager.self.PlayList = ConstantSaver.AudioFD;
				SoundManager.self.LoseList = ConstantSaver.AudioFDL;
				break;
		}
	}
	public void InjectEnemyCardStorage()
	{
		CardData[] TempCardDatas = AutoLoad.self.enemy.GetCardDatas();

		foreach (CardData cardData in TempCardDatas)
		{
			CardData CopyCard = new CardData(cardData, false);
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
			OpponentCardStorge = Tool.AddElementToArray(OpponentCardStorge, CopyCard);
		}
	}
	public void InjectCardStorage()         //should be from some autoload script, but just test for now
	{
		CardData[] TempCardDatas = AutoLoad.self.PlayerCardStorage;

		foreach (CardData cardData in TempCardDatas)
		{
			CardData CopyCard = new CardData(cardData, false);
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
				EndStatus = 2;
				ShowWinLosePanel("你赢了！");

				SoundManager.self.RandomPlayLoseSound();
			}
			else if (playerValue < opponentValue)
			{
				EndStatus = 0;
				ShowWinLosePanel("你输了！");
				SoundManager.self.RandomPlaySound();
			}
			else
			{
				EndStatus = 1;
				ShowWinLosePanel("平局！");

			}
		}
	}
	public async void OnPressGetCard()
	{

		GetCardButton.Disabled = true;
		StopButton.Disabled = true;

		await PlayerDrawCard();
		updateUI();

		if (TotalValue(PlayerCards) > 21)
		{
			updateUI();
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
			EndStatus = 0;
			ShowWinLosePanel("你输了！");
			IsOpponentStop = true;

			SoundManager.self.RandomPlaySound();
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
		if ((SelfTotalValue >= 21 - AffordableDifference && TotalValue(OpponentCards) > TotalValue(PlayerCards))
			|| TotalValue(OpponentCards) == 21)
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
				EndStatus = 2;
				ShowWinLosePanel("你赢了！");
				IsOpponentStop = true;
				
			}
		}
	}
	public async Task PlayerDrawCard()
	{
		if (PlayerCardStorage.Length == 0)
		{
			updateUI();
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
			EndStatus = 0;
			ShowWinLosePanel("你没牌了，你输了！");
			IsOpponentStop = true;

			SoundManager.self.RandomPlaySound();
		}
		int PyId = random.Next(0, PlayerCardStorage.Length);
		PlayerCards = Tool.AddElementToArray(PlayerCards, AddACard(PlayerCardStorage[PyId], PlayerCardContainer, PlayerCards.Length + 1));
		PlayerCardStorage = Tool.DeleteElementFromArray(PyId, PlayerCardStorage);
		await ActsSpecialActionsAfterDraw(true);
	}
	public async Task OpponentDrawCard()
	{
		if (OpponentCardStorge.Length == 0)
		{
			updateUI();
			await ToSignal(GetTree().CreateTimer(0.5f), SceneTreeTimer.SignalName.Timeout);
			EndStatus = 2;
			ShowWinLosePanel("他没牌了，你赢了！");
			IsOpponentStop = true;

			SoundManager.self.RandomPlayLoseSound();
		}
		else
		{
			SoundManager.self.RandomPlaySound();
		}
		int OpId = random.Next(0, OpponentCardStorge.Length);
		OpponentCards = Tool.AddElementToArray(OpponentCards, AddACard(OpponentCardStorge[OpId], OpponentCardContainer, OpponentCards.Length + 1));
		OpponentCardStorge = Tool.DeleteElementFromArray(OpId, OpponentCardStorge);
		await ActsSpecialActionsAfterDraw(false);
	}
	public async Task ActsSpecialActionsAfterDraw(bool IsPlayer)
	{
		Card[] Array;
		HBoxContainer parent;
		if (IsPlayer)
		{
			Array = PlayerCards;
			parent = PlayerCardContainer;
		}
		else
		{
			Array = OpponentCards;
			parent = OpponentCardContainer;
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
			else if (Array[Count - 1].specialFunction == CardData.SpecialFunction.Assimilation)
			{
				for (int i = 0; i < Count; i++)
				{
					Array[i].ChangeCardValue(Array[Count - 1].SpecialFunctionValue);
					Array[i].IsAce = false;
				}
			}
		}
		await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);
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
			if (Array[Count - 1].specialFunction == CardData.SpecialFunction.Delete)		//Will not delete itself
			{
				int target, times = Array[Count - 1].SpecialFunctionValue;
				while (times > 0 && Count > 1)
				{
					int ConvertOnDeleteValue = -1;
					target = random.Next(0, Count - 1);
					await Array[target].AnimateDeletion();
					if (Array[target].specialFunction == CardData.SpecialFunction.Nirvana)
					{
						Array = Tool.AddElementToArray(Array, AddACard(new CardData(1), parent, Count + 1));
					}
					else if (Array[target].specialFunction == CardData.SpecialFunction.Deprave)
					{
						Array = Tool.AddElementToArray(Array, AddACard(AllCardDatas.self.cardDatas[0], parent, Count + 1));        //Add devil
					}
					else if (Array[target].specialFunction == CardData.SpecialFunction.Chain)
					{
						times++;
					}
					else if (Array[target].specialFunction == CardData.SpecialFunction.ConvertOnDelete)
					{
						ConvertOnDeleteValue = Array[target].SpecialFunctionValue;
					}
					Array[target].QueueFree();
					Array = Tool.DeleteElementFromArray(Array, Array[target]);
					Count = Array.Length;
					UpdateSep(200, parent, Count);
					times--;
					if (ConvertOnDeleteValue != -1)
					{
						await ConvertCard(Array, Count, ConvertOnDeleteValue);
					}
				}
			}
			if (Array[Count - 1].specialFunction == CardData.SpecialFunction.ConvertOnDraw)
			{
				await ConvertCard(Array, Count, Array[Count - 1].SpecialFunctionValue);
			}
			await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);
		}

		for (int i = 0; i < Count - 1; i++)
		{
			if (Array[i].specialFunction == CardData.SpecialFunction.SelfChange)
			{
				await Array[i].AnimateValueChange(Array[i].SpecialFunctionValue);
			}
		}
		await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);
		if (Array[Count - 1].specialFunction == CardData.SpecialFunction.summon)
		{
			Array = Tool.AddElementToArray(Array, AddACard(new CardData(-5), parent, Count + 1));
			await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);
			Array = Tool.AddElementToArray(Array, AddACard(AllCardDatas.self.cardDatas[0], parent, Count + 2));        //Add devil
		}
		else if (Array[Count - 1].specialFunction == CardData.SpecialFunction.derive)
		{
			Array = Tool.AddElementToArray(Array, AddACard(new CardData(2), parent, Count + 1));
			await ToSignal(GetTree().CreateTimer(0.05f), SceneTreeTimer.SignalName.Timeout);
			Array = Tool.AddElementToArray(Array, AddACard(new CardData(3), parent, Count + 2));
		}
		Count = Array.Length;
		if (IsPlayer)
		{
			PlayerCards = Array;
		}
		else
		{
			OpponentCards = Array;
		}
	}
	public async Task ConvertCard(Card[] Array, int Count, int tovalue)
	{
		int times = 1;
		List<int> indices = Enumerable.Range(0, Count).ToList();		
		//change order
		for (int i = indices.Count - 1; i > 0; i--)
		{
			int j = random.Next(i + 1);
			(indices[i], indices[j]) = (indices[j], indices[i]);
		}
		
		int actualTimes = Math.Min(times, indices.Count);
		for (int i = 0; i < actualTimes; i++)
		{
			int target = indices[i];
			await Array[target].Convertion(tovalue);
		}
	}

	public int TotalValue(Card[] cards)
	{
		int TotalValue = 0;
		int NumOfAces = 0, NumOfDevil = 0;
		bool ExistAbsolute = false;
		int UpperBound = 999;
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
			else if (card.specialFunction == CardData.SpecialFunction.Absolute)
			{
				ExistAbsolute = true;
			}
			else if (card.specialFunction == CardData.SpecialFunction.UpperBounds)
			{
				UpperBound = Math.Min(UpperBound, card.SpecialFunctionValue);
			}
		}
		GD.Print("Num Of ACe : " + NumOfAces);
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
		if (TotalValue < 0)
		{
			if (ExistAbsolute)
			{
				TotalValue = TotalValue * -1;
			}
		}
		if (TotalValue > UpperBound)
		{
			TotalValue = UpperBound;
		}
		GD.Print("Total value : " + TotalValue);
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
		if (EndStatus == 0)         //Lose
		{
			WinLosePanel.Balance.Text = "-$" + AutoLoad.self.BetValue;
		}
		else if (EndStatus == 1)         //Draw
		{
			WinLosePanel.Balance.Text = "$0";
		}
		else
		{
			WinLosePanel.Balance.Text = "+$" + AutoLoad.self.BetValue;
		}
	}
	public void OnPressContinue()       //Exit the Scene
	{
		if (EndStatus == 0)         //Lose
		{
			//Nothing
		}
		else if (EndStatus == 1)         //Draw
		{
			AutoLoad.self.Money += AutoLoad.self.BetValue;
		}
		else
		{
			AutoLoad.self.Money += AutoLoad.self.BetValue * 2;
		}
		AutoLoad.self.GetRandomEnemy();
		AutoLoad.self.BetValue = 0;
		GetTree().ChangeSceneToFile(AutoLoad.self.ReturnScene);
	}
	public Card AddACard(CardData cardData, HBoxContainer parent, int NumOfCard)    //NumOfCardinclude the card to be added
	{
		Card card = CardPrefab.Instantiate<Card>();
		UpdateSep(200, parent, NumOfCard);

		card.SetCardValue(cardData);
		card.animationPlayer.SpeedScale = AnimationSpeed / 2f;
		parent.AddChild(card);


		return card;
	}
	public void UpdateSep(float CardSize, HBoxContainer parent, int NumOfCard)
	{
		if (NumOfCard >= 4)
		{
			float sep = (620 - (NumOfCard * CardSize)) / (NumOfCard - 1);
			sep = Mathf.FloorToInt(sep);
			parent.AddThemeConstantOverride("separation", (int)sep);
		}
		else
		{
			parent.AddThemeConstantOverride("separation", 10);
		}
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
	public void OnPressChangeSpeed()
	{
		if (AnimationSpeed == 1)
		{
			AnimationSpeed = 2;
		}
		else if (AnimationSpeed == 2)
		{
			AnimationSpeed = 3;
		}
		else
		{
			AnimationSpeed = 1;
		}
		AutoLoad.self.SavedSpeed = AnimationSpeed;
		SpeedButton.Text = AnimationSpeed + ".0X";
		if (PlayerCards == null || OpponentCards == null)
		{
			return;
		}
		foreach (Card card in PlayerCards)
		{
			card.animationPlayer.SpeedScale = AnimationSpeed;
		}
		foreach (Card card in OpponentCards)
		{
			card.animationPlayer.SpeedScale = AnimationSpeed;
		}
	}
}
