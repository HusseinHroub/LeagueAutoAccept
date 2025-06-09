using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    internal class LCUWebSocketListener
    {
        public static async Task StartAsync(string port, string auth)
        {

            Console.WriteLine("auth and port: " + auth + " " + port);
            using var ws = new ClientWebSocket();
            ws.Options.RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            ws.Options.SetRequestHeader("Authorization", "Basic " + auth);

            var uri = new Uri($"wss://127.0.0.1:{port}/");
            await ws.ConnectAsync(uri, CancellationToken.None);

            // Subscribe to the session event
            var subscribeMsg = "[5, \"OnJsonApiEvent_lol-gameflow_v1_session\"]";
            await ws.SendAsync(Encoding.UTF8.GetBytes(subscribeMsg), WebSocketMessageType.Text, true, CancellationToken.None);

            var buffer = new byte[4096];
            while (ws.State == WebSocketState.Open)
            {
                Console.WriteLine("Waiting for WebSocket messages...");
                var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Received message: {msg}");
                // Look for phase changes in the event payload
                if (msg.Contains("ReadyCheck"))
                {
                    Console.WriteLine("Ready Check Detected! Accepting...");
                    LCU.ClientRequest("POST", "lol-matchmaking/v1/ready-check/accept");
                }
            }
        }
    }
}