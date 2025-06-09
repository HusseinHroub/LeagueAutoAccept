using System;
using System.Threading;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting League Client Auto-Accept...");
            // Wait for League Client to open and initialize LCU
            if (!LCU.isLeagueOpen)
            {
                Console.WriteLine("Waiting for League Client to open...");
                LCU.WaitForLeagueClient();
            }

            // Enable auto-accept
            MainLogic.isAutoAcceptOn = true;

            // Start the auto-accept loop in a background thread
            Thread autoAcceptThread = new Thread(MainLogic.StartAutoAcceptLoop)
            {
                IsBackground = true
            };
            autoAcceptThread.Start();

            Console.WriteLine("Auto-accept is running. Write Anything Then Press Enter To Exit...");
            Console.ReadLine();
            // Optionally, you can set isAutoAcceptOn = false here to stop the loop gracefully
            MainLogic.isAutoAcceptOn = false;
        }
    }
}