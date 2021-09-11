using Discord;
using Discord.Commands;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace RandomFacts.CommandModules
{
    public class FactModule : ModuleBase<SocketCommandContext>
    {
        private readonly HttpClient _http;
        public FactModule(HttpClient http)
        {
            _http = http;
        }

        [Command("fact")]
        [Summary("Displays a random fact from Wikipedia.")]
        public async Task GetFactAsync()
        {
            await GetRandomArticle();
        }

        private async Task GetRandomArticle()
        {
            var response = await _http.GetAsync(UriHelpers.WikiRandomUri);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Success: {response.StatusCode}");
                Console.WriteLine($"Article: {response.RequestMessage.RequestUri}");
                string fact = await GetFactFromArticle(UriHelpers.GetArticleTitleFromUri(response.RequestMessage.RequestUri));
                Console.WriteLine($"Fact: {fact}");

                var embed = new EmbedBuilder()
                    .WithTitle("Random Fact:")
                    .WithDescription($"{fact}\n({response.RequestMessage.RequestUri})")
                    .WithFooter(footer => footer.Text = "Created by Nash#2851")
                    .WithColor(Color.Green)
                    .WithCurrentTimestamp();

                await ReplyAsync(embed: embed.Build());
            }
            else
            {
                Console.WriteLine($"Failed: {response.StatusCode}");
            }
        }

        private async Task<string> GetFactFromArticle(string title)
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
