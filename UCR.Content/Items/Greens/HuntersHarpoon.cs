using MonoMod.Cil;
using R2API;
using RoR2;

namespace UltimateCustomRun.Items.Greens
{
    public class HuntersHarpoon : ItemBase
    {
        public static float MoveSpeed;
        public static float Duration;
        public static float StackDuration;

        public override string Name => ":: Items :: Greens :: Hunter's Harpoon";
        public override string InternalPickupToken => "moveSpeedOnKill";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Killing an enemy increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + d(MoveSpeed) + "</style>, fading over <style=cIsUtility>" + Duration + "</style> <style=cStack>(+" + StackDuration + " per stack)</style> seconds.";

        public override void Init()
        {
            MoveSpeed = ConfigOption(0.25f, "Move Speed Increase", "Decimal. Per Buff. Vanilla is 0.25");
            Duration = ConfigOption(1f, "Base Duration", "Vanilla is 1");
            StackDuration = ConfigOption(0.5f, "Stack Duration", "Per Stack. Vanilla is 0.5");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeMoveSpeed;
            IL.RoR2.GlobalEventManager.OnCharacterDeath += ChangeDuration;
        }

        public static void ChangeMoveSpeed(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.25f),
                x => x.MatchLdarg(0),
                x => x.MatchLdsfld<DLC1Content>("KillMoveSpeed")
            );
            c.Next.Operand = MoveSpeed;
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