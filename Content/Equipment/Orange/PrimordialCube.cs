namespace WellRoundedBalance.Equipment.Orange
{
    public class PrimordialCube : EquipmentBase
    {
        public override string Name => ":: Equipment :: Primordial Cube";
        public override EquipmentDef InternalPickup => RoR2Content.Equipment.Blackhole;

        public override string PickupText => "Fire a black hole that draws enemies in.";

        public override string DescText => "Fire a black hole that <style=cIsUtility>draws enemies within " + range + "m" +
                                           (pullStrength > 0 ? " outside of" : " into") +
                                          " its center</style>. Lasts " + duration + " seconds.";

        [ConfigField("Cooldown", "", 60f)]
        public static float cooldown;

        [ConfigField("Duration", "", 10f)]
        public static float duration;

        [ConfigField("Range", "", 30f)]
        public static float range;

        [ConfigField("Speed", "", 10f)]
        public static float speed;

        [ConfigField("Pull Strength", "", -1500f)]
        public static float pullStrength;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();

            var Hole = Utils.Paths.EquipmentDef.Blackhole.Load<EquipmentDef>();
            Hole.cooldown = cooldown;
        }

        private void Changes()
        {
            var hole = Utils.Paths.GameObject.GravSphere.Load<GameObject>();

            var projectileSimple = hole.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = duration;
            projectileSimple.desiredForwardSpeed = speed;

            var radialForce = hole.GetComponent<RadialForce>();
            radialForce.radius = range;
            radialForce.forceMagnitude = pullStrength;
        }
    }
}