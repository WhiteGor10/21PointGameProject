using Godot;
using System;

public partial class AllEnemy : Node2D
{
	[Export]
	public Node2D Difficulty1Parent;
	[Export]
	public Node2D Difficulty2Parent;
	[Export]
	public Node2D Difficulty3Parent;
	[Export]
	public Node2D Difficulty4Parent;

	public Enemy[] Difficulty1;
	public Enemy[] Difficulty2;
	public Enemy[] Difficulty3;
	public Enemy[] Difficulty4;

	[Export]
	public Texture2D[] OpponentTextures;		//Follow the sounds

	public static AllEnemy self;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		self = this;
		Difficulty1 = GetNodesUnderParent(Difficulty1Parent, Difficulty1);
		Difficulty2 = GetNodesUnderParent(Difficulty2Parent, Difficulty2);
		Difficulty3 = GetNodesUnderParent(Difficulty3Parent, Difficulty3);
		Difficulty4 = GetNodesUnderParent(Difficulty4Parent, Difficulty4);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public Enemy[] GetNodesUnderParent(Node2D Parent, Enemy[] array)
	{
		Godot.Collections.Array<Node> Array = Parent.GetChildren();
		GD.Print(Array.Count);
		array = new Enemy[Array.Count];
		for (int i = 0; i < Array.Count; i++)
		{
			array[i] = (Enemy)Array[i];
			GD.Print("CID : " + array[i].characterindex);
		}
		return array;
	}
	public Enemy GetRandomEnemy(int diffculty)
	{
		Random random = new Random();
		int p = random.Next(1, 101);
		if (diffculty == 1)
		{
			return Difficulty1[random.Next(0, Difficulty1.Length)];
		}
		else if (diffculty == 2)        //20, 80
		{
			if (p <= 20)
			{
				return Difficulty1[random.Next(0, Difficulty1.Length)];
			}
			return Difficulty2[random.Next(0, Difficulty2.Length)];
		}
		else if (diffculty == 3)        //0, 30, 70
		{
			if (p <= 30)
			{
				return Difficulty2[random.Next(0, Difficulty2.Length)];
			}
			return Difficulty3[random.Next(0, Difficulty3.Length)];
		}
		else if (diffculty == 4)        //0, 0, 40, 60
		{
			if (p <= 40)
			{
				return Difficulty3[random.Next(0, Difficulty3.Length)];
			}
			return Difficulty4[random.Next(0, Difficulty4.Length)];
		}
		else            //100
		{
			return Difficulty4[random.Next(0, Difficulty4.Length)];
		}
	}
}
