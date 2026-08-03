using Godot;
using System;

public partial class Tool : Node
{
	public static T[] AddElementToArray<T>(T[] array, T element)
	{
		T[] newArray = new T[array.Length + 1];
		for (int i = 0; i < array.Length; i++)
		{
			newArray[i] = array[i];
		}
		newArray[array.Length] = element;
		return newArray;
	}
	public static T[] DeleteElementFromArray<T>( int id, T[] array)
	{
		T[] newArray = new T[array.Length - 1];
		for (int i = 0; i < id; i++)
		{
			newArray[i] = array[i];
		}
		for (int i = id + 1; i < array.Length; i++)
		{
			newArray[i - 1] = array[i];
		}
		return newArray;
	}
	public static T[] DeleteElementFromArray<T>(T[] array,  T e)
	{
		int id = Array.IndexOf(array, e);
		if (id == -1)
		{
			GD.PrintErr("Index Out of Bounds !");
			return array;
		}
		T[] newArray = new T[array.Length - 1];
		for (int i = 0; i < id; i++)
		{
			newArray[i] = array[i];
		}
		for (int i = id + 1; i < array.Length; i++)
		{
			newArray[i - 1] = array[i];
		}
		return newArray;
	}


}
