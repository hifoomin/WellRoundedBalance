using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Greens
{
    public class HuntersHarpoon : ItemBase
    {
        public static float Duration;
        public static float MoveSpeed;
        public static float StackDuration;

        public override string DescText => "Killing an enemy increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(MoveSpeed) + "</style>, fading over <style=cIsUtility>" + Duration + "</style> <style=cStack>(+" + StackDuration + " per stack)</style> seconds.";
        public override string InternalPickupToken => "moveSpeedOnKill";
        public override string Name => ":: Items :: Greens :: Hunters Harpoon";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeMoveSpeed;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDuration;
        }

        public override void Init()
        {
            MoveSpeed = ConfigOption(1.25f, "Move Speed Increase", "Decimal. Vanilla is 1.25");
            Duration = ConfigOption(1f, "Base Duration", "Vanilla is 1");
            StackDuration = ConfigOption(0.5f, "Stack Duration", "Per Stack. Vanilla is 0.5");
            base.Init();
        }

        public static void ChangeMoveSpeed(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.25f),
                    x => x.MatchLdarg(0),
                    x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "KillMoveSpeed")))
            {
                c.Next.Operand = MoveSpeed / 5f;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Hunter's Harpoon Move Speed Increase hook");
            }
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f),
                    x => x.MatchLdloc(out _),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(0.5f)))
            {
                c.Next.Operand = Duration;
                c.Index += 3;
                c.Next.Operand = StackDuration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Hunter's Harpoon Duration hook");
            }
        }
    }
}