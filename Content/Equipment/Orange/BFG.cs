using MonoMod.Cil;

namespace WellRoundedBalance.Equipment
{
    public class BFG : EquipmentBase
    {
        public override string Name => ":: Equipment :: Preon Accumulator";
        public override string InternalPickupToken => "bfg";

        public override string PickupText => "Fire a ball of energy that electrocutes nearby enemies before detonating.";

        public override string DescText => "Fires preon tendrils, zapping enemies within 35m for up to <style=cIsDamage>1200% damage/second</style>. On contact, detonate in an enormous 20m explosion for <style=cIsDamage>8000% damage</style>.";

        // desc fix
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
            var bfg = Utils.Paths.GameObject.BeamSphere.Load<GameObject>();
            var projectileProximityBeamController = bfg.GetComponent<ProjectileProximityBeamController>();
            projectileProximityBeamController.damageCoefficient = 1.995f;
            // tendrils proccing bands fix
        }
    }
}