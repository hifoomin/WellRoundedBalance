using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Equipment
{
    public class RadarScanner : EquipmentBase
    {
        public override string Name => "::: Equipment :: Radar Scanner";
        public override string InternalPickupToken => "scanner";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>Reveal</style> all interactables within " + Range + "m for <style=cIsUtility>" + Duration + " seconds</style>.";

        public static float Range;
        public static float Duration;

        public override void Init()
        {
            Duration = ConfigOption(10f, "Duration", "Vanilla is 10");
            Range = ConfigOption(500f, "Radius", "Vanilla is 500");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var scanner = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Scanner/ChestScanner.prefab").WaitForCompletion().GetComponent<ChestRevealer>();
            scanner.radius = Range;
            scanner.revealDuration = Duration;
            scanner.pulseInterval = Duration;
            scanner.pulseTravelSpeed = Range / 4f;
        }
    }
}