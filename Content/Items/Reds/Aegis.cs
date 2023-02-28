using MonoMod.Cil;

namespace WellRoundedBalance.Items.Reds
{
    public class Aegis : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Aegis";
        public override string InternalPickupToken => "barrierOnOverheal";

        public override string PickupText => "Healing past full grants you a temporary barrier.";

        public override string DescText => (armorGainWithBarrier > 0 ? "<style=cIsHealing>Increase armor</style> by <style=cIsHealing>" + armorGainWithBarrier + "</style> while you have <style=cIsHealing>barrier</style>. " : "") +
                                           "Healing past full grants you a <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>" + d(overhealPercent) + " <style=cStack>(+" + d(overhealPercent) + " per stack)</style></style> of the amount you <style=cIsHealing>healed</style>.";

        [ConfigField("Overheal Percent", "Decimal.", 0.75f)]
        public static float overhealPercent;

        [ConfigField("Armor Gain With Barrier", "", 30f)]
        public static float armorGainWithBarrier;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            HealthComponent.onCharacterHealServer += HealthComponent_onCharacterHealServer;
        }

        private void HealthComponent_Heal(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", "barrierOnOverHeal"),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.5f)))
            {
                c.Index += 2;
                c.Next.Operand = overhealPercent;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Aegis Overheal hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventory = sender.inventory;
            if (inventory)
            {
                var stack = inventory.GetItemCount(RoR2Content.Items.BarrierOnOverHeal);
                if (stack > 0 && sender.healthComponent.barrier > 0f)
                {
                    args.armorAdd += armorGainWithBarrier;
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
    }
}