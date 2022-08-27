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

        public override string PickupText => "Increase your health by " + d(HpIncrease) + (MassIncrease > 0 ? " and reduce knockback" : "") +
                                             "... <color=#FF7F7F>BUT reduce your speed by " + Mathf.Round(Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 100f)) + "%.</color>";

        public override string DescText => "Increase <style=cIsHealing>max health</style> by <style=cIsHealing>" + d(HpIncrease) + " <style=cStack>(+" + d(HpIncrease) + " per stack)</style></style>" + (MassIncrease > 0 ? " and reduce <style=cIsUtility>knockback</style>." : ".") +
                                           " Reduce <style=cIsUtility>movement speed</style> by <style=cIsUtility>" + Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 100f) + "%</style> <style=cStack>(+" + (Util.ConvertAmplificationPercentageIntoReductionPercentage(SpeedDecrease * 100f)) + "% per stack)</style>.";

        // slows aren't accurate in ror2
        public override void Init()
        {
            HpIncrease = ConfigOption(1f, "Max HP Increase", "Decimal. Per Stack. Vanilla is 1");
            SpeedDecrease = ConfigOption(1f, "Move Speed Decrease", "Decimal. Per Stack. Vanilla is 1");
            MassIncrease = ConfigOption(0f, "Mass Increase", "Per Stack. Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.CharacterBody.RecalculateStats += Changes;
            On.RoR2.CharacterBody.OnInventoryChanged += AddBehavior;
        }

        private void AddBehavior(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            self.AddItemBehavior<UCR_MassComponent>(self.inventory.GetItemCount(DLC1Content.Items.HalfSpeedDoubleHealth));
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

    public class UCR_MassComponent : CharacterBody.ItemBehavior
    {
        public CharacterMotor motor;
        public Inventory inv;

        public void Start()
        {
            motor = body.gameObject.GetComponent<CharacterMotor>();
            inv = body.master.GetComponent<Inventory>();
            if (motor != null)
            {
                motor.mass += StoneFluxPauldron.MassIncrease * stack;
            }
        }
    }
}