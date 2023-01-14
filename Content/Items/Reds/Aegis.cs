using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace WellRoundedBalance.Items.Reds
{
    public class Aegis : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Aegis";
        public override string InternalPickupToken => "barrierOnOverheal";

        public override string PickupText => "Healing past full grants you a temporary barrier.";

        public override string DescText => "Halve <style=cIsHealing>barrier decay</style>. Healing past full grants you a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>75% <style=cStack>(+75% per stack)</style></style> of the amount you <style=cIsHealing>healed</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            IL.RoR2.HealthComponent.Heal += ChangeOverheal;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            if (self.inventory)
            {
                var stack = self.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0)
                {
                    self.barrierDecayRate *= 0.5f;
                }
            }
            orig(self);
        }

        public static void ChangeOverheal(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", "barrierOnOverHeal"),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.5f)))
            {
                c.Index += 2;
                c.Next.Operand = 0.75f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Aegis Overheal hook");
            }
        }
    }
}