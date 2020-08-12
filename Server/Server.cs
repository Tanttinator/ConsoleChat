using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Server
    {
        static Dictionary<string, Command> commands;

        static bool exit = false;

        static object _lock = new object();
        static Dictionary<int, TcpClient> connections = new Dictionary<int, TcpClient>();

        static void Main(string[] args)
        {
            Console.WriteLine("CONSOLECHAT DEDICATED SERVER");

            commands = new Dictionary<string, Command>();
            commands.Add("exit", new Command(CommandExit));

            TcpListener listener = new TcpListener(IPAddress.Any, Libs.PORT);
            listener.Start();
            Console.WriteLine("Server started.");

            Thread inputThread = new Thread(InputHandler);
            inputThread.Start();

            int count = 0;

            while (!exit)
            {
                TcpClient client = listener.AcceptTcpClient();
                lock (_lock) connections.Add(count, client);
                Console.WriteLine("New connection from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

                Thread t = new Thread(ConnectionHandler);
                t.Start(count++);
            }

            Console.WriteLine("Shutting down...");
        }

        /// <summary>
        /// Handle messaging with a single connection.
        /// </summary>
        /// <param name="o"></param>
        static void ConnectionHandler(object o)
        {
            int id = (int)o;
            TcpClient connection;

            lock (_lock) connection = connections[id];

            while(!exit)
            {

            }

            lock (_lock) connections.Remove(id);
            connection.Client.Shutdown(SocketShutdown.Both);
            connection.Close();
            Console.WriteLine("Client disconnected!");
        }

        /// <summary>
        /// Thread for handling inputs by the host.
        /// </summary>
        /// <param name="o"></param>
        static void InputHandler()
        {
            while(!exit)
            {
                Console.Write(">");
                InputParser(Console.ReadLine());
            }
        }

        /// <summary>
        /// Parses user input and decides what to do with it.
        /// </summary>
        /// <param name="input"></param>
        static void InputParser(string input)
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

        #region Commands

        /// <summary>
        /// Tells the program to stop.
        /// </summary>
        /// <param name="args"></param>
        static void CommandExit(string[] args)
        {
            exit = true;
        }

        #endregion
    }
}
