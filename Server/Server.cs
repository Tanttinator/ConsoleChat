using System;
using System.Collections.Generic;

namespace Server
{
    class Server
    {
        static Dictionary<string, Command> commands;

        static void Main(string[] args)
        {
            Console.WriteLine("CONSOLECHAT DEDICATED SERVER");

            commands = new Dictionary<string, Command>();
            commands.Add("test", new Command(CommandTest));

            while (true)
            {
                Console.Write(">");
                HandleInput(Console.ReadLine());
            }
        }

        /// <summary>
        /// Parses user input and decides what to do with it.
        /// </summary>
        /// <param name="input"></param>
        static void HandleInput(string input)
        {
            if (input.Trim().StartsWith("/"))
            {
                Libs.HandleCommand(input, commands);
            }
            else
            {
                Console.WriteLine(input);
            }
        }

        /// <summary>
        /// Simple test command.
        /// </summary>
        /// <param name="args"></param>
        static void CommandTest(string[] args)
        {
            Console.WriteLine("Test command called!");
        }
    }
}
