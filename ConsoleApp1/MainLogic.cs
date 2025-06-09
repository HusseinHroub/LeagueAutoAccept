
using System.Text.Json;
namespace ConsoleApp1
{
    internal class MainLogic
    {
        public static bool isAutoAcceptOn = false;

        public static void StartAutoAcceptLoop()
        {
            while (true)
            {
                if (isAutoAcceptOn)
                {
                    string[] gameSession = LCU.ClientRequest("GET", "lol-gameflow/v1/session");
                    if (gameSession[0] == "200")
                    {
                        string phase = GetPhaseFromSession(gameSession[1]);
                        //Console.WriteLine($"Current Phase: {phase}");
                        if (phase == "ReadyCheck")
                        {
                            AcceptReadyCheck();
                        }

                        // Sleep to avoid spamming the API
                        Thread.Sleep(100);
                    }
                }
                else
                {
                    Console.WriteLine("Auto-accept is turned off. Exiting loop.");
                    break; // Exit the loop if auto-accept is turned off
                }
            }
        }

        private static string GetPhaseFromSession(string sessionJson)
        {
            try
            {
                var session = JsonSerializer.Deserialize<GameSession>(sessionJson);
                return session?.phase ?? "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing session JSON: {ex.Message}");
                return "";
            }
        }

        private static void AcceptReadyCheck()
        {
            Console.WriteLine("Ready Check Detected! Accepting...");
            LCU.ClientRequest("POST", "lol-matchmaking/v1/ready-check/accept");
        }
    }

    public class GameSession
    {
        public string phase { get; set; }
    }
}