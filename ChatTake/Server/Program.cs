using System;

namespace Server
{
    // Classe principal, será instanciada quando essa aplicação for executada
    class Program
    {
        // Método principal, será chamado quando essa aplicação for executada
        static void Main(string[] args)
        {
            // Instancia a classe server e executa o médoto run 
            new Server().Run();
            Console.ReadLine();
        }
    }
}

