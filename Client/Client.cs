using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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
            commands.Add("disconnect", new Command(CommandDisconnect));

            while(!exit)
            {
                InputParser(Console.ReadLine());
            }

            Disconnect();
            Console.WriteLine("Shutting down...");
        }

        /// <summary>
        /// Handle messaging with the server.
        /// </summary>
        /// <param name="o"></param>
        static void ConnectionListener()
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            while (!exit)
            {
                int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

                if (bytesRead == 0) break;

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine(data);
            }

            stream.Close();
            Disconnect();
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
                if(client.Connected)
                {
                    Libs.SendMessage(client.GetStream(), input);
                }
            }
        }

        /// <summary>
        /// Try to connect to a server.
        /// </summary>
        /// <param name="address"></param>
        static void Connect(string address)
        {
            Console.WriteLine("Trying to connect to " + address + "...");

            client = new TcpClient();

            try
            {
                client.Connect(address, Libs.PORT);
                new Thread(ConnectionListener).Start();
                Console.WriteLine("Connected!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection failed!");
            }
        }

        /// <summary>
        /// Disconnect from the current server.
        /// </summary>
        static void Disconnect()
        {
            if(!client.Connected) return;

            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
            Console.WriteLine("Disconnected!");
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
                Connect(address);
            } 
            else
            {
                Console.WriteLine("Invalid number of arguments!");
            }
        }

        /// <summary>
        /// Disconnect from the current server.
        /// </summary>
        /// <param name="args"></param>
        static void CommandDisconnect(string[] args)
        {
            if(!client.Connected)
            {
                Console.WriteLine("Not connected to any server!");
                return;
            }

            Disconnect();
        }

        #endregion
    }
}
