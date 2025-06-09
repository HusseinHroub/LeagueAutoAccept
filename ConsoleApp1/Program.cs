namespace ConsoleApp1
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Starting League Client Auto-Accept (WebSocket Mode)...");
                    LCU.WaitForLeagueClient();
                    await LCUWebSocketListener.StartAsync(LCU.Port, LCU.Token);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] {ex.GetType().Name}: {ex.Message}");
                    Console.WriteLine("Retrying in 1 second...");
                    await Task.Delay(1000);
                }
            }
        }
    }
}