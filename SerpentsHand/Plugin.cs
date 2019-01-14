using Smod2;
using Smod2.Attributes;
using System;
using Smod2.API;
using System.Collections.Generic;
using scp4aiur;
using System.Linq;

namespace SerpentsHand
{
	[PluginDetails(
	author = "Cyanox",
	name = "Serpents Hand",
	description = "A new class for SCP:SL",
	id = "cyan.serpents.hand",
	version = "0.5.2",
	SmodMajor = 3,
	SmodMinor = 0,
	SmodRevision = 0
	)]
	public class SHPlugin : Plugin
    {
		public static SHPlugin instance;

		public static Random rand = new Random();

		public static List<string> shPlayersInPocket = new List<string>();
		public static List<string> shPlayers = new List<string>();
		public static List<int> shItemList = new List<int>();
        public static Player scp106;

		private static readonly Vector shSpawnPos = new Vector(0, 1001, 8);

		public static string ciAnnouncement;
		public static string shAnnouncement;

		public static int spawnChance;
		public static int shMaxSquad;
		public static int shHealth;

		public static bool friendlyFire;
		public static bool ciWinWithSCP;
		public static bool teleportTo106;

		public override void OnEnable() => Info(Details.name + " Activated!");

		public override void OnDisable() {}

		public override void Register()
		{
			instance = this;

			AddEventHandlers(new EventHandler());

			Timing.Init(this);

			AddConfig(new Smod2.Config.ConfigSetting("sh_spawn_chance", 50, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_entry_announcement", "serpents hand entered", Smod2.Config.SettingType.STRING, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_spawn_items", new[] 
			{
				20,
				26,
				12,
				14,
				10
			}, Smod2.Config.SettingType.NUMERIC_LIST, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_ci_entry_announcement", "", Smod2.Config.SettingType.STRING, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_friendly_fire", false, Smod2.Config.SettingType.BOOL, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_teleport_to_106", true, Smod2.Config.SettingType.BOOL, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_ci_win_with_scp", false, Smod2.Config.SettingType.BOOL, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_health", 120, Smod2.Config.SettingType.NUMERIC, true, ""));
			AddConfig(new Smod2.Config.ConfigSetting("sh_max_squad", 8, Smod2.Config.SettingType.NUMERIC, true, ""));

			AddCommands(new[] { "spawnsh" }, new SpawnCommand());
			AddCommands(new[] { "spawnshsquad" }, new SpawnSquad());
		}

		public static Player FindPlayer(string identifier)
		{
			return PluginManager.Manager.Server.GetPlayers(identifier).FirstOrDefault();
		}

		public static void TeleportTo106(Player Player)
		{
			if (scp106 != null)
			{
				Timing.Next(() =>
				{
					Player.Teleport(scp106.GetPosition());
				});
			}
		}

		public static void SpawnPlayer(Player player)
		{
			shPlayers.Add(player.SteamId);
			player.ChangeRole(Role.TUTORIAL, false);
			player.SetAmmo(AmmoType.DROPPED_5, 250);
			player.SetAmmo(AmmoType.DROPPED_7, 250);
			player.SetAmmo(AmmoType.DROPPED_9, 250);
			player.SetHealth(shHealth);
			player.Teleport(shSpawnPos);
		}

		public static void SpawnSHSquad(List<Player> Playerlist)
		{
			List<Player> SHPlayers = new List<Player>();
			List<Player> CIPlayers = Playerlist;
			for (int i = 0; i < shMaxSquad && CIPlayers.Count > 0; i++)
			{
				Player player = CIPlayers[rand.Next(CIPlayers.Count)];
				SHPlayers.Add(player);
				CIPlayers.Remove(player);
			}

			foreach (Player player in SHPlayers)
				SpawnPlayer(player);
			foreach (Player player in CIPlayers)
				player.ChangeRole(Role.SPECTATOR);

			PluginManager.Manager.Server.Map.AnnounceCustomMessage(shAnnouncement);
		}

		public static int CountRoles(Role role)
		{
			return PluginManager.Manager.Server.GetPlayers().Count(x => x.TeamRole.Role == role);
		}

		public static int CountRoles(Smod2.API.Team team)
		{
			return PluginManager.Manager.Server.GetPlayers().Count(x => x.TeamRole.Team == team);
		}

		public static void SpawnSquad(int size)
		{
		    List<Player> spec = PluginManager.Manager.Server.GetPlayers()
		        .Where(x => x.TeamRole.Team == Smod2.API.Team.SPECTATOR)
		        .ToList();


            int spawnCount = 1;
			while (spec.Count > 0 && spawnCount <= size)
			{
				int index = rand.Next(0, spec.Count);
				if (spec[index] != null)
				{
					SpawnPlayer(spec[index]);
					spec.RemoveAt(index);
					spawnCount++;
				}
			}
		}
	}
}
