using MonoMod.Cil;
using RoR2;
using MonoMod.RuntimeDetour;

namespace UltimateCustomRun.Global
{
    public class IsHealthLow : GlobalBase
    {
        public static float Threshold;
        public override string Name => ": Global ::: Health";

        public override void Init()
        {
            Threshold = ConfigOption(0.25f, "Low Health Threshold", "Decimal. Used for Old War Stealthkit, Genesis Loop and modded items. Vanilla is 0.25");
            base.Init();
        }

        public override void Hooks()
        {
            // new ILHook(typeof(CharacterBody).GetMethod("get_isHealthLow"), ChangeThreshold);
            On.RoR2.HealthComponent.Awake += ChangeThreshold;
        }

        private void ChangeThreshold(On.RoR2.HealthComponent.orig_Awake orig, HealthComponent self)
        {
            HealthComponent.lowHealthFraction = Threshold;
            orig(self);
        }

        /*
        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.25f)
            );
            c.Next.Operand = Threshold;
        }
        */
    }
}