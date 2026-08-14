using Godot;
using System;

public partial class WinScene : Control
{
	[Export]
	public VideoStreamPlayer Out;
	[Export]
	public VideoStreamPlayer Down;
	[Export]
	public PanelContainer EndPanel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Down.Visible = false;
		Out.Visible = true;
		EndPanel.Visible = false;
		Out.Play();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
	public void Back()
	{
		GetTree().ChangeSceneToFile("res://scenes/MainScene.tscn");
	}
	public void OnDownFinish()
	{
		EndPanel.Visible = true;
	}
	public void OnOutFinish()
	{
		Down.Visible = true;
		Out.Visible = false;
		Down.Play();
	}
}
