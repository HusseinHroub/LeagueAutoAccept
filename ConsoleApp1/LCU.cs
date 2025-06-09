using System.Diagnostics;
using System.Management;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp1
{
    internal class LCU
    {
        public static string Token { get; private set; }
        public static string Port { get; private set; }
        public static bool isLeagueOpen = false;

        public static bool CheckIfLeagueClientIsOpen()
        {
            return Process.GetProcessesByName("LeagueClientUx").Any();
        }

        public static void WaitForLeagueClient()
        {
            while (true)
            {
                var client = Process.GetProcessesByName("LeagueClientUx").FirstOrDefault();
                if (client != null)
                {
                    SetLeagueAuth(client);
                    isLeagueOpen = true;
                    break;
                }

                Console.WriteLine("Waiting for League Client to start...");
                Thread.Sleep(2000);
            }
        }

        private static void SetLeagueAuth(Process client)
        {
            string query = $"SELECT CommandLine FROM Win32_Process where ProcessId = {client.Id}";
            string commandLine = "";

            using (var searcher = new ManagementObjectSearcher(query))
            using (var results = searcher.Get())
            {
                foreach (var result in results)
                {
                    commandLine = result["CommandLine"]?.ToString();
                    break;
                }
            }

            Port = Regex.Match(commandLine, @"--app-port=""?(\d+)""?").Groups[1].Value;
            string authToken = Regex.Match(commandLine, @"--remoting-auth-token=([a-zA-Z0-9_-]+)").Groups[1].Value;
            Token = Convert.ToBase64String(Encoding.UTF8.GetBytes("riot:" + authToken));
        }

        public static string[] ClientRequest(string method, string url, string body = null)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            try
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri("https://127.0.0.1:" + Port + "/");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Token);

                    var request = new HttpRequestMessage(new HttpMethod(method), url);

                    if (!string.IsNullOrEmpty(body))
                    {
                        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                    }

                    HttpResponseMessage response = client.SendAsync(request).Result;

                    if (response == null)
                        return new string[] { "999", "" };

                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    return new string[] { ((int)response.StatusCode).ToString(), responseContent };
                }
            }
            catch
            {
                return new string[] { "999", "" };
            }
        }

        public static string[] ClientRequestUntilSuccess(string method, string url, string body = null)
        {
            string[] response;
            do
            {
                response = ClientRequest(method, url, body);
                if (!CheckIfLeagueClientIsOpen()) break;
                if (!response[0].StartsWith("2")) Thread.Sleep(1000);
            } while (!response[0].StartsWith("2"));

            return response;
        }
    }
}
