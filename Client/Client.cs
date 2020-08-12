using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Client
{
    class Client
    {
        static Dictionary<string, Command> commands;

        static bool exit = false;

        static TcpClient client;

        static void Main(string[] args)
        {
            Console.WriteLine("CONSOLECHAT CLIENT");

            commands = new Dictionary<string, Command>();
            commands.Add("exit", new Command(CommandExit));
            commands.Add("connect", new Command(CommandConnect));

            client = new TcpClient();

            while(!exit)
            {
                Console.Write(">");
                InputParser(Console.ReadLine());
            }

            client.Close();
            Console.WriteLine("Shutting down...");
        }

        /// <summary>
        /// Parses user input and decides what to do with it.
        /// </summary>
        /// <param name="input"></param>
        static void InputParser(string input)
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

        #region Commands

        /// <summary>
        /// Tells the program to stop.
        /// </summary>
        /// <param name="args"></param>
        static void CommandExit(string[] args)
        {
            exit = true;
        }

        /// <summary>
        /// Tries to connect to a server.
        /// </summary>
        /// <param name="args"></param>
        static void CommandConnect(string[] args)
        {
            if(args.Length > 0)
            {
                string address = args[0];
                Console.WriteLine("Trying to connect to " + address + "...");
                try
                {
                    client.Connect(address, Libs.PORT);
                    Console.WriteLine("Connected!");
                } 
                catch(Exception e)
                {
                    Console.WriteLine("Connection failed!");
                }
            } 
            else
            {
                Console.WriteLine("Invalid number of arguments!");
            }
        }

        #endregion
    }
}
