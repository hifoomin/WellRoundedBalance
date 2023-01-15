using HG;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Text;

namespace WellRoundedBalance.Items.Whites
{
    internal class Gasoline : ItemBase
    {
        public override string Name => ":: Items : Whites :: Gasoline";

        public override string InternalPickupToken => "igniteOnKill";

        public override string PickupText => "Killing an enemy ignites other nearby enemies.";

        public override string DescText => "Killing an enemy <style=cIsDamage>ignites</style> all enemies within <style=cIsDamage>12m</style> for <style=cIsDamage>100%</style> base damage. Additionally, enemies <style=cIsDamage>burn</style> for <style=cIsDamage>200%</style> <style=cStack>(+200% per stack)</style> base damage.";

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.ProcIgniteOnKill += GlobalEventManager_ProcIgniteOnKill;
        }

        private void GlobalEventManager_ProcIgniteOnKill(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(1),
                    x => x.MatchLdarg(1),
                    x => x.MatchAdd(),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.75f)))
            {
                // c.Next.Operand = 1;
                c.Index += 4;
                c.Next.Operand = 1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Gasoline Burn Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchAdd(),
               x => x.MatchStloc(1),
               x => x.MatchLdcR4(1.5f)))
            {
                c.Index += 2;
                c.Next.Operand = 1f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Gasoline Explosion Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(8f),
                x => x.MatchLdcR4(4f)))
            {
                c.Next.Operand = 12f;
                c.Index += 1;
                c.Next.Operand = 0f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Gasoline Radius hook");
            }
        }
    }
}