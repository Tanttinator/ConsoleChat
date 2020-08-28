using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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
    public static void SendMessage(NetworkStream stream, Message message)
    {
        byte[] bytes = message.GetBytes();
        stream.Write(bytes, 0, bytes.Length);
    }
}

public struct Message
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
    public string message;

    public Message(string message)
    {
        this.message = message;
    }

    /// <summary>
    /// Convert this message into a byte array.
    /// </summary>
    /// <returns></returns>
    public byte[] GetBytes()
    {
        int size = Marshal.SizeOf(this);
        byte[] bytes = new byte[size];

        IntPtr pointer = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(this, pointer, true);
        Marshal.Copy(pointer, bytes, 0, size);
        Marshal.FreeHGlobal(pointer);

        return bytes;
    }

    /// <summary>
    /// Create a message from a byte array.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static Message FromBytes(byte[] bytes)
    {
        Message message = new Message();

        int size = Marshal.SizeOf(message);
        IntPtr pointer = Marshal.AllocHGlobal(size);

        Marshal.Copy(bytes, 0, pointer, size);

        message = Marshal.PtrToStructure<Message>(pointer);
        Marshal.FreeHGlobal(pointer);

        return message;
    }

    public override string ToString()
    {
        return message;
    }
}

public delegate void Command(string[] args);
