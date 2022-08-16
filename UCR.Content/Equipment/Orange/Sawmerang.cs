using MonoMod.Cil;
using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;

namespace UltimateCustomRun.Equipment
{
    public class Sawmerang : EquipmentBase
    {
        public override string Name => "::: Equipment :: Sawmerang";
        public override string InternalPickupToken => "sawmerang";

        public override bool NewPickup => false;

        public override bool NewDesc => true;

        public override string PickupText => "";

        public override string DescText => "Throw <style=cIsDamage>three large saw blades</style> that slice through enemies for <style=cIsDamage>3x" + d(BeegDamage) + "</style> damage. Also deals an additional <style=cIsDamage>3x" + d(SmolDamage) + " damage per second</style> while <style=cIsDamage>bleeding</style> enemies. Can <style=cIsDamage>strike</style> enemies again on the way back.";

        public static float Rotation;
        public static float BeegDamage;
        public static float BeegProcCoefficient;
        public static float SmolDamage;
        public static float SmolProcCoefficient;
        public static float DistanceMult;
        public static float Speed;

        public override void Init()
        {
            Rotation = ConfigOption(15f, "Rotation Per Saw", "Vanilla is 15");
            BeegDamage = ConfigOption(4f, "Front Saw Damage", "Decimal. Vanilla is 4");
            BeegProcCoefficient = ConfigOption(1f, "Front Saw Proc Coefficient", "Vanilla is 1");
            SmolDamage = ConfigOption(1.4f, "Returning Saw Damage", "Decimal. Vanilla is 1.4");
            SmolProcCoefficient = ConfigOption(0.2f, "Returning Saw Proc Coefficient", "Vanilla is 0.2");
            DistanceMult = ConfigOption(0.6f, "Distance Multiplier", "Vanilla is 0.6");
            Speed = ConfigOption(60f, "Projectile Speed", "Vanilla is 60");
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.EquipmentSlot.FireSaw += ChangeRotation;
            Changes();
        }

        private void ChangeRotation(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(15f)))
            {
                c.Next.Operand = Rotation;
            }
            else
            {
                Main.UCRLogger.LogError("Failed to apply Sawmerang Rotation hook");
            }
        }

        private void Changes()
        {
            var saw = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Saw/Sawmerang.prefab").WaitForCompletion();
            saw.GetComponent<BoomerangProjectile>().travelSpeed = Speed;
            saw.GetComponent<BoomerangProjectile>().distanceMultiplier = DistanceMult;
            saw.GetComponent<ProjectileOverlapAttack>().damageCoefficient = BeegDamage;
            saw.GetComponent<ProjectileOverlapAttack>().overlapProcCoefficient = BeegProcCoefficient;
            saw.GetComponent<ProjectileDotZone>().damageCoefficient = SmolDamage;
            saw.GetComponent<ProjectileDotZone>().overlapProcCoefficient = SmolProcCoefficient;
        }
    }
}