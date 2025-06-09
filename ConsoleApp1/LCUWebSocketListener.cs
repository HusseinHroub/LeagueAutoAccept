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
            using var ws = CreateWebSocket(auth);
            await ConnectWebSocketAsync(ws, port);
            await SubscribeToSessionEventsAsync(ws);
            await ListenForMessagesAsync(ws);
        }

        private static ClientWebSocket CreateWebSocket(string auth)
        {
            var ws = new ClientWebSocket();
            ws.Options.RemoteCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            ws.Options.SetRequestHeader("Authorization", "Basic " + auth);
            return ws;
        }

        private static async Task ConnectWebSocketAsync(ClientWebSocket ws, string port)
        {
            var uri = new Uri($"wss://127.0.0.1:{port}/");
            await ws.ConnectAsync(uri, CancellationToken.None);
        }

        private static async Task SubscribeToSessionEventsAsync(ClientWebSocket ws)
        {
            var subscribeMsg = "[5, \"OnJsonApiEvent_lol-gameflow_v1_session\"]";
            var msgBytes = Encoding.UTF8.GetBytes(subscribeMsg);
            await ws.SendAsync(msgBytes, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private static async Task ListenForMessagesAsync(ClientWebSocket ws)
        {
            var buffer = new byte[4096];
            while (ws.State == WebSocketState.Open)
            {
                var msg = await ReceiveFullMessageAsync(ws, buffer);
                HandleWebSocketMessage(msg);
            }
        }

        private static async Task<string> ReceiveFullMessageAsync(ClientWebSocket ws, byte[] buffer)
        {
            var messageBuilder = new StringBuilder();
            WebSocketReceiveResult result;
            do
            {
                result = await ws.ReceiveAsync(buffer, CancellationToken.None);
                messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            }
            while (!result.EndOfMessage);
            return messageBuilder.ToString();
        }

        private static void HandleWebSocketMessage(string msg)
        {
            try
            {
                using var doc = JsonDocument.Parse(msg);
                if (doc.RootElement.ValueKind == JsonValueKind.Array && doc.RootElement.GetArrayLength() > 2)
                {
                    var payload = doc.RootElement[2];
                    var data = payload.GetProperty("data");
                    var gameSession = JsonSerializer.Deserialize<GameSessionWS>(data.GetRawText());
                    if (gameSession?.phase == "ReadyCheck")
                    {
                        Console.WriteLine("Ready Check Detected! Accepting...");
                        LCU.ClientRequest("POST", "lol-matchmaking/v1/ready-check/accept");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing WebSocket message: {ex.Message}");
            }
        }
    }

    public class GameSessionWS
    {
        public string phase { get; set; }
    }
}