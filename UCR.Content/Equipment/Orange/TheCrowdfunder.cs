using MonoMod.Cil;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;

namespace UltimateCustomRun.Equipment
{
    public class TheCrowdfunder : EquipmentBase
    {
        public override string Name => "::: Equipment :: The Crowdfunder";
        public override string InternalPickupToken => "goldGat";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Wind up a continuous barrage that shoots up to <style=cIsDamage>" + MaxShotsPerSec + " times per second</style>, dealing <style=cIsDamage>" + d(Damage) + " damage per shot</style> (extremely low). Costs $" + GoldCost + " per bullet. Cost increases with level.";

        public static int GoldCost;
        public static float MinShotsPerSec;
        public static float MaxShotsPerSec;
        public static float WindUpDuration;
        public static float ProcCoefficient;
        public static float Damage;

        public override void Init()
        {
            GoldCost = ConfigOption(1, "Gold Cost", "Vanilla is 1");
            MinShotsPerSec = ConfigOption(3f, "Minimum Shots Per Second", "Vanilla is 3");
            MaxShotsPerSec = ConfigOption(15f, "Maximum Shots Per Second", "Vanilla is 15");
            WindUpDuration = ConfigOption(10f, "Wind Up Duration", "Vanilla is 10");
            ProcCoefficient = ConfigOption(1f, "Proc Coefficient", "Vanilla is 1");
            Damage = ConfigOption(1f, "Damage", "Decimal. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.GoldGat.GoldGatFire.OnEnter += Changes;
        }

        private void Changes(On.EntityStates.GoldGat.GoldGatFire.orig_OnEnter orig, EntityStates.GoldGat.GoldGatFire self)
        {
            EntityStates.GoldGat.GoldGatFire.windUpDuration = WindUpDuration;
            EntityStates.GoldGat.GoldGatFire.minFireFrequency = MinShotsPerSec;
            EntityStates.GoldGat.GoldGatFire.maxFireFrequency = MaxShotsPerSec;
            EntityStates.GoldGat.GoldGatFire.baseMoneyCostPerBullet = GoldCost;
            EntityStates.GoldGat.GoldGatFire.procCoefficient = ProcCoefficient;
            EntityStates.GoldGat.GoldGatFire.damageCoefficient = Damage;
            orig(self);
        }
    }
}