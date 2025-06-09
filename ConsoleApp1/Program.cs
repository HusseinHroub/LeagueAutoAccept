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
            var leagueAuth = typeof(LCU)
                .GetField("leagueAuth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                .GetValue(null) as string[];
            string auth = leagueAuth[0];
            string port = leagueAuth[1];

            var wsTask = LCUWebSocketListener.StartAsync(port, auth);

            Console.WriteLine("WebSocket auto-accept is running. Press Enter to exit...");
            Console.ReadLine();
        }
    }
}