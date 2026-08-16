using Godot;
using System;

public partial class PlayerCardStorage : Control
{
	[Export]
	public Button SortButton;
	[Export]
	public Label Reminder;
	[Export]
	public HFlowContainer CardsContainer;
	[Export]
	public PackedScene CardPrefab;

	private int State;		//Default 0, 1 is sorted by cardvalue, 2 is sorted by function

	private Card[] cards;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		State = 0;
		GenAllPlayerCards();
		Reminder.Visible = false;
		SortButton.Visible = true;
		if (AutoLoad.self.CanCardHover)
		{
			Reminder.Visible = true;
			SortButton.Visible = false;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (AutoLoad.self.CanCardHover)
		{
			ManageConfirm();
		}

	}
	public void Sort()
	{
		if (State != 1)
		{
			AutoLoad.self.SortAccordingToCardValueThenSpecialFunction();
			State = 1;
		}
		else
		{
			AutoLoad.self.SortAccordingToSpecialFunctionThenCardValue();
			State = 2;
		}

		foreach (Card card in cards)
		{
			cards = Tool.DeleteElementFromArray(cards, card);
			card.QueueFree();
		}
		GenAllPlayerCards();
	}
	public void GenAllPlayerCards()
	{
		cards = new Card[0];
		for (int i = 0; i < AutoLoad.self.PlayerCardStorage.Length; i++)
		{
			AddACard(AutoLoad.self.PlayerCardStorage[i], CardsContainer, AutoLoad.self.CanCardHover, i);
		}
	}
	public void AddACard(CardData cardData, Control parent, bool CanHover, int id)
	{
		Card card = CardPrefab.Instantiate<Card>();

		card.SetCardValue(cardData);
		card.CanHover = CanHover;
		card.index = id;
		cards = Tool.AddElementToArray(cards, card);
		parent.AddChild(card);
	}
	
	public void OnReturn()
	{
		if (AutoLoad.self.CanCardHover)
		{
			QueueFree();
			return;
		}
		AutoLoad.self.SelectedCardID = -1;
		GetTree().ChangeSceneToFile(AutoLoad.self.ReturnScene);
	}
	public void ManageConfirm()
	{
		if (!Input.IsActionJustReleased("Click"))
		{
			return;
		}
		foreach (Card card in cards)
		{
			if (card.IsHover)
			{
				AutoLoad.self.SelectedCardID = card.index;
				QueueFree();
				return;
			}
		}
		
	}
}
