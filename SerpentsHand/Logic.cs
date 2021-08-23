using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using Exiled.API.Extensions;

namespace SerpentsHand
{
    using Exiled.API.Enums;
	using Exiled.Loader;
	using System.Reflection;

	partial class EventHandlers
    {
        internal static void SpawnPlayer(Player player, bool full = true)
        {
            shPlayers.Add(player.Id);
            player.SetRole(RoleType.Tutorial);
            player.Broadcast(10, SerpentsHand.instance.Config.SpawnBroadcast);
            if (full)
            {
                Timing.CallDelayed(1f, () =>
                {
                    for (int i = 0; i < SerpentsHand.instance.Config.SpawnItems.Count; i++)
                    {
                        player.AddItem(SerpentsHand.instance.Config.SpawnItems[i]);
                    }
                    player.Health = SerpentsHand.instance.Config.Health;
                });
                // Prevent Serpents Hand from taking up Chaos spawn tickets
                //Respawning.RespawnTickets.Singleton.GrantTickets(Respawning.SpawnableTeamType.ChaosInsurgency, 1);
            }

			Timing.CallDelayed(1.5f, () => player.Position = shSpawnPos);
            Player scp966 = Player.List.FirstOrDefault(p => p.SessionVariables.ContainsKey("is966") && (bool)p.SessionVariables["is966"]);
            if (scp966 != null)
            {
                player.TargetGhostsHashSet.Remove(scp966.Id);
            }
        }

        internal static void CreateSquad(int size)
        {
            List<Player> spec = new List<Player>();
            List<Player> pList = Player.List.ToList();

            foreach (Player player in pList)
            {
                if (player.Team == Team.RIP)
                {
                    spec.Add(player);
                }
            }

            int spawnCount = 1;
            while (spec.Count > 0 && spawnCount <= size)
            {
                int index = rand.Next(0, spec.Count);
                if (spec[index] != null)
                {
                    SpawnPlayer(spec[index], true);
                    spec.RemoveAt(index);
                    spawnCount++;
                }
            }
        }

        internal static void SpawnSquad(List<Player> players)
        {
            foreach (Player player in players)
            {
                SpawnPlayer(player);
            }

            Cassie.Message(SerpentsHand.instance.Config.EntryAnnouncement, true, true);
        }

        internal static void GrantFF()
		{
            foreach (int id in shPlayers)
            {
                Player p = Player.Get(id);
                if (p != null) p.IsFriendlyFireEnabled = true;
            }

            foreach (int id in SerpentsHand.instance.EventHandlers.shPocketPlayers)
            {
                Player p = Player.Get(id);
                if (p != null) p.IsFriendlyFireEnabled = true;
            }

            shPlayers.Clear();
            SerpentsHand.instance.EventHandlers.shPocketPlayers.Clear();
        }

        private Player TryGet035()
        {
            Player scp035 = null;
            if (SerpentsHand.isScp035)
                scp035 = (Player)Loader.Plugins.First(pl => pl.Name == "scp035").Assembly.GetType("scp035.API.Scp035Data").GetMethod("GetScp035", BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
            return scp035;
        }

        private int CountRoles(Team team)
        {
            Player scp035 = null;

            if (SerpentsHand.isScp035)
            {
                scp035 = TryGet035();
            }

            int count = 0;
            foreach (Player pl in Player.List)
            {
                if (pl.Team == team)
                {
                    if (scp035 != null && pl.Id == scp035.Id) continue;
                    count++;
                }
            }
            return count;
        }

        private void TeleportTo106(Player player)
        {
            Player scp106 = Player.List.Where(x => x.Role == RoleType.Scp106).FirstOrDefault();
            if (scp106 != null)
            {
                player.Position = scp106.Position;
            }
            else
            {
                player.Position = RoleType.Scp096.GetRandomSpawnProperties().Item1;
            }
        }
    }
}