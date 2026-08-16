using Godot;
using System;

public partial class AutoLoad : Node
{
	public Enemy enemy;

	public int BetValue;
	public Vector2 BetMaxMin;
	public bool CanCardHover;       //For PlayerStorage only

	public int SavedSpeed = 1;
	public string ReturnScene;

	public CardData[] PlayerCardStorage;
	public int Money;

	public int diffculty;           //1-5

	public int SelectedCardID = -1;         //ID in PlayerCardStorage
	public int PlayerSkill = 0;
	public static AutoLoad self;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		self = this;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Debug"))
		{
			Money += 10;
		}
	}
	public void GetRandomEnemy()
	{
		enemy = AllEnemy.self.GetRandomEnemy(diffculty);
		SetEnemyData();
	}
	public void SetEnemyData()      //After having enemy
	{
		BetMaxMin = enemy.BetMaxMin;
	}
	public void DefaultTheCards()
	{
		PlayerCardStorage = new CardData[0];
		PlayerCardStorage = AddBasicCardStorage(PlayerCardStorage);

		for (int i = 0; i < 220; i++)        //temp add 50 Devil, -ve card for test
		{
			//TempCardDatas = Tool.AddElementToArray(TempCardDatas, AllCardDatas.self.cardDatas[random.Next(39, 43)]);
			//TempCardDatas = Tool.AddElementToArray(TempCardDatas, AllCardDatas.self.cardDatas[32]);
			//TempCardDatas = Tool.AddElementToArray(TempCardDatas, AllCardDatas.self.cardDatas[random.Next(36, 39)]);
			//TempCardDatas = Tool.AddElementToArray(TempCardDatas, AllCardDatas.self.cardDatas[38]);
			//PlayerCardStorage = Tool.AddElementToArray(PlayerCardStorage, new CardData(-7));
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

	public void SortAccordingToCardValueThenSpecialFunction()
    {
        QuickSort(PlayerCardStorage, 3, true);
    }
    
    // New method for sorting by both (SpecialFunction first, then CardValue)
    public void SortAccordingToSpecialFunctionThenCardValue()
    {
        QuickSort(PlayerCardStorage, 4, true);
    }
    
    // Modified QuickSort with sort mode parameter
    public void QuickSort(CardData[] cards, int sortMode, bool ascending = true)
    {
        if (cards == null || cards.Length <= 1)
            return;
            
        QuickSort(cards, 0, cards.Length - 1, sortMode, ascending);
    }
    
    private void QuickSort(CardData[] cards, int low, int high, int sortMode, bool ascending)
    {
        if (low < high)
        {
            int pivotIndex = Partition(cards, low, high, sortMode, ascending);
            QuickSort(cards, low, pivotIndex - 1, sortMode, ascending);
            QuickSort(cards, pivotIndex + 1, high, sortMode, ascending);
        }
    }
    
    private int Partition(CardData[] cards, int low, int high, int sortMode, bool ascending)
    {
        // Get the pivot key based on sort mode
        int pivotValue = GetSortKey(cards[high], sortMode);
        int i = low - 1;
        
        for (int j = low; j < high; j++)
        {
            int currentValue = GetSortKey(cards[j], sortMode);
            
            bool shouldSwap = ascending ? 
                currentValue <= pivotValue : 
                currentValue >= pivotValue;
                
            if (shouldSwap)
            {
                i++;
                Swap(cards, i, j);
            }
        }
        
        Swap(cards, i + 1, high);
        return i + 1;
    }

	// Helper method to get the appropriate sort key
	//1: ByCardValue, 2: BySpecialFunction, 3: CardValue first, then SpecialFunction, 4: SpecialFunction first, then CardValue
    private int GetSortKey(CardData card, int sortMode)
	{
		switch (sortMode)
		{
			case 1:
				return card.CardValue;

			case 2:
				return (int)card.SpecialFunctionIndex;

			case 3:
				// CardValue takes priority (multiplied by a large number)
				return card.CardValue * 1000 + ((int)card.SpecialFunctionIndex + 1);

			case 4:
				// SpecialFunction takes priority
				return ((int)card.SpecialFunctionIndex + 1) * 1000 + card.CardValue;

			default:
				return card.CardValue;
		}
	}
    
    private void Swap(CardData[] cards, int i, int j)
    {
        CardData temp = cards[i];
        cards[i] = cards[j];
        cards[j] = temp;
    }
}
