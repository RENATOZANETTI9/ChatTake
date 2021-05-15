using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{

   // responsável por administrar as conexões e as mensagens do server
    public class Server
    {

        // declara o dicionário responsável por armazenar os clientes conectados
        public static Dictionary<string, Client> connectedClients;

        // declara a coleção responsável por armazenar e controlar a fila de mensagens, garantindo que a primeira mensagem que chegar será a primeira a sair
        public static ConcurrentQueue<Message> queueMessage = new ConcurrentQueue<Message>();

        public static Client client;

        private bool isConnected;

        private TcpListener server;

        private Logger log;

        public static Object dictLock;

        public Server()
        {
            // Pega o caminho completo do arquivo de log do arquivo de configuração
            string logFileName = Config.logFile;

            // Instancia o logger para registrar as mensagens em arquivo de texto
            this.log = new Logger(logFileName);

            connectedClients = new Dictionary<string, Client>();

            var host = Config.host; // captura o endereço do host do arquivo de configuração
            if (String.IsNullOrEmpty(host)) // valida se o host foi informado no arquivo de configuração
                throw new Exception("HOST config parameter is required.");

            var port = Config.port; // captura a porta do host do arquivo de configuração
            if (String.IsNullOrEmpty(port.ToString()) || port < 0) // valida se a porta foi informada no arquivo de configuração
                throw new Exception("PORT config parameter is required.");

            server = new TcpListener(IPAddress.Parse(host), port); // cria a conexão utilizando o protocolo tcp  
            server.Start(); // inicia a conexão utilizando o protocolo tcp  
            Console.WriteLine("Waiting for connections...");

            Thread response = new Thread(() => Response());
            response.Start();

            dictLock = new Object();
        }


        /// looping principal da aplicação server
        public void Run()
        {
            isConnected = true;
            while (isConnected)
            {
                try
                {
                     
                    TcpClient clientSocket = default(TcpClient);
                    clientSocket = server.AcceptTcpClient();
                    Thread clientThread = new Thread(() => AcceptClient(clientSocket));
                    clientThread.Start();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    break;
                }
            }
            isConnected = false;
        }

        /// aguarda por novas conexões de clientes  
        private void AcceptClient(TcpClient clientSocket)
        {
            Console.WriteLine("new user connected...");
            NetworkStream stream = clientSocket.GetStream();
            client = new Client(stream, clientSocket);
            Task userName = Task.Run(() => CheckDuplicatedUser(client));
        }

        /// gerencia a fila de mensagens
        private void Response()
        {
            while (isConnected == true)
            {
                Message message = default(Message);
                if (queueMessage.TryDequeue(out message)) // remove uma mensagem da fila
                {
                    if (message.MessageBody.StartsWith("/p ")) // caso a mensagem iniciar com '/p' essa mensagem será enviada de forma privada ao usuário definido após caracter de espaço, logo a frente do '/p'
                    {
                        if (message.MessageBody.Split().Count() <= 0)
                            continue;
                        lock (dictLock) 
                        {
                            string userIdFound = message.MessageBody.Split()[1];
                            bool hasUserID = IsSearchForUserOnline(userIdFound);
                            if (hasUserID == true) // verifica se o usuário informado existe
                            {
                                foreach (KeyValuePair<string, Client> privateSender in connectedClients)
                                {
                                    if (privateSender.Key == userIdFound)
                                    {
                                        message.MessageBody = message.MessageBody.Split()[2];                                        privateSender.Value.Send($"[{DateTime.Now.ToString("HH:mm:ss")}] {message.Sender.UserId}: {message.MessageBody}"); // envia a mensagem apenas ao usuário privado
                                        log.Log($"[{DateTime.Now.ToString("HH:mm:ss")}] {message.Sender.UserId} '\\p' to {userIdFound} >> {message.MessageBody}"); // grava a mensagem no arquivo de log
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        lock (dictLock) 
                        {
                            foreach (KeyValuePair<string, Client> keyValue in connectedClients)
                            {
                                if (keyValue.Key != message.UserId)
                                {
                                    keyValue.Value.Send($"[{DateTime.Now.ToString("HH:mm:ss")}] {message.Sender.UserId}: {message.MessageBody}"); // envia a mensagem apenas ao usuário privado
                                    log.Log($"[{DateTime.Now.ToString("HH:mm:ss")}] {message.Sender.UserId} >> {message.MessageBody}"); // grava a mensagem no arquivo de log
                                }
                            }
                        }
                    }
                }
            }
        }

        /// Mostra os usuários on-line
        private void ShowOnlineUsers(Client client)
        {
            lock (dictLock)
            {
                if (connectedClients.Count() > 1) // se existe mais de um usuário conectado
                {
                    foreach (KeyValuePair<string, Client> keyValue in connectedClients)
                    {
                        if (keyValue.Key != client.UserId)
                        {
                            keyValue.Value.Send($"\n{client.UserId} joined the chat!");
                        }
                    }
                    client.Send("Online Users:");
                    foreach (KeyValuePair<string, Client> online in connectedClients)
                    {
                        if (online.Key != client.UserId)
                        {
                            client.Send($"\n>>{online.Key}");
                        }
                    }
                }
                else // somente o usuário atual conectado
                {
                    client.Send($"You're the only one here {client.UserId}!");
                }
            }
        }

        /// verifica se já existe um usuário com o nome informado
        private void CheckDuplicatedUser(Client client)
        {
            client.SetUserName();
            lock (dictLock)
            {
                if (!connectedClients.ContainsKey(client.UserId)) // verifica se o usuário já existe no dicionário de clientes conectados
                {
                    connectedClients.Add(client.UserId, client); //adiciona o usuário no dicionário de clientes conectados
                    ShowOnlineUsers(client); // mostra o novo usuário na lista de usuários conectados
                    log.Log($"[{DateTime.Now.ToString("HH:mm:ss")}] >> {client.UserId} connected to the chat");
                    Thread receive = new Thread(() => client.Receive(log, connectedClients));
                    receive.Start();
                }
                else // usuário já existe
                {
                    client.Send("This username already in use.\n");
                }
            }
        }

        /// verifica se já existe um usuário com o id informado, no dicionário de clientes conectados
        private bool IsSearchForUserOnline(string matchedUserId)
        {
            lock (dictLock)
            {
                if (connectedClients.ContainsKey(matchedUserId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }


    }
}










