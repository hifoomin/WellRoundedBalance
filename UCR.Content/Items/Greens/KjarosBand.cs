using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public static class KjarosBand
    {
        public static void KjaroChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int itemCountLocation = 51;
            int totalDamageMultiplierLocation = 56;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "FireRing"),
                x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)),
                x => x.MatchStloc(out itemCountLocation)
                );

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(2.5f),
                x => x.MatchLdloc(itemCountLocation),
                x => x.MatchConvR4(),
                x => x.MatchMul(),
                x => x.MatchStloc(out totalDamageMultiplierLocation)
                );
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, Main.KjaroTotalDamage);

            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(totalDamageMultiplierLocation),
                x => x.MatchCallOrCallvirt(out _)
                );
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((damage, self) =>
            {
                float dam = self.baseDamage * Main.KjaroBaseDamage.Value;

                return damage + dam;
            });
        }
        // THIS BREAKS ALL MY OTHER IL PLEASE HELP TO FIX
    }
}
