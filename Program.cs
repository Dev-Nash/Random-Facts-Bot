using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RandomFacts
{
    class Program
    {
        static HttpClient client = new();
                
        static async Task GetRandomArticle()
        {
            var response = await client.GetAsync(UriHelpers.WikiRandomUri);
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

        static async Task<string> GetFactFromArticle(string title)
        {
            var response = await client.GetAsync(UriHelpers.WikiPHPRequestUri(title));
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

        static void Main()
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {            
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            while(true)
            {
                Console.Write("fact?: ");

                var input = Console.ReadLine();

                if (input == "y")
                {
                    Console.WriteLine();
                    await GetRandomArticle();
                    Console.WriteLine();
                    continue;
                }

                break;
            }            
        }
    }
}
