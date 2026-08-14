using Godot;
using System;

public partial class MainScene : Node2D
{
	[Export]
	public PanelContainer StartPanel;
	[Export]
	public Label SkillNameLabel;
	[Export]
	public Label SkillDescriptionLabel;
	[Export]
	public string[] SkillNames;
	[Export]
	public string[] SkillDescriptions;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AutoLoad.self.PlayerSkill = 0;
		SetDisplay();
		StartPanel.Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void Exit()
	{
		GetTree().Quit();
	}
	public void Start()
	{
		StartPanel.Visible = true;
	}
	public void StartGame()
	{
		GetTree().ChangeSceneToFile("res://scenes/ExploreScene.tscn");
		AutoLoad.self.diffculty = 1;
		AutoLoad.self.DefaultTheCards();
		AutoLoad.self.Money = 5;
		AutoLoad.self.GetRandomEnemy();
	}
	public void OnPressChangeSkill()
	{
		if (AutoLoad.self.PlayerSkill < SkillNames.Length - 1)
		{
			AutoLoad.self.PlayerSkill++;
		}
		else
		{
			AutoLoad.self.PlayerSkill = 0;
		}
		SetDisplay();
	}
	public void SetDisplay()
	{
		SkillNameLabel.Text = SkillNames[AutoLoad.self.PlayerSkill];
		SkillDescriptionLabel.Text = SkillDescriptions[AutoLoad.self.PlayerSkill];
	}

}
