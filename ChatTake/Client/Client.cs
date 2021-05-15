using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    // responsável por administrar as conexões e as mensagens do client
    public class Client
    {
        TcpClient clientSocket;
        NetworkStream stream;
        bool connected;

        public Client(string host, int port)
        {
            connected = true;
            using (clientSocket = new TcpClient())
            {
                Console.WriteLine("Connecting.....");
                try
                {
                    clientSocket.Connect(IPAddress.Parse(host), port); // Conecta ao socket  

                }
                catch (Exception)
                {
                    Console.WriteLine($"Server {host}:{port} not found...");
                    Thread.Sleep(5000);
                    Environment.Exit(-1);
                }
                Console.WriteLine("Connected");
                string userName = GetUserName();
                stream = clientSocket.GetStream();
                try
                {
                    byte[] message = Encoding.ASCII.GetBytes(userName);
                    stream.Write(message, 0, message.Count());
                }
                catch
                {
                    Console.WriteLine("Something went wrong.");
                }
                Task send = Task.Run(() => Send()); // executa o método responsável por monitorar as mensagens enviadas
                Task receive = Task.Run(() => Receive()); // executa o método responsável por monitorar as mensagens enviadas
                receive.Wait(); // aguarda as threads

                clientSocket.Close();
                Console.WriteLine("Disconnected.");
                Console.ReadKey();
                connected = false;
            }
        }

        // solicita o nome do usuário
        private string GetUserName()
        {
            Console.WriteLine("Please enter your username.");
            string user = UserConsole.GetInput();
            return user;
        }

        // monitora as mensagens enviadas
        public void Send()
        {
            while (connected == true)
            {
                try
                {
                    string messageString = UserConsole.GetInput();
                    byte[] message = Encoding.ASCII.GetBytes(messageString);
                    stream.Write(message, 0, message.Count());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        // monitora as mensagens recebidas
        public void Receive()
        {
            while (connected == true)
            {
                try
                {
                    byte[] receivedMessages = new byte[88];
                    stream.Read(receivedMessages, 0, receivedMessages.Length);
                    UserConsole.DisplayMessage(Encoding.ASCII.GetString(receivedMessages));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

    }
}