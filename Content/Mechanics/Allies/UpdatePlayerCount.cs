using MonoMod.RuntimeDetour;
using R2API.Utils;
using System.Runtime.CompilerServices;
using System;

namespace WellRoundedBalance.Mechanics.Allies
{
    internal class UpdatePlayerCount : MechanicBase<UpdatePlayerCount>
    {
        public override string Name => ":: Mechanics ::::::::::::::: Update Player Count";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var getParticipatingPlayerCount = new Hook(typeof(Run).GetMethodCached("get_participatingPlayerCount"),
                typeof(UpdatePlayerCount).GetMethodCached(nameof(GetParticipatingPlayerCountHook)));
        }

        private static int GetParticipatingPlayerCountHook(Run self)
        {
            return GetConnectedPlayers();
        }

        private static int GetConnectedPlayers()
        {
            int players = 0;
            foreach (PlayerCharacterMasterController pc in PlayerCharacterMasterController.instances)
            {
                if (pc.isConnected)
                {
                    players++;
                }
            }
            if (Main.WildbookMultitudesLoaded)
            {
                players = ApplyMultitudes(players);
            }
            if (Main.ZetArtifactsLoaded)
            {
                players = ApplyZetMultitudesArtifact(players);
            }

            return players;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static int ApplyMultitudes(int origPlayerCount)
        {
            return origPlayerCount * Multitudes.Multitudes.Multiplier;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static int ApplyZetMultitudesArtifact(int origPlayerCount)
        {
            if (TPDespair.ZetArtifacts.ZetMultifact.Enabled && RunArtifactManager.instance.IsArtifactEnabled(TPDespair.ZetArtifacts.ZetArtifactsContent.Artifacts.ZetMultifact))
            {
                return origPlayerCount * Math.Max(2, TPDespair.ZetArtifacts.ZetArtifactsPlugin.MultifactMultiplier.Value); //GetMultiplier is private so I copypasted the code.
            }
            else
            {
                return origPlayerCount;
            }
        }
    }
}