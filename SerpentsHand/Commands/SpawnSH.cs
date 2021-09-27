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
	class SpawnSH : ICommand
	{
		public string[] Aliases { get; set; } = Array.Empty<string>();

		public string Description { get; set; } = "Spawn a single Serpents Hand";

		string ICommand.Command { get; } = "spawnsh";

		public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
		{
            if (arguments.Count > 0 && arguments.ElementAt(0).Length > 0)
            {
                Player cPlayer = Player.Get(arguments.ElementAt(0));
                if (cPlayer != null)
                {
                    EventHandlers.SpawnPlayer(cPlayer);
                    response = $"Spawned {cPlayer.Nickname} as Serpents Hand.";
                    return true;
                }
                else
                {
                    response = "Invalid player.";
                    return true;
                }
            }
            else
            {
                response = "SPAWNSH [Player Name / Player ID]";
            }
            return true;
		}
	}
}
