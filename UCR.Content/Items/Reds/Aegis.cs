using R2API;
using RoR2;
using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class Aegis
    {
        public static void ChangeBarrierDecay(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            if (self.inventory)
            {
                var stack = self.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0 && Main.AegisDynamicBarrierDecay.Value)
                {
                    self.barrierDecayRate = Mathf.Max(1f, self.healthComponent.barrier / self.barrierDecayRate);
                }
            }
            orig(self);
        }
        // sender.barrierDecayRate didnt work SOOO lets try the dumb way ig
        public static void ChangeOverheal(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", "barrierOnOverHeal"),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.5f)
            );
            c.Index += 2;
            c.Next.Operand = Main.AegisOverhealPercent.Value;
        }
        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0)
                {
                    args.armorAdd += Main.AegisArmor.Value;
                }
            }
        }
    }
}
