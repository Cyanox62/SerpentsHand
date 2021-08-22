using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandSystem;
using Exiled.API.Features;
using RemoteAdmin;

namespace SerpentsHand.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    class SpawnSHSquad : ICommand
    {
        public string[] Aliases { get; set; } = Array.Empty<string>();

        public string Description { get; set; } = "Spawn a squad of Serpents Hand";

        string ICommand.Command { get; } = "spawnshsquad";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count > 0)
            {
                if (int.TryParse(arguments.ElementAt(0), out int a))
                {
                    EventHandlers.CreateSquad(a);
                }
                else
                {
                    response = "Error: invalid size.";
                    return true;
                }
            }
            else
            {
                EventHandlers.CreateSquad(5);
            }
            Cassie.Message(SerpentsHand.instance.Config.EntryAnnouncement, true, true);
            response = "Spawned squad.";
            return true;
        }
    }
}
