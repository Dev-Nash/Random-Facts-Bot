using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RandomFacts
{
    public static class StringHelpers
    {
        public static string GetFactFromJson(string rawJson)
        {
            var json = JObject.Parse(rawJson);
            var pageId = Regex.Match(rawJson, "\"\\d+\"");
            return (string)json.SelectToken($"query.pages.{pageId.Value.Substring(1, pageId.Value.Length - 2)}.extract");
        }

        public static string SanitizeFact(string rawFact)
        {            
            var matches = Regex.Matches(rawFact, @"\w*\.");

            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];
                if (match.Success)
                {
                    if (match.Length > 3)
                    {
                        return CheckForUnwantedChars(rawFact.Substring(0, rawFact.IndexOf(match.Value) + match.Value.Length));
                    }
                }
            }

            if (rawFact.Contains('\n'))
            {
                return CheckForUnwantedChars(rawFact.Substring(0, rawFact.IndexOf('\n')));
            }

            return CheckForUnwantedChars(rawFact);
        }

        private static string CheckForUnwantedChars(string fact)
        {
            if (fact == null)
                return fact;

            if (Encoding.UTF8.GetByteCount(fact) != fact.Length)
            {
                return fact.Substring(0, fact.IndexOf('(')) + fact[(fact.IndexOf(')') + 1)..];
            }            

            return fact;
        }
    }
}
