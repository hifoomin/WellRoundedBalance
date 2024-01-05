using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Gamemodes.Eclipse
{
    internal class Eclipse3 : GamemodeBase<Eclipse3>
    {
        // look at Elites folder
        public override string Name => ":: Gamemode : Eclipse 3";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnCharacterHitGroundServer += GlobalEventManager_OnCharacterHitGroundServer;
        }

        private void GlobalEventManager_OnCharacterHitGroundServer(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcI4(5)))
            {
                c.Index++;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return int.MaxValue;
                });
            }
            else
            {
                Logger.LogError("Failed to apply Eclipse 3 hook");
            }
        }

        internal static bool CheckEclipse()
        {
            return instance.isEnabled && Run.instance && Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3;
        }
    }
}