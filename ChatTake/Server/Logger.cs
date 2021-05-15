using System;
using System.IO;

namespace Server
{
    // responsável por armazenar as mensagens em arquivo de texto 
    public class Logger : ILogger
    {
        private string logFileName;

        public Logger(string logFileName)
        {
            if (String.IsNullOrEmpty(logFileName))
                throw new Exception("LOGFILE config parameter is required.");
            this.logFileName = logFileName;
        }

        // Adiciona a mensagem passada por parâmetro ao arquivo de log
        public void Log(string message)
        {
            try
            {
                File.AppendAllText(logFileName, $"{DateTime.Now.ToString("HH:mm:ss")}: {message}\n" + Environment.NewLine);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
