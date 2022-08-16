using MonoMod.Cil;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Equipment
{
    public class GnarledWoodsprite : EquipmentBase
    {
        public override string Name => "::: Equipment :: Gnarled Woodsprite";
        public override string InternalPickupToken => "passiveHealing";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Gain a Woodsprite follower that heals a friendly target for <style=cIsHealing>" + d(BurstHealing) + " of their maximum health</style> instantly, then <style=cIsHealing>" + d(PassiveHealing) + " of your maximum health</style> every second. \n\nActivating the equipment assigns a new target, or yourself if there are no targets available.";

        public static float BurstHealing;
        public static float PassiveHealing;
        public static float Interval;

        public override void Init()
        {
            BurstHealing = ConfigOption(0.1f, "Burst Percent Healing", "Decimal. Vanilla is 0.1");
            PassiveHealing = ConfigOption(0.015f, "Passive Percent Healing", "Decimal. Vanilla is 0.015");
            Interval = ConfigOption(1f, "Passive Healing Interval", "Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FirePassiveHealing += ChangeBurst;
            Changes();
        }

        private void ChangeBurst(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(0.1f)))
            {
                c.Next.Operand = BurstHealing;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Gnarled Woodsprite Burst Healing hook");
            }
        }

        private void Changes()
        {
            var wood = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/PassiveHealing/HealingFollower.prefab").WaitForCompletion().GetComponent<HealingFollowerController>();
            wood.fractionHealthBurst = BurstHealing;
            wood.fractionHealthHealing = PassiveHealing;
            wood.healingInterval = Interval;
        }
    }
}