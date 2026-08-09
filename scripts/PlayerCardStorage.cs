using Godot;
using System;

public partial class PlayerCardStorage : Control
{
	[Export]
	public Label Reminder;
	[Export]
	public HFlowContainer CardsContainer;
	[Export]
	public PackedScene CardPrefab;

	private Card[] cards;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GenAllPlayerCards();
		Reminder.Visible = false;
		if (AutoLoad.self.CanCardHover)
		{
			Reminder.Visible = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ManageConfirm();
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
				GetTree().ChangeSceneToFile(AutoLoad.self.ReturnScene);
				return;
			}
		}
		
	}
}
