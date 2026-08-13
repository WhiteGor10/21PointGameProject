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
			//TempCardDatas = Tool.AddElementToArray(TempCardDatas, new CardData(-1 * random.Next(1,11)));
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
	public void SortAccordingtoCardValue()
	{
		QuickSort(PlayerCardStorage);
	}
	public void QuickSort(CardData[] cards)
    {
        if (cards == null || cards.Length <= 1)
            return;
            
        QuickSort(cards, 0, cards.Length - 1);
    }
    
    private void QuickSort(CardData[] cards, int low, int high)
    {
        if (low < high)
        {
            // Partition the array and get the pivot index
            int pivotIndex = Partition(cards, low, high);
            
            // Recursively sort elements before and after partition
            QuickSort(cards, low, pivotIndex - 1);
            QuickSort(cards, pivotIndex + 1, high);
        }
    }
    
    private int Partition(CardData[] cards, int low, int high)
    {
        // Choose the rightmost element as pivot
        int pivotValue = cards[high].CardValue;
        int i = low - 1; // Index of smaller element
        
        for (int j = low; j < high; j++)
        {
            // If current element is less than or equal to pivot
            if (cards[j].CardValue <= pivotValue)
            {
                i++;
                Swap(cards, i, j);
            }
        }
        
        // Place pivot in its correct position
        Swap(cards, i + 1, high);
        return i + 1;
    }
    
    private void Swap(CardData[] cards, int i, int j)
    {
        CardData temp = cards[i];
        cards[i] = cards[j];
        cards[j] = temp;
    }
}
