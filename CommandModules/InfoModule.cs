using Discord.Commands;
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
