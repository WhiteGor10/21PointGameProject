using Godot;
using System;

public partial class ExploreScene : Node2D
{
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
		UpdateUI();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}
	public void UpdateUI()
	{
		BetSlider.Value = (AutoLoad.self.BetValue - AutoLoad.self.BetMaxMin.X) / (AutoLoad.self.BetMaxMin.Y - AutoLoad.self.BetMaxMin.X) * 10;
		BetValueLabel.Text = "$" + BetSlider.Value;
		MoneyLabel.Text = "$" + AutoLoad.self.Money;
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
			BetSlider.Value = AutoLoad.self.Money;
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
			AutoLoad.self.BetValue = BetValue;
		}
	}
	public void StartNextCombat()
	{
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
