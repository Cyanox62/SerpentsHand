using Exiled.API.Features;
using HarmonyLib;
using UnityEngine;

namespace SerpentsHand.Patches
{
	[HarmonyPatch(typeof(PlayableScps.Scp939), nameof(PlayableScps.Scp939.ServerAttack))]
	class Scp939Attack
	{
		public static void Postfix(PlayableScps.Scp939 __instance, GameObject target)
		{
			Player player = Player.Get(target);
			if (player != null && EventHandlers.shPlayers.Contains(player.Id) && !SerpentsHand.instance.Config.FriendlyFire)
			{
				player.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Amnesia>();
			}
		}
	}

	//[HarmonyPatch(typeof(Scp939PlayerScript), nameof(Scp939PlayerScript.UserCode_CmdShoot))]
	//class Scp939Attack
	//{
	//	public static void Postfix(Scp939PlayerScript __instance, GameObject target)
	//	{
	//		Player player = Player.Get(target);
	//		if (player != null && EventHandlers.shPlayers.Contains(player.Id) && !SerpentsHand.instance.Config.FriendlyFire)
	//		{
	//			player.ReferenceHub.playerEffectsController.DisableEffect<CustomPlayerEffects.Amnesia>();
	//		}
	//	}
	//}
}
