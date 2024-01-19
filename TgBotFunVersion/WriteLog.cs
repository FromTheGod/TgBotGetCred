using System;
using System.IO;

namespace TgBotFunVersion
{
    internal class WriteLog
    {
        private string path { get; } = @"C:\Users\User\source\repos\TgBotFunVersion\TgBotFunVersion\Log.txt";

        public void writeIN(string erorr)
        {
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine("\n"+erorr + "\n Время ошибки:" + DateTime.Now.ToString());
            }
        }
    }
}
