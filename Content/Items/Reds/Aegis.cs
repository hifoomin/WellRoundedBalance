using Inferno.Stat_AI;
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

        public override string DescText => "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>30</style> while you have <style=cIsHealing>barrier</style>. Healing past full grants you a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>75% <style=cStack>(+75% per stack)</style></style> of the amount you <style=cIsHealing>healed</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.HealthComponent.Heal += ChangeOverheal;
            HealthComponent.onCharacterHealServer += HealthComponent_onCharacterHealServer;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (inventory)
            {
                var stack = inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0 && sender.healthComponent.barrier > 0f)
                {
                    args.armorAdd += 30f;
                }
            }
        }

        private void HealthComponent_onCharacterHealServer(HealthComponent healthComponent, float amount, ProcChainMask procChainMask)
        {
            var body = healthComponent.body;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    var stack = inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                    if (stack > 0 && healthComponent.barrier > 0f)
                    {
                        body.statsDirty = true;
                    }
                }
            }
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