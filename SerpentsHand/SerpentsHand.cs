using Exiled.API.Features;
using Exiled.Loader;
using HarmonyLib;
using System.Reflection;
using EPlayer = Exiled.Events.Handlers.Player;
using EServer = Exiled.Events.Handlers.Server;

namespace SerpentsHand
{
    public class SerpentsHand : Plugin<Config>
    {
        public EventHandlers EventHandlers;

        public static SerpentsHand instance;
        private Harmony hInstance;

        public static bool isScp035 = false;

        public override void OnEnabled()
        {
            base.OnEnabled();

            if (!Config.IsEnabled) return;

            hInstance = new Harmony("cyanox.serpentshand");
            hInstance.PatchAll();

            instance = this;
            EventHandlers = new EventHandlers();
            Check035();

            EPlayer.EnteringPocketDimension += EventHandlers.OnPocketDimensionEnter;
            EPlayer.FailingEscapePocketDimension += EventHandlers.OnPocketDimensionDie;
            EPlayer.EscapingPocketDimension += EventHandlers.OnPocketDimensionExit;
            EPlayer.Dying += EventHandlers.OnPlayerDying;
            EPlayer.Hurting += EventHandlers.OnPlayerHurt;
            EPlayer.ChangingRole += EventHandlers.OnSetRole;
            EPlayer.Left += EventHandlers.OnDisconnect;
            Exiled.Events.Handlers.Scp106.Containing += EventHandlers.OnContain106;
            EPlayer.InsertingGeneratorTablet += EventHandlers.OnGeneratorInsert;
            EPlayer.EnteringFemurBreaker += EventHandlers.OnFemurEnter;
            EPlayer.Died += EventHandlers.OnPlayerDeath;
            EPlayer.Shooting += EventHandlers.OnShoot;
            EPlayer.Spawning += EventHandlers.OnSpawn;
            EServer.SendingRemoteAdminCommand += EventHandlers.OnRACommand;
            EServer.RoundStarted += EventHandlers.OnRoundStart;
            EServer.RespawningTeam += EventHandlers.OnTeamRespawn;
            EServer.EndingRound += EventHandlers.OnCheckRoundEnd;
        }

        public override void OnDisabled()
        {
            base.OnDisabled();

            EPlayer.EnteringPocketDimension -= EventHandlers.OnPocketDimensionEnter;
            EPlayer.FailingEscapePocketDimension -= EventHandlers.OnPocketDimensionDie;
            EPlayer.EscapingPocketDimension -= EventHandlers.OnPocketDimensionExit;
            EPlayer.Dying -= EventHandlers.OnPlayerDying;
            EPlayer.Hurting -= EventHandlers.OnPlayerHurt;
            EPlayer.ChangingRole -= EventHandlers.OnSetRole;
            EPlayer.Left -= EventHandlers.OnDisconnect;
            Exiled.Events.Handlers.Scp106.Containing -= EventHandlers.OnContain106;
            EPlayer.InsertingGeneratorTablet -= EventHandlers.OnGeneratorInsert;
            EPlayer.EnteringFemurBreaker -= EventHandlers.OnFemurEnter;
            EPlayer.Died -= EventHandlers.OnPlayerDeath;
            EPlayer.Shooting -= EventHandlers.OnShoot;
            EPlayer.Spawning -= EventHandlers.OnSpawn;
            EServer.SendingRemoteAdminCommand -= EventHandlers.OnRACommand;
            EServer.RoundStarted -= EventHandlers.OnRoundStart;
            EServer.RespawningTeam -= EventHandlers.OnTeamRespawn;
            EServer.EndingRound -= EventHandlers.OnCheckRoundEnd;

            hInstance.UnpatchAll();
            EventHandlers = null;
        }

        public override string Name => "SerpentsHand";
        public override string Author => "Cyanox";

        internal void Check035()
        {
            foreach (var plugin in Loader.Plugins)
            {
                if (plugin.Name == "scp035")
                {
                    isScp035 = true;
                    return;
                }
            }
        }
    }
}
