using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Greens
{
    public class Chronobauble : ItemBase
    {
        public static float BaseAttackSpeed;
        public static bool StackAttackSpeed;
        // :trollge:

        public override string Name => ":: Items :: Greens :: Chronobauble";
        public override string InternalPickupToken => "slowOnHit";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>Slow</style> enemies on hit for <style=cIsUtility>-60% movement speed</style>" +
                                           (BaseAttackSpeed != 0f ? " and <style=cIsDamage>-" + d(BaseAttackSpeed) + " attack speed</style>" +
                                           (StackAttackSpeed ? " <style=cStack>(-" + d(BaseAttackSpeed) + " per stack)</style>" : "") : "") +
                                           " for <style=cIsUtility>2s</style> <style=cStack>(+2s per stack)</style>.";

        public override void Init()
        {
            BaseAttackSpeed = ConfigOption(0f, "Attack Speed Decrease", "Decimal. Vanilla is 0");
            StackAttackSpeed = ConfigOption(false, "Stack Attack Speed Decrease?", "Vanilla is false");
            base.Init();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        public static void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var debuff = sender.HasBuff(RoR2Content.Buffs.Slow50);
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SlowOnHit);
                if (stack > 0 && sender && debuff)
                {
                    args.attackSpeedMultAdd -= StackAttackSpeed ? BaseAttackSpeed * stack : BaseAttackSpeed;
                }
            }
        }
    }
}