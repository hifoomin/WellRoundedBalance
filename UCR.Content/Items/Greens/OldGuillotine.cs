using UnityEngine;
using MonoMod.Cil;

namespace UltimateCustomRun
{
    public class OldGuillotine : ItemBase
    {
        public static float threshold;

        public override string Name => ":: Items :: Greens :: Old Guillotine";
        public override string InternalPickupToken => "executeLowHealthElite";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Instantly kill Elite monsters below <style=cIsHealth>" + Mathf.Round(1f - 1f / (1f + threshold)) * 100f + "% <style=cStack>(+" + Mathf.Round((1f - 1f / (1f + threshold* 2f))) * 100f + "% per stack)</style> health</style>.";


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
