using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Equipment
{
    public class RadarScanner : EquipmentBase
    {
        public override string Name => "::: Equipment :: Radar Scanner";
        public override string InternalPickupToken => "scanner";

        public override string PickupText => "Reveal all nearby interactables.";

        public override string DescText => "<style=cIsUtility>Reveal</style> all interactables within 1000m for <style=cIsUtility>10 seconds</style>.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var scanner = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Scanner/ChestScanner.prefab").WaitForCompletion().GetComponent<ChestRevealer>();
            scanner.radius = 1000f;
            scanner.pulseTravelSpeed = 250f;
        }
    }
}