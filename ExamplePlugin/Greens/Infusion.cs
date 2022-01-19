using R2API;
using RoR2;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;

namespace UltimateCustomRun
{
    static class Infusion
    {
        public static void ChangeBehavior(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            int bodyLoc = 17;
            int countLoc = 33;
            int capLoc = 47;

            c.GotoNext(MoveType.After,
                x => x.MatchLdsfld("RoR2.RoR2Content/Items", "Infusion"),
                x => x.MatchCallOrCallvirt<RoR2.Inventory>(nameof(RoR2.Inventory.GetItemCount)),
                x => x.MatchStloc(out countLoc)
                );
            c.GotoNext(MoveType.Before,
                x => x.MatchStloc(out capLoc)
                );
            c.Emit(OpCodes.Ldloc, countLoc);
            c.Emit(OpCodes.Ldloc, 13);
            c.EmitDelegate<Func<int, int, RoR2.CharacterBody, int>>((currentInfusionCap, infusionCount, body) =>
            {
                float newInfusionCap = 100 * infusionCount;

                if (body != null)
                {
                    float levelBonus = 1 + 0.3f * (body.level - 1);

                    newInfusionCap = Main.InfusionBaseCap.Value * levelBonus * infusionCount;
                }

                return (int)newInfusionCap;
            });
        }
        public static void BehaviorAddFlatHealth(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Infusion);
                if (stack > 0)
                {
                    args.baseHealthAdd += Main.InfusionBaseHealth.Value * stack;
                }
            }
        }

        public static void BehaviorAddPercentHealth(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.Infusion);
                if (stack > 0)
                {
                    args.healthMultAdd += Main.InfusionPercentHealth.Value * stack;
                }
            }
        }
    }
}
