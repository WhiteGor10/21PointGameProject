using Godot;
using System;

public partial class SoundManager : Node
{
	[Export]
	public AudioStreamPlayer SoundPlayer;
	public string[] PlayList = new string[0];
	public string[] LoseList = new string[0];
	public static SoundManager self;
	private Random random = new Random();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		self = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void RandomPlaySound()
	{
		if (PlayList.Length == 0)
		{
			return;
		}
		int id = random.Next(0, PlayList.Length);
		SoundPlayer.Stream = GD.Load<AudioStream>(PlayList[id]);
		SoundPlayer.Play();
	}
	public void RandomPlayLoseSound()
	{
		if (LoseList.Length == 0)
		{
			return;
		}
		int id = random.Next(0, LoseList.Length);
		SoundPlayer.Stream = GD.Load<AudioStream>(LoseList[id]);
		SoundPlayer.Play();
	}
}
