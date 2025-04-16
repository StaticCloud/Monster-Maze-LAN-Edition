using System;
using System.Text.Json;

public readonly struct Coords
{
    public int X { get; init; }
    public int Y { get; init; }

    public Coords(int x, int y)
	{
		X = x; Y = y;
	}

	public string toJSON() => JsonSerializer.Serialize(this);
}
