using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class PaulsGoatHoof : Based
    {
        public static float speed;

        public override string Name => ":: Items : Whites :: Pauls Goat Hoof";
        public override string InternalPickupToken => "hoof";
        public override bool NewPickup => false;

        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>Sprint speed</style> is improved by <style=cIsUtility>" + d(speed) + "</style> <style=cStack>(+" + d(speed) + " per stack)</style>.";
        public override void Init()
        {
            speed = ConfigOption(0.14f, "Speed Increase", "Decimal. Per Stack. Vanilla is 0.14");
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
                x => x.MatchConvR4(),
                x => x.MatchLdcR4(0.14f)
            );
            c.Index += 1;
            c.Next.Operand = speed;
        }
    }
}
