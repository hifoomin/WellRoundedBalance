using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Lunars
{
    public class StoneFluxPauldron : ItemBase
    {
        public static float HpIncrease;
        public static float SpeedDecrease;
        public static float MassIncrease;

        public override string Name => ":: Items ::::: Lunars :: Stone Flux Pauldron";
        public override string InternalPickupToken => "halfSpeedDoubleHealth";
        public override bool NewPickup => true;
        public override string PickupText => "Increase your health by " + d(HpIncrease) + "... <color=#FF7F7F>BUT reduce your speed by " + Mathf.Round(Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 100f)) + "%.</color>";
        public override string DescText => "Increase <style=cIsHealing>max health</style> by <style=cIsHealing>" + d(HpIncrease) + " <style=cStack>(+" + d(HpIncrease) + " per stack)</style></style>. Reduce <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 100f) + "%</style> <style=cStack>(+" + Mathf.Round((Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 500f) - (Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 100f)))) + "% per stack)</style>.";

        public override void Init()
        {
            HpIncrease = ConfigOption(1f, "Max HP Increase", "Decimal. Per Stack. Vanilla is 1");
            SpeedDecrease = ConfigOption(1f, "Move Speed Decrease", "Decimal. Per Stack. Vanilla is 1");
            MassIncrease = ConfigOption(0f, "Knockback Resistance", "Per Stack. Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += AddMass;
        }

        private void AddMass(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            if (self != null && self.gameObject.GetComponent<CharacterMaster>() != null && self.gameObject.GetComponent<CharacterMaster>().GetBody() != null)
            {
                var stack = self.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth);
                if (stack > 0 && itemIndex == DLC1Content.Items.HalfSpeedDoubleHealth.itemIndex)
                {
                    self.gameObject.GetComponent<CharacterMotor>().mass += MassIncrease * stack;
                }
            }
            orig(self, itemIndex, count);
        }

        private void Changes(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdloc(76),
                    x => x.MatchLdloc(44),
                    x => x.MatchConvR4(),
                    x => x.MatchLdcR4(1f)))
            {
                c.Index += 3;
                c.Next.Operand = SpeedDecrease;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Stone Flux Pauldron Speed hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
               x => x.MatchLdloc(63),
               x => x.MatchLdloc(44),
               x => x.MatchConvR4(),
               x => x.MatchLdcR4(1f)))
            {
                c.Index += 3;
                c.Next.Operand = HpIncrease;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Stone Flux Pauldron Health hook");
            }
        }
    }
}