using System;
using System.Collections.Generic;

public static class Libs
{
    /// <summary>
    /// Parses an inputted command.
    /// </summary>
    /// <param name="input"></param>
    public static void HandleCommand(string input, Dictionary<string, Command> commands)
    {
        string[] parts = input.Trim().Substring(1).Split(' ');

        string commandName = parts[0];

        string[] args = new string[parts.Length - 1];
        if (args.Length > 0)
            args.CopyTo(parts, 1);

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
}

public delegate void Command(string[] args);
