﻿using Smod2;
using Smod2.Events;
using Smod2.EventSystem.Events;
using Smod2.EventHandlers;
using Smod2.API;
using scp4aiur;
using System.Collections.Generic;
using System.Linq;

namespace SerpentsHand
{
	public class EventHandler : IEventHandlerRoundStart, IEventHandlerTeamRespawn, IEventHandlerPocketDimensionEnter, IEventHandlerPocketDimensionDie,
		IEventHandlerPocketDimensionExit, IEventHandlerPlayerHurt, IEventHandlerPlayerDie, IEventHandlerCheckRoundEnd, IEventHandlerWaitingForPlayers,
		IEventHandlerSetRole, IEventHandlerDisconnect
	{
        // Used to tell OnCheckRoundEnd that it should refresh the SH players and make sure they are all connected
	    private bool disconnectOccured;

		public void SetConfigs()
		{
			SHPlugin.shItemList = new List<int>(SHPlugin.instance.GetConfigIntList("sh_spawn_items"));
			SHPlugin.shAnnouncement = SHPlugin.instance.GetConfigString("sh_entry_announcement");
			SHPlugin.ciAnnouncement = SHPlugin.instance.GetConfigString("sh_ci_entry_announcement");

			SHPlugin.spawnChance = SHPlugin.instance.GetConfigInt("sh_spawn_chance");
			SHPlugin.shMaxSquad = SHPlugin.instance.GetConfigInt("sh_max_squad");
			SHPlugin.shHealth = SHPlugin.instance.GetConfigInt("sh_health");

			SHPlugin.friendlyFire = SHPlugin.instance.GetConfigBool("sh_friendly_fire");
			SHPlugin.ciWinWithSCP = SHPlugin.instance.GetConfigBool("sh_ci_win_with_scp");
			SHPlugin.teleportTo106 = SHPlugin.instance.GetConfigBool("sh_teleport_to_106");
		}

		public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
		{
			SetConfigs();
		}

		public void OnRoundStart(RoundStartEvent ev)
		{
			SHPlugin.shPlayers.Clear();
			SHPlugin.shPlayersInPocket.Clear();
		}

		public void OnSetRole(PlayerSetRoleEvent ev)
		{
			if (ev.Player.TeamRole.Team == Smod2.API.Team.TUTORIAL && SHPlugin.shPlayers.Contains(ev.Player.SteamId))
			{
				ev.Items.Clear();

				int max = System.Enum.GetValues(typeof(ItemType)).Cast<int>().Max(), 
				    min = System.Enum.GetValues(typeof(ItemType)).Cast<int>().Min();

				foreach (int id in SHPlugin.shItemList.Where(x => min <= x && x <= max))
					ev.Items.Add((ItemType)id);
			}
            else if (ev.Player.TeamRole.Role == Role.SCP_106)
			{
			    SHPlugin.scp106 = ev.Player;
			}
            else if (ev.Player.PlayerId == SHPlugin.scp106.PlayerId) // If they were 106 but got swapped to someone besides 106
			{
			    SHPlugin.scp106 = null;
			}
        }

		public void OnTeamRespawn(TeamRespawnEvent ev)
		{
			if (ev.SpawnChaos)
			{
				if (SHPlugin.rand.Next(1, 101) <= SHPlugin.spawnChance && ev.PlayerList.Count > 0)
				{
					Timing.InTicks(() => SHPlugin.SpawnSHSquad(ev.PlayerList), 4);
				}
				else
				{
					string ann = SHPlugin.ciAnnouncement;
					if (ann != "")
						PluginManager.Manager.Server.Map.AnnounceCustomMessage(ann);
				}
			}
		}

		public void OnPlayerHurt(PlayerHurtEvent ev)
		{
			if (((SHPlugin.shPlayers.Contains(ev.Player.SteamId) && (ev.Attacker.TeamRole.Team == Smod2.API.Team.SCP || ev.DamageType == DamageType.POCKET)) || // If the victim is a SH member and the attacker is an SCP or 106 dimension
				(SHPlugin.shPlayers.Contains(ev.Attacker.SteamId) && ev.Player.TeamRole.Team == Smod2.API.Team.SCP) || // If the victim is an SCP and the attacker is a SH member
				(SHPlugin.shPlayers.Contains(ev.Player.SteamId) && SHPlugin.shPlayers.Contains(ev.Attacker.SteamId) && ev.Player.SteamId != ev.Attacker.SteamId)) && // If the victim and attacker are SH members but not the same person
			    !SHPlugin.friendlyFire)
			{
				ev.Damage = 0;
			}
		}

		public void OnPlayerDie(PlayerDeathEvent ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
				SHPlugin.shPlayers.Remove(ev.Player.SteamId);

			if (ev.Player.TeamRole.Role == Role.SCP_106 && !SHPlugin.friendlyFire)
			{
				foreach (Player pl in PluginManager.Manager.Server.GetPlayers()
					.Where(p => SHPlugin.shPlayersInPocket.Contains(p.SteamId)))
				{
					pl.Kill();
				}
			}
		}

		public void OnCheckRoundEnd(CheckRoundEndEvent ev)
		{
		    if (disconnectOccured)
		    {
		        disconnectOccured = false;

		        string[] connectedSteamIds = ev.Server.GetPlayers().Select(x => x.SteamId).ToArray();
		        SHPlugin.shPlayers = SHPlugin.shPlayers.Where(x => connectedSteamIds.Contains(x)).ToList();
		    }

			bool MTFAlive = SHPlugin.CountRoles(Smod2.API.Team.NINETAILFOX) > 0;
			bool CiAlive = SHPlugin.CountRoles(Smod2.API.Team.CHAOS_INSURGENCY) > 0;
			bool ScpAlive = SHPlugin.CountRoles(Smod2.API.Team.SCP) > 0;
			bool DClassAlive = SHPlugin.CountRoles(Smod2.API.Team.CLASSD) > 0;
			bool ScientistsAlive = SHPlugin.CountRoles(Smod2.API.Team.SCIENTIST) > 0;
			bool SHAlive = SHPlugin.shPlayers.Count > 0;

			if (MTFAlive && (CiAlive || ScpAlive || DClassAlive || SHAlive))
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (CiAlive && (MTFAlive || (DClassAlive && ScpAlive) || ScientistsAlive || SHAlive))
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (ScpAlive && (MTFAlive || DClassAlive || ScientistsAlive))
				ev.Status = ROUND_END_STATUS.ON_GOING;
			else if (SHAlive && ScpAlive && !MTFAlive && !CiAlive && !DClassAlive && !ScientistsAlive)
				ev.Status = ROUND_END_STATUS.SCP_VICTORY;
			else if (CiAlive && ScpAlive && !SHPlugin.ciWinWithSCP)
				ev.Status = ROUND_END_STATUS.ON_GOING;
		}

		public void OnPocketDimensionEnter(PlayerPocketDimensionEnterEvent ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
				SHPlugin.shPlayersInPocket.Add(ev.Player.SteamId);
		}

		public void OnPocketDimensionDie(PlayerPocketDimensionDieEvent ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
			{
				if (!SHPlugin.friendlyFire)
					ev.Die = false;
				if (SHPlugin.teleportTo106)
					SHPlugin.TeleportTo106(ev.Player);
			}
		}

		public void OnPocketDimensionExit(PlayerPocketDimensionExitEvent ev)
		{
			if (SHPlugin.shPlayers.Contains(ev.Player.SteamId))
			{
				if (SHPlugin.teleportTo106)
					SHPlugin.TeleportTo106(ev.Player);
				SHPlugin.shPlayersInPocket.Remove(ev.Player.SteamId);
			}
		}

	    public void OnDisconnect(DisconnectEvent ev)
	    {
	        disconnectOccured = true;
	    }
	}
}
