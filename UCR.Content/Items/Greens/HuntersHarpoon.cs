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

        public override string DescText => "Killing an enemy increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(MoveSpeed * 5f) + "</style>, fading over <style=cIsUtility>" + Duration + "</style> <style=cStack>(+" + StackDuration + " per stack)</style> seconds.";
        public override string InternalPickupToken => "moveSpeedOnKill";
        public override string Name => ":: Items :: Greens :: Hunters Harpoon";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public static void ChangeMoveSpeed(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.25f),
                x => x.MatchLdarg(0),
                x => x.MatchLdsfld("RoR2.DLC1Content/Buffs", "KillMoveSpeed")
            );
            c.Next.Operand = MoveSpeed / 5f;
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeMoveSpeed;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDuration;
        }

        public override void Init()
        {
            MoveSpeed = ConfigOption(1.25f, "Move Speed Increase", "Decimal. Vanilla is 1.25");
            ROSOption("Greens", 0f, 3f, 0.05f, "2");
            Duration = ConfigOption(1f, "Base Duration", "Vanilla is 1");
            ROSOption("Greens", 0f, 5f, 0.1f, "2");
            StackDuration = ConfigOption(0.5f, "Stack Duration", "Per Stack. Vanilla is 0.5");
            ROSOption("Greens", 0f, 5f, 0.1f, "2");
            base.Init();
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.5f)
            );
            c.Next.Operand = Duration;
            c.Index += 3;
            c.Next.Operand = StackDuration;
        }
    }
}