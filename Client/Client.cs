using System;
using System.Collections.Generic;

namespace Client
{
    class Client
    {
        static Dictionary<string, Command> commands;

        static bool exit = false;

        static void Main(string[] args)
        {
            Console.WriteLine("CONSOLECHAT CLIENT");

            commands = new Dictionary<string, Command>();
            commands.Add("exit", new Command(CommandExit));

            while(!exit)
            {
                Console.Write(">");
                HandleInput(Console.ReadLine());
            }

            Console.WriteLine("Shutting down...");
        }

        /// <summary>
        /// Parses user input and decides what to do with it.
        /// </summary>
        /// <param name="input"></param>
        static void HandleInput(string input)
        {
            if(input.Trim().StartsWith("/"))
            {
                Libs.HandleCommand(input, commands);
            }
            else
            {
                Console.WriteLine(input);
            }
        }

        /// <summary>
        /// Tells the program to stop.
        /// </summary>
        /// <param name="args"></param>
        static void CommandExit(string[] args)
        {
            exit = true;
        }
    }
}
