using MonoMod.Cil;

namespace UltimateCustomRun.Items.Lunars
{
    public class Corpsebloom : ItemBase
    {
        public static float HealingIncrease;
        public static float HealingOverTimeCap;

        public override string Name => ":: Items ::::: Lunars :: Corpsebloom";
        public override string InternalPickupToken => "repeatHeal";
        public override bool NewPickup => true;
        public override string PickupText => "Double your healing... <color=#FF7F7F>BUT it's applied over time.</color>";
        public override string DescText => "<style=cIsHealing>Heal +" + d(HealingIncrease - 1f) + "</style> <style=cStack>(+" + d(HealingIncrease - 1f) + " per stack)</style> more. <style=cIsHealing>All healing is applied over time</style>. Can <style=cIsHealing>heal</style> for a <style=cIsHealing>maximum</style> of <style=cIsHealing>" + d(HealingOverTimeCap) + "</style> <style=cStack>(-50% per stack)</style> of your <style=cIsHealing>health per second</style>.";

        public override void Init()
        {
            HealingIncrease = ConfigOption(2f, "Healing Increase", "Decimal. Per Stack. Vanilla is 2");
            HealingOverTimeCap = ConfigOption(0.1f, "Base Healing Cap", "Decimal. Vanilla is 0.1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.HealthComponent.Heal += Changes;
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdarg(1),
                    x => x.MatchLdcR4(2f),
                    x => x.MatchMul()))
            {
                c.Index += 1;
                c.Next.Operand = HealingIncrease;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Corpsebloom Heal Increase hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdfld<RoR2.HealthComponent>("repeatHealComponent"),
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Index += 1;
                c.Next.Operand = HealingOverTimeCap;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Corpsebloom Healing Cap hook");
            }
        }
    }
}