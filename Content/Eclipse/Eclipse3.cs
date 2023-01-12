using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace WellRoundedBalance.Eclipse
{
    internal class Eclipse3 : GamemodeBase
    {
        // look at Elites folder, though I have no idea what to do for overloading
        public override string Name => ":: Gamemode : Eclipse";

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
                x => x.MatchLdcR4(2f)))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Eclipse 3 hook");
            }
        }
    }
}