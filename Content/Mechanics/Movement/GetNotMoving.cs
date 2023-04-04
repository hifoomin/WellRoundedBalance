using MonoMod.Cil;

namespace WellRoundedBalance.Mechanics.Movement
{
    public class GetNotMoving : MechanicBase<GetNotMoving>
    {
        public override string Name => ":: Mechanics ::: Movement";

        [ConfigField("Not Moving Interval", "Affects all other mods using it!", 0.5f)]
        public static float notMovingInterval;

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
                c.Next.Operand = notMovingInterval;
            }
            else
            {
                Logger.LogError("Failed to apply Get Not Moving hook");
            }
        }
    }
}