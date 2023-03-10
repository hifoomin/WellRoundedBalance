using MonoMod.Cil;

namespace WellRoundedBalance.Items.Lunars
{
    public class Corpsebloom : ItemBase
    {
        public override string Name => ":: Items ::::: Lunars :: Corpsebloom";
        public override string InternalPickupToken => "repeatHeal";

        public override string PickupText => "Increase your healing... <color=#FF7F7F>BUT it's applied over time.</color>";
        public override string DescText => "<style=cIsHealing>Heal " + d(healingIncrease) + "</style> <style=cStack>(+" + d(healingIncrease) + " per stack)</style> more. <style=cIsHealing>All healing is applied over time</style>. Can <style=cIsHealing>heal</style> for a <style=cIsHealing>maximum</style> of <style=cIsHealing>" + d(percentHealingCapPerSecond) + "</style> <style=cStack>(-50% per stack)</style> of your <style=cIsHealing>health per second</style>.";

        [ConfigField("Healing Increase", "Decimal.", 0.5f)]
        public static float healingIncrease;

        [ConfigField("Percent Healing Cap Per Second", "Decimal.", 0.07f)]
        public static float percentHealingCapPerSecond;

        public override void Init()
        {
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
                c.Next.Operand = 1f + healingIncrease;
            }
            else
            {
                Logger.LogError("Failed to apply Corpsebloom Heal Increase hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdfld<HealthComponent>("repeatHealComponent"),
                    x => x.MatchLdcR4(0.1f)))
            {
                c.Index += 1;
                c.Next.Operand = percentHealingCapPerSecond;
            }
            else
            {
                Logger.LogError("Failed to apply Corpsebloom Healing Cap hook");
            }
        }
    }
}