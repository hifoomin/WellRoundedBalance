using MonoMod.Cil;

namespace UltimateCustomRun
{
    public static class Headstompers
    {
        public static void Changes()
        {
            On.EntityStates.Headstompers.HeadstompersCooldown.OnEnter += (orig, self) =>
            {
                EntityStates.Headstompers.HeadstompersCooldown.baseDuration = Main.HeadstompersCooldown.Value;
                orig(self);
            };
            On.EntityStates.Headstompers.HeadstompersFall.OnEnter += (orig, self) =>
            {
                EntityStates.Headstompers.HeadstompersFall.minimumRadius = Main.HeadstompersMinRange.Value;
                EntityStates.Headstompers.HeadstompersFall.maximumRadius = Main.HeadstompersMaxRange.Value;
                EntityStates.Headstompers.HeadstompersFall.minimumDamageCoefficient = Main.HeadstompersMinDamage.Value;
                EntityStates.Headstompers.HeadstompersFall.maximumDamageCoefficient = Main.HeadstompersMaxDamage.Value;
                orig(self);
            };
        }
        public static void ChangeJumpHeight(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(2f)
            );
            c.Next.Operand = Main.HeadstompersJumpHeight.Value;
        }
    }
}
