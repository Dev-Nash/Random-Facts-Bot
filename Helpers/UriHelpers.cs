using System;

namespace RandomFacts
{
    public static class UriHelpers
    {
        private const string WikiRandomString = @"https://en.wikipedia.org/wiki/Special:Random";
        private const string WikiPHPRequestString = @"https://en.wikipedia.org/w/api.php?format=json&action=query&prop=extracts&exintro&explaintext&redirects=1&titles";
        
        public static Uri WikiRandomUri => new(WikiRandomString);
        
        public static Uri WikiPHPRequestUri(string title) => new($"{WikiPHPRequestString}={title}");

        public static string GetArticleTitleFromUri(Uri uri) => uri.AbsoluteUri[30..];
    }
}
