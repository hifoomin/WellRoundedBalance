namespace WellRoundedBalance.Items.Reds
{
    public class CeremonialDagger : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Ceremonial Dagger";
        public override ItemDef InternalPickup => RoR2Content.Items.Dagger;

        public override string PickupText => "Killing an enemy releases homing daggers.";

        public override string DescText => "Killing an enemy fires out <style=cIsDamage>3</style> <style=cIsDamage>homing daggers</style> that deal <style=cIsDamage>150%</style> <style=cStack>(+150% per stack)</style> base damage.";

        [ConfigField("Proc Chance", 0f)]
        public static float procChance;

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
            var c = Utils.Paths.GameObject.DaggerProjectile.Load<GameObject>().GetComponent<ProjectileController>();
            c.procCoefficient = procChance * globalProc;
        }
    }
}