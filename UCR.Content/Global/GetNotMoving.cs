using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public class GetNotMoving : GlobalBase
    {
        public static float NotMovingTimer;
        public override string Name => ": Global ::::: Movement";

        public override void Init()
        {
            NotMovingTimer = ConfigOption(1f, "Not Moving Timer", "Used for Bustling Fungus. Vanilla is 1.\nRecommended Value: 0.5");
            base.Init();
        }

        public override void Hooks()
        {
            //new ILHook(typeof(CharacterBody).GetMethod("get_isHealthLow"), ChangeTimer);
            IL.RoR2.CharacterBody.GetNotMoving += ChangeTimer;
        }

        public static void ChangeTimer(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(1f)
            );
            c.Next.Operand = NotMovingTimer;
        }
    }
}