using MonoMod.Cil;
using RoR2;
using UnityEngine;

namespace UltimateCustomRun.Items.Lunars
{
    public class GestureOfTheDrowned : ItemBase
    {
        public static float Cdr;
        public static float StackCdr;

        // public static float RandomCdDev;
        public static bool ShouldRun;

        private System.Random random = new();

        public override string Name => ":: Items ::::: Lunars :: Gesture of The Drowned";
        public override string InternalPickupToken => "autoCastEquipment";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>Reduce Equipment cooldown</style> by <style=cIsUtility>50%</style> <style=cStack>(+15% per stack)</style>. Forces your Equipment to <style=cIsUtility>activate</style> whenever it is off <style=cIsUtility>cooldown</style>.";

        public override void Init()
        {
            Cdr = ConfigOption(0.5f, "Base Equipment Cooldown Reduction", "Decimal. Vanilla is 0.5");
            StackCdr = ConfigOption(0.15f, "Stack Equipment Cooldown Reduction", "Decimal. Per Stack. Vanilla is 0.15");
            //  RandomCdDev = ConfigOption(0f, "Random Equipment Cooldown Deviation", "Decimal. Per Stack. Vanilla is 0");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.Inventory.CalculateEquipmentCooldownScale += ChangeCdr;
            // On.RoR2.EquipmentSlot.FixedUpdate += Fuck;
            //  On.RoR2.Inventory.CalculateEquipmentCooldownScale += Inventory_CalculateEquipmentCooldownScale;
        }

        /*
        private void Fuck(On.RoR2.EquipmentSlot.orig_FixedUpdate orig, EquipmentSlot self)
        {
            switch (self == null)
            {
                case false:
                    var stack = self.inventory.GetItemCount(RoR2Content.Items.AutoCastEquipment);
                    if (stack > 0 && self.characterBody.isEquipmentActivationAllowed)
                    {
                        ShouldRun = true;
                    }
                    else
                    {
                        ShouldRun = false;
                    }
                    break;

                default:
                    break;
            }
            orig(self);
        }

        private float Inventory_CalculateEquipmentCooldownScale(On.RoR2.Inventory.orig_CalculateEquipmentCooldownScale orig, Inventory self)
        {
            if (self != null && ShouldRun)
            {
                var stack = self.GetItemCount(RoR2Content.Items.AutoCastEquipment);
                return Mathf.Pow(random.Next(1, (int)RandomCdDev * 100) * 0.01f, stack);
            }
            else
            {
                return orig(self);
            }
        }
        */

        private void ChangeCdr(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                    x => x.MatchLdcR4(0.5f),
                    x => x.MatchLdcR4(0.85f)))
            {
                c.Next.Operand = Cdr;
                c.Index += 1;
                c.Next.Operand = 1f - StackCdr;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Gesture Of The Drowned Equipment Cooldown Reduction hook");
            }
        }
    }
}