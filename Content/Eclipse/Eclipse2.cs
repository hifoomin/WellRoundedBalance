using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace WellRoundedBalance.Eclipse
{
    internal class Eclipse2 : GamemodeBase
    {
        // enemies predict your movement, pseudopulse please copy AccurateEnemies code im too lazy :trolley:
        public override string Name => ":: Gamemode : Eclipse";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HoldoutZoneController.FixedUpdate += HoldoutZoneController_FixedUpdate;
        }

        private void HoldoutZoneController_FixedUpdate(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = 1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Eclipse 2 hook");
            }
        }
    }
}