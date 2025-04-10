using System.Net;
using System.Net.Sockets;
using Spectre.Console;

namespace MonsterMaze
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Are you connecting as the client or the server?")
                .AddChoices(new[]
                {
                    "Client",
                    "Server"
                }));

            if (choice == "Server")
            {
                Server.Start();
            }
            else 
            {
                Client.Start();
            }
        }
    }
}
