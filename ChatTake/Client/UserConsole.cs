namespace Client
{
    public static class UserConsole
    {
        // lê informações do console
        public static string GetInput()
        {
            return System.Console.ReadLine();
        }

        // Mostra informações no console
        public static void DisplayMessage(string message)
        {
            System.Console.WriteLine(message);
        }
    }
}
