using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using R2API;
using MonoMod.Cil;
using Mono.Cecil.Cil;

namespace UltimateCustomRun.Equipment
{
    public class HelfireTincture : EquipmentBase
    {
        public override string Name => "::: Equipment ::: Helfire Tincture";
        public override string InternalPickupToken => "burnNearby";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "<style=cIsDamage>Ignite</style> ALL characters within " + Radius + "m for " + Duration + " seconds. Deal <style=cIsDamage>" + d(SelfDamage * 5) + " of your maximum health/second as burning</style> to yourself. The burn is <style=cIsDamage>" + d(Mathf.Abs(AllyDamage - SelfDamage / Mathf.Abs(SelfDamage))) + "</style>" +
                                           (SelfDamage > AllyDamage ? " weaker" : " stronger") +
                                           " on allies, and <style=cIsDamage>" + d(Mathf.Abs(EnemyDamage - SelfDamage / Mathf.Abs(SelfDamage))) + "</style>" +
                                           (SelfDamage > EnemyDamage ? " weaker" : " stronger") +
                                            " on enemies.";

        public static float SelfDamage;
        public static float AllyDamage;
        public static float EnemyDamage;

        // public static float FireRate;
        public static float Radius;

        public static float Duration;
        public static float DoTDuration;

        public override void Init()
        {
            SelfDamage = ConfigOption(0.005f, "Percent Health Damage To Self", "Decimal. Vanilla is 0.005");
            AllyDamage = ConfigOption(0.0025f, "Percent Health Damage To Allies", "Decimal. Vanilla is 0.0025");
            EnemyDamage = ConfigOption(0.24f, "Percent Health Damage To Enemies", "Decimal. Vanilla is 0.24");
            Radius = ConfigOption(15f, "Radius", "Vanilla is 15");
            Duration = ConfigOption(12f, "Duration", "Vanilla is 12");
            DoTDuration = ConfigOption(3f, "Burn Duration", "Vanilla is 3");
            // Interval = ConfigOption(5f, "Fire Rate", "Vanilla is 5);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireBurnNearby += ChangeDuration;
            Changes();
        }

        private void ChangeDuration(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(12f)))
            {
                c.Next.Operand = Duration;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Helfire Tincture Duration hook");
            }
        }

        private void Changes()
        {
            var hel = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BurnNearby/HelfireController.prefab").WaitForCompletion().GetComponent<HelfireController>();
            hel.baseRadius = Radius;
            hel.dotDuration = DoTDuration;
            //hel.interval = FireRate;
            // how the hell does this work, the interval is literally 0.25 in game and it attacks 5x per sec???
            hel.healthFractionPerSecond = SelfDamage * 10f;
            hel.allyDamageScalar = 1f / (SelfDamage * 10f / (AllyDamage * 10f));
            hel.enemyDamageScalar = EnemyDamage * 100f;
        }
    }
}