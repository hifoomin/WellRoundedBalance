using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class OldGuillotine : Based
    {
        public static float threshold;

        public override string Name => ":: Items :: Greens :: Old Guillotine";
        public override string InternalPickupToken => "executeLowHealthElite";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public static float actualthreshold = (1f - 100f / (100f + threshold)) * 100f;
        public static float actualthresholdtwo = (1f - 100f / (100f + threshold * 2f)) * 100f;
        public static float firststack = Mathf.Round(actualthreshold);
        public static float secondstack = Mathf.Round(actualthresholdtwo);

        public override string DescText => "Instantly kill Elite monsters below <style=cIsHealth>" + firststack + "% <style=cStack>(+" + secondstack + "% per stack)</style> health</style>.";


        public override void Init()
        {
            threshold = ConfigOption(13f, "Threshold", "Vanilla is 13");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.OnInventoryChanged += ChangeThreshold;
        }
        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new ILCursor(il);

            c.GotoNext(MoveType.Before,
                x => x.MatchLdcR4(13f)
            );
            c.Next.Operand = threshold;
        }
    }
}
