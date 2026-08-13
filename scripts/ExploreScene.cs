using Godot;
using System;

public partial class ExploreScene : Node2D
{
	[Export]
	public TextureRect Opponent;
	[Export]
	public Label MoneyLabel;
	[Export]
	public Label BetValueLabel;
	[Export]
	public HSlider BetSlider;
	public int BetValue;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (AutoLoad.self.Money <= 0)
		{
			GetTree().ChangeSceneToFile("res://scenes/FailScene.tscn");
		}
		ManageDifficulty();
		UpdateUI();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MoneyLabel.Text = "$" + AutoLoad.self.Money;
	}
	public void ManageDifficulty()
	{
		switch (AutoLoad.self.diffculty)
		{
			case 1:
				if (AutoLoad.self.Money >= 150)
				{
					AutoLoad.self.diffculty = 2;
				}
				break;
			case 2:
				if (AutoLoad.self.Money >= 800)
				{
					AutoLoad.self.diffculty = 3;
				}
				break;
			case 3:
				if (AutoLoad.self.Money >= 3000)
				{
					AutoLoad.self.diffculty = 4;
				}
				break;
			case 4:
				if (AutoLoad.self.Money >= 10000)
				{
					AutoLoad.self.diffculty = 5;
				}
				break;
			default:
				break;
		}
	}
	public void UpdateUI()
	{
		BetSlider.Value = (AutoLoad.self.BetValue - AutoLoad.self.BetMaxMin.X) / (AutoLoad.self.BetMaxMin.Y - AutoLoad.self.BetMaxMin.X) * 10;

		if (AutoLoad.self.BetValue <= 0)
		{
			AutoLoad.self.BetValue = (int)AutoLoad.self.BetMaxMin.X;
			if (AutoLoad.self.Money < AutoLoad.self.BetValue)
			{
				AutoLoad.self.BetValue = AutoLoad.self.Money;
			}
			if (AutoLoad.self.BetValue <= 0)
			{
				AutoLoad.self.BetValue = 1;
			}
		}
		BetValueLabel.Text = "$" + AutoLoad.self.BetValue;
		MoneyLabel.Text = "$" + AutoLoad.self.Money;
		Opponent.Texture = AllEnemy.self.OpponentTextures[AutoLoad.self.enemy.characterindex];
	}
	public void BetValueChanged(float value)
	{
		value = (int)(AutoLoad.self.BetMaxMin.X + (AutoLoad.self.BetMaxMin.Y - AutoLoad.self.BetMaxMin.X) / 10 * value);
		if (value <= 0)
		{
			value = 1;
		}
		if (value > AutoLoad.self.Money)
		{
			double MaxBet =  (AutoLoad.self.Money - AutoLoad.self.BetMaxMin.X) / (AutoLoad.self.BetMaxMin.Y - AutoLoad.self.BetMaxMin.X) * 10;
			if (MaxBet <= 0)
			{
				MaxBet = 0;
			}
			BetSlider.Value = MaxBet;
			value = AutoLoad.self.Money;
		}
		BetValueLabel.Text = "$" + value;
	}
	public void DragEnded(bool ValueChanged)
	{
		if (ValueChanged)
		{
			int value = (int)BetSlider.Value;
			BetValue = (int)(AutoLoad.self.BetMaxMin.X + (AutoLoad.self.BetMaxMin.Y - AutoLoad.self.BetMaxMin.X) / 10 * value);
			// Min + step * value,   value is 0-10
			if (BetValue <= 0)
			{
				BetValue = 1;
			}
			GD.Print("Betvalue : " + BetValue);
			AutoLoad.self.BetValue = BetValue;
		}
	}
	public void StartNextCombat()
	{
		if (AutoLoad.self.BetValue <= 0)
		{
			AutoLoad.self.BetValue = 1;
		}
		AutoLoad.self.Money -= AutoLoad.self.BetValue;
		AutoLoad.self.ReturnScene = GetTree().CurrentScene.SceneFilePath;
		GetTree().ChangeSceneToFile("res://scenes/CombatScene.tscn");
	}
	public void GotoCardStorage()
	{
		AutoLoad.self.CanCardHover = false;
		AutoLoad.self.ReturnScene = GetTree().CurrentScene.SceneFilePath;

		GetTree().ChangeSceneToFile("res://Prefabs/PlayerCardStorage.tscn");
	}
}
