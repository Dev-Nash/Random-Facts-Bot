using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RandomFacts.CommandModules
{
    public class InfoModule : ModuleBase<SocketCommandContext>
    {        
        [Command("help")]
        [Summary("Displays Help.")]
        public async Task SayAsync()
        {
            

            await ReplyAsync("Type \"!fact\" without the quotes to get a random fact!");
        }
    }
}
