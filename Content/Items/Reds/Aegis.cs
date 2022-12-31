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

        public override string DescText => "Halve <style=cIsHealing>barrier decay</style>. Healing past full grants you a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>50% <style=cStack>(+50% per stack)</style></style> of the amount you <style=cIsHealing>healed</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.CharacterBody.FixedUpdate += ChangeBarrierDecay;
        }

        public static void ChangeBarrierDecay(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            if (self.inventory)
            {
                var stack = self.inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0)
                {
                    self.barrierDecayRate /= 2f;
                }
            }
            orig(self);
        }
    }
}