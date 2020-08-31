using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Server
    {
        static Dictionary<string, Command> commands;

        static bool exit = false;

        static object _lock = new object();
        static Dictionary<int, TcpClient> clients = new Dictionary<int, TcpClient>();

        static TcpListener listener;

        static void Main(string[] args)
        {
            Console.ForegroundColor = Libs.DEFAULT_COLOR;
            Console.WriteLine("CONSOLECHAT DEDICATED SERVER");

            commands = new Dictionary<string, Command>();
            commands.Add("exit", new Command(CommandExit));

            listener = new TcpListener(IPAddress.Any, Libs.PORT);
            listener.Start();
            Libs.StatusMessage("Server started.");

            Thread inputThread = new Thread(InputHandler);
            inputThread.Start();

            int count = 0;

            while (!exit)
            {
                TcpClient client = listener.AcceptTcpClient();
                lock (_lock) clients.Add(count, client);
                Libs.StatusMessage("New connection from " + ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString());

                Thread t = new Thread(ConnectionListener);
                t.Start(count++);
            }

            Libs.StatusMessage("Shutting down...");
        }

        /// <summary>
        /// Handle messaging with a single client.
        /// </summary>
        /// <param name="o"></param>
        static void ConnectionListener(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock) client = clients[id];

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];

            while(!exit)
            {
                int bytesRead = stream.Read(buffer, 0, client.ReceiveBufferSize);

                if(bytesRead == 0)  break;

                ReceiveMessage(id, Message.FromBytes(buffer));
            }

            DisconnectClient(id);
        }

        /// <summary>
        /// Relay a message sent by one client to the other clients.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        static void ReceiveMessage(int client, Message message)
        {
            Libs.DisplayMessage(message);
            foreach(int otherClient in clients.Keys)
            {
                if (otherClient != client) Libs.SendMessage(clients[otherClient].GetStream(), message);
            }
        }

        /// <summary>
        /// Disconnect a client from this server.
        /// </summary>
        /// <param name="client"></param>
        static void DisconnectClient(int id)
        {
            TcpClient client = clients[id];
            lock (_lock) clients.Remove(id);
            client.Client.Shutdown(SocketShutdown.Both);
            client.Close();
            Libs.StatusMessage("Client disconnected!");
        }

        /// <summary>
        /// Thread for handling inputs by the host.
        /// </summary>
        /// <param name="o"></param>
        static void InputHandler()
        {
            while(!exit)
            {
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
                foreach (TcpClient client in clients.Values) Libs.SendMessage(client.GetStream(), new Message(input, "Server", (int)ConsoleColor.Gray));
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
            listener.Stop();
        }

        #endregion
    }
}
