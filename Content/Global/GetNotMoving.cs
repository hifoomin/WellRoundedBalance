using MonoMod.Cil;

namespace WellRoundedBalance.Global
{
    public class GetNotMoving : GlobalBase
    {
        public override string Name => ": Global ::::: Movement";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            //new ILHook(typeof(CharacterBody).GetMethod("get_isHealthLow"), ChangeTimer);
            IL.RoR2.CharacterBody.GetNotMoving += ChangeTimer;
        }

        public static void ChangeTimer(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(1f)))
            {
                c.Next.Operand = 0.5f;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Get Not Moving hook");
            }
        }
    }
}