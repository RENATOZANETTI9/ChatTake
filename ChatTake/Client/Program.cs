namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var host = Config.host; // captura o endereço do host do arquivo de configuração
            if (host is null) // valida se o host foi informado no arquivo de configuração
                System.Console.WriteLine("HOST config parameter is required...");

            var port = Config.port; // captura a porta do host do arquivo de configuração
            if (port <= 0) // valida se a porta foi informada no arquivo de configuração
                System.Console.WriteLine("PORT config parameter is required...");

            Client client = new Client(host, port);
            System.Console.ReadLine();
        }
    }
}