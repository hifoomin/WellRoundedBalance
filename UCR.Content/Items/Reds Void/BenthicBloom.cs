using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.VoidReds
{
    public class BenthicBloom : ItemBase
    {
        public static int ItemCount;

        public override string Name => ":: Items :::::::: Void Reds :: Benthic Bloom";
        public override string InternalPickupToken => "cloverVoid";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>Upgrades " + ItemCount + "</style> <style=cStack>(+" + ItemCount + " per stack)</style> random items to items of the next <style=cIsUtility>higher rarity</style> at the <style=cIsUtility>start of each stage</style>. <style=cIsVoid>Corrupts all 57 Leaf Clovers</style>.";

        public override void Init()
        {
            ItemCount = ConfigOption(3, "Items to Corrupt", "Per Stack. Vanilla is 3");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterMaster.TryCloverVoidUpgrades += ChangeCount;
        }

        private void ChangeCount(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcI4(3)))
            {
                c.Next.Operand = ItemCount;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Benthic Bloom Count hook");
            }
        }
    }
}