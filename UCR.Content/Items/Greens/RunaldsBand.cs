using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    public static class RunaldsBand
    {
        public static void RunaldChange(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int itemCountLocation = 51;
            int totalDamageMultiplierLocation = 56;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "IceRing"),
                x => x.MatchCallOrCallvirt<Inventory>(nameof(Inventory.GetItemCount)),
                x => x.MatchStloc(out itemCountLocation)
                );

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f),
                x => x.MatchLdloc(itemCountLocation),
                x => x.MatchConvR4(),
                x => x.MatchMul(),
                x => x.MatchStloc(out totalDamageMultiplierLocation)
                );
            c.Remove();
            c.Emit(OpCodes.Ldc_R4, Main.RunaldTotalDamage.Value);

            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(totalDamageMultiplierLocation),
                x => x.MatchCallOrCallvirt(out _)
                );
            c.Emit(OpCodes.Ldloc_0);
            c.EmitDelegate<Func<float, CharacterBody, float>>((damage, self) =>
            {
                float dam = self.baseDamage * Main.RunaldBaseDamage.Value;

                return damage + dam;
            });
        }
        // PLEASE HELP TO FIX
    }
}
