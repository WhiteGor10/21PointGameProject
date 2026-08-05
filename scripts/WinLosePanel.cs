using Godot;
using System;

public partial class WinLosePanel : PanelContainer
{
	[Export]
	public PanelContainer MainContent;
	[Export]
	public Button HideButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void HideOrShowMainContent()
	{
		MainContent.Visible = !MainContent.Visible;
		if (MainContent.Visible)
		{
			HideButton.Text = "隐藏";
		}
		else
		{
			HideButton.Text = "展示";
		}
	}
}
