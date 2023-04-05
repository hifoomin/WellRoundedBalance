namespace WellRoundedBalance.Items.Reds
{
    public class CeremonialDagger : ItemBase<CeremonialDagger>
    {
        public override string Name => ":: Items ::: Reds :: Ceremonial Dagger";
        public override ItemDef InternalPickup => RoR2Content.Items.Dagger;

        public override string PickupText => "Killing an enemy releases homing daggers.";

        public override string DescText => "Killing an enemy fires out <style=cIsDamage>3</style> <style=cIsDamage>homing daggers</style> that deal <style=cIsDamage>150%</style> <style=cStack>(+150% per stack)</style> base damage.";

        [ConfigField("Proc Coefficient", 0f)]
        public static float procCoefficient;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var dagger = Utils.Paths.GameObject.DaggerProjectile.Load<GameObject>();
            var projectileController = dagger.GetComponent<ProjectileController>();
            projectileController.procCoefficient = procCoefficient * globalProc;

            var projectileDirectionalTargetFinder = dagger.GetComponent<ProjectileDirectionalTargetFinder>();
            projectileDirectionalTargetFinder.lookRange = 40f;
        }
    }
}