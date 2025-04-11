using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

public class Player
{
    [DllImport("user32.dll")]
    private static extern short GetAsyncKeyState(int vkey);

    private Coords Coords { get; set; }

	public Player()
	{
		Coords = new Coords(0, 0);
	}

    private static bool KeyDown(ConsoleKey key)
    {
        return (GetAsyncKeyState((int)key) & 0x8000) != 0;
    }

	public async Task HandleMovements(NetworkStream stream)
	{
        bool keydown = false;

        if (KeyDown(ConsoleKey.UpArrow))
        {
            Coords = new Coords(Coords.X, Coords.Y + 1);
            keydown = true;
        } else if (KeyDown(ConsoleKey.DownArrow))
        {
            Coords = new Coords(Coords.X, Coords.Y - 1);
            keydown = true;
        } else if (KeyDown(ConsoleKey.LeftArrow))
        {
            Coords = new Coords(Coords.X - 1, Coords.Y);
            keydown = true;
        } else if (KeyDown(ConsoleKey.RightArrow))
        {
            Coords = new Coords(Coords.X + 1, Coords.Y);
            keydown = true;
        }

        if (keydown) 
        {
            await TransmitMovements(stream);
        }

        Thread.Sleep(1000);
	}

    public async Task TransmitMovements(NetworkStream stream)
    {
        byte[] messageByte = Encoding.UTF8.GetBytes(Coords.toJSON());

        await stream.WriteAsync(messageByte);
    }
}
