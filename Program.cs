using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace RandomFacts
{
    class Program
    {        
        static void Main(string[] args) => new Program().RunAsync().GetAwaiter().GetResult();

        private readonly HttpClient _http;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        private Program()
        {
            _client = new DiscordSocketClient(new DiscordSocketConfig { LogLevel = LogSeverity.Info });

            _commands = new CommandService(new CommandServiceConfig { LogLevel = LogSeverity.Info, CaseSensitiveCommands = false });

            _http = new();
            _http.DefaultRequestHeaders.Accept.Clear();
            _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _client.Log += Log;
            _commands.Log += Log;
        }

        public async Task RunAsync()
        {                                                          
            await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("BotToken", EnvironmentVariableTarget.Machine));
            await _client.StartAsync();

            await Task.Delay(-1);                       
        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{msg.Severity,8}] {msg.Source}: {msg.Message} {msg.Exception}");
            Console.ResetColor();
            
            return Task.CompletedTask;
        }        

        public async Task GetRandomArticle()
        {
            var response = await _http.GetAsync(UriHelpers.WikiRandomUri);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Success: {response.StatusCode}");
                Console.WriteLine($"Article: {response.RequestMessage.RequestUri}");
                string fact = await GetFactFromArticle(UriHelpers.GetArticleTitleFromUri(response.RequestMessage.RequestUri));
                Console.WriteLine($"Fact: {fact}");
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }
        }

        public async Task<string> GetFactFromArticle(string title)
        {
            var response = await _http.GetAsync(UriHelpers.WikiPHPRequestUri(title));
            string jsonContent = await response.Content.ReadAsStringAsync();

            string fact = string.Empty;
            try
            {
                fact = StringHelpers.GetFactFromJson(jsonContent);
            }
            catch
            {
                Console.WriteLine("Something Broke! D:");
            }

            return StringHelpers.SanitizeFact(fact);
        }
    }
}
