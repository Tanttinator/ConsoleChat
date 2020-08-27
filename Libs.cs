using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

public static class Libs
{

    public const int PORT = 13;

    /// <summary>
    /// Parses an inputted command.
    /// </summary>
    /// <param name="input"></param>
    public static void HandleCommand(string input, Dictionary<string, Command> commands)
    {
        string[] parts = input.Trim().Substring(1).Split(' ');

        string commandName = parts[0];

        string[] args = parts.Skip(1).ToArray();

        Command command;
        if (commands.TryGetValue(commandName.ToLower(), out command))
        {
            command(args);
        }
        else
        {
            Console.WriteLine("Unknown command \"" + commandName + "\"");
        }
    }

    /// <summary>
    /// Send a message to the given stream.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="message"></param>
    public static void SendMessage(NetworkStream stream, string message)
    {
        byte[] bytes = ASCIIEncoding.ASCII.GetBytes(message);
        stream.Write(bytes, 0, bytes.Length);
    }
}

public delegate void Command(string[] args);
