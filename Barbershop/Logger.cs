using System;
using System.IO;
using System.Text;

namespace Barbershop
{
    public static class Logger
    {
        private const string logFileName = "log";

        private static readonly object locker = new object();
        private static readonly FileStream fileStream;
        private static readonly StreamWriter streamWriter;

        static Logger()
        {
            fileStream = File.Create(logFileName);
            streamWriter = new StreamWriter(fileStream);
        }

        public static void Write(string message)
        {
            lock (locker)
            {
                Console.WriteLine(message);
                streamWriter.WriteLine(message);
            }
        }

        public static void Dispose()
        {
            lock (locker)
            {
                streamWriter.Dispose();
                fileStream.Dispose();
            }
        }

        public static void Flush()
        {
            lock (locker)
            {
                streamWriter.Flush();
            }
        }
    }
}