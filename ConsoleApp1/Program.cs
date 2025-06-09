using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting League Client Auto-Accept (WebSocket Mode)...");
            if (!LCU.isLeagueOpen)
            {
                Console.WriteLine("Waiting for League Client to open...");
                LCU.WaitForLeagueClient();
            }

            // Start the WebSocket listener (async)
            var wsTask = LCUWebSocketListener.StartAsync(LCU.Port, LCU.Token);

            Console.WriteLine("WebSocket auto-accept is running. Press Enter to exit...");
            Console.ReadLine();
        }
    }
}