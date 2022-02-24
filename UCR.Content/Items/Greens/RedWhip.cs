using MonoMod.Cil;

namespace UltimateCustomRun.Items.Greens
{
    public class RedWhip : ItemBase
    {
        public static float Speed;

        public override string Name => ":: Items :: Greens :: Red Whip";
        public override string InternalPickupToken => "sprintOutOfCombat";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "Leaving combat boosts your <style=cIsUtility>movement Speed</style> by <style=cIsUtility>" + d(Speed) + "</style> <style=cStack>(+" + d(Speed) + " per stack)</style>.";

        public override void Init()
        {
            Speed = ConfigOption(0.3f, "Speed Increase", "Decimal. Per Stack. Vanilla is 0.3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += ChangeSpeed;
        }

        public static void ChangeSpeed(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchCallOrCallvirt<RoR2.CharacterBody>("HasBuff"),
                x => x.MatchBrfalse(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchLdloc(out _),
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.3f)
            );
            c.Index += 5;
            c.Next.Operand = Speed;
        }
    }
}