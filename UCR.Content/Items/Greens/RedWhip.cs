using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Greens
{
    public class RedWhip : ItemBase
    {
        public static float Speed;
        public static float UnconditionalSpeed;
        public override string Name => ":: Items :: Greens :: Red Whip";
        public override string InternalPickupToken => "sprintOutOfCombat";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => (UnconditionalSpeed > 0 ? "Passively gain <style=cIsUtility>" + d(UnconditionalSpeed) + "</style> <style=cStack>(+" + d(UnconditionalSpeed) + " per stack)</style> movement speed." : "") +
                                           "Leaving combat boosts your <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(Speed) + "</style> <style=cStack>(+" + d(Speed) + " per stack)</style>.";

        public override void Init()
        {
            Speed = ConfigOption(0.3f, "Speed", "Decimal. Per Stack. Vanilla is 0.3");
            UnconditionalSpeed = ConfigOption(0f, "Unconditional Speed", "Decimal. Per Stack. Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
            RecalculateStatsAPI.GetStatCoefficients += AddBehavior;
        }

        private void AddBehavior(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory)
            {
                var stack = sender.inventory.GetItemCount(RoR2Content.Items.SprintOutOfCombat);
                if (stack > 0)
                {
                    args.moveSpeedMultAdd += UnconditionalSpeed * stack;
                }
            }
        }

        public static void ChangeSpeed(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("HasBuff"),
                    x => x.MatchBrfalse(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.3f)))
            {
                c.Index += 5;
                c.Next.Operand = Speed;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Red Whip Speed hook");
            }
        }
    }
}