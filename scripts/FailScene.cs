using Godot;
using System;

public partial class FailScene : Control
{
	public override void _Process(double delta)
	{
		if (Input.IsActionJustReleased("Click"))
		{
			OnFinish();
		}
    }

	public void OnFinish()
	{
		GetTree().ChangeSceneToFile("res://scenes/MainScene.tscn");
	}
}
