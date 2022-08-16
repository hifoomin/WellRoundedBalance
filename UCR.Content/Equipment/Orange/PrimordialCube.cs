using MonoMod.Cil;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltimateCustomRun.Equipment
{
    public class PrimordialCube : EquipmentBase
    {
        public override string Name => "::: Equipment :: Primordial Cube";
        public override string InternalPickupToken => "blackHole";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Fire a black hole that <style=cIsUtility>draws enemies within " + Range + "m" +
            (Force > 0 ? " outside of" : " into") +
            " its center</style>. Lasts " + Duration + " seconds.";

        public static float Duration;
        public static float Range;
        public static float Speed;
        public static float Force;

        public override void Init()
        {
            Duration = ConfigOption(10f, "Duration", "Vanilla is 10");
            Range = ConfigOption(30f, "Radius", "Vanilla is 30");
            Speed = ConfigOption(10f, "Projectile Speed", "Vanilla is 10");
            Force = ConfigOption(-1500f, "Force", "Vanilla is -1500");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var hole = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Blackhole/GravSphere.prefab").WaitForCompletion();
            hole.GetComponent<ProjectileSimple>().lifetime = Duration;
            hole.GetComponent<ProjectileSimple>().desiredForwardSpeed = Speed;
            hole.GetComponent<RadialForce>().radius = Range;
            hole.GetComponent<RadialForce>().forceMagnitude = Force;
        }
    }
}