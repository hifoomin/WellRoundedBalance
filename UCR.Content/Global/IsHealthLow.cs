/*
using MonoMod.Cil;
using RoR2;
using MonoMod.RuntimeDetour;

namespace UltimateCustomRun.Global
{
    public class IsHealthLow : GlobalBase
    {
        public static float lowhpthreshold;
        public override string Name => ": Global ::: Health";

        public override void Init()
        {
            lowhpthreshold = ConfigOption(0.25f, "Low Health Threshold", "Decimal. Used for Genesis Loop, Old War Stealthkit and the Low Health Visual. Vanilla is 0.25");
            base.Init();
        }
        public override void Hooks()
        {
            new ILHook(typeof(CharacterBody).GetMethod("get_isHealthLow"), ChangeThreshold);
        }
        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.25f)
            );
            c.Next.Operand = lowhpthreshold;
        }
    }
}
*/