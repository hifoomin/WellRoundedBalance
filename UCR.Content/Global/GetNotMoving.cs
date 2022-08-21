using MonoMod.Cil;

namespace UltimateCustomRun.Global
{
    public class GetNotMoving : GlobalBase
    {
        public static float NotMovingTimer;
        public override string Name => ": Global ::::: Movement";

        public override void Init()
        {
            NotMovingTimer = ConfigOption(1f, "Not Moving Timer", "Used for Bustling Fungus and modded items. Vanilla is 1.");
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
                c.Next.Operand = NotMovingTimer;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Get Not Moving hook");
            }
        }
    }
}