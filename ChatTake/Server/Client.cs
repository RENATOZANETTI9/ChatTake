using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class Client
    {
        NetworkStream stream;
        TcpClient client;
        private string userId;
        private bool connected;

        public string UserId
        {
            get
            {
                return userId;
            }
            set
            {
                userId = value;
            }
        }

        public Client(NetworkStream Stream, TcpClient Client)
        {
            stream = Stream;
            client = Client;
            UserId = "Default User";
            connected = true;
        }

        public void Send(string Message)
        {
            try
            {
                byte[] message = Encoding.ASCII.GetBytes(Message);
                stream.Write(message, 0, message.Count());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Receive(Logger chatLog, Dictionary<string, Client> connectedClientsInChat)
        {
            while (connected == true)
            {
                try
                {
                    byte[] recievedMessage = new byte[88];
                    stream.Read(recievedMessage, 0, recievedMessage.Length);
                    string recievedMessageString = Encoding.ASCII.GetString(recievedMessage);
                    Message message = new Message(this, recievedMessageString);
                    Server.queueMessage.Enqueue(message);
                    Console.WriteLine(recievedMessageString);
                }
                catch
                {
                    lock (Server.dictLock)
                    {
                        connected = false;
                        Console.WriteLine($"\n{this.UserId} has left the chat.");
                        Server.connectedClients.Remove(this.UserId);
                        chatLog.Log($"[{DateTime.Now.ToString("HH:mm:ss tt")}] >> {this.UserId} disconnected");
                        foreach (KeyValuePair<string, Client> keyValue in connectedClientsInChat)
                        {
                            if (keyValue.Key != this.UserId)
                            {
                                keyValue.Value.Send($"\n{this.UserId} has left the chat.");
                            }
                        }
                    }
                }
            }
        }

        public void SetUserName()
        {
            byte[] recievedID = new byte[88];
            stream.Read(recievedID, 0, recievedID.Length);
            string recievedMessageString = Encoding.ASCII.GetString(recievedID).Trim('\0');
            userId = recievedMessageString;
            Console.WriteLine($"{userId} has entered the chat!");
        }
    }
}
