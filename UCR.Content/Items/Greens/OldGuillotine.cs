using MonoMod.Cil;
using UnityEngine;

namespace UltimateCustomRun.Items.Greens
{
    public class OldGuillotine : ItemBase
    {
        public static float Threshold;

        public override string Name => ":: Items :: Greens :: Old Guillotine";
        public override string InternalPickupToken => "executeLowHealthElite";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "Instantly kill Elite monsters below <style=cIsHealth>" + Mathf.Round((1f - 1f / (1f + (Threshold / 100f))) * 100f) + "% <style=cStack>(+" + Mathf.Round((1f - 1f / (1f + (Threshold * 2f / 100f))) * 100f - (1f - 1f / (1f + (Threshold / 100f))) * 100f) + "% per stack)</style> health</style>.";

        public override void Init()
        {
            Threshold = ConfigOption(13f, "Health Threshold", "Vanilla is 13");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.OnInventoryChanged += ChangeThreshold;
        }

        public static void ChangeThreshold(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(13f)))
            {
                c.Next.Operand = Threshold;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Old Guillotine Threshold hook");
            }
        }
    }
}