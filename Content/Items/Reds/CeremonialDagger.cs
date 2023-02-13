namespace WellRoundedBalance.Items.Reds
{
    public class CeremonialDagger : ItemBase
    {
        public override string Name => ":: Items ::: Reds :: Ceremonial Dagger";
        public override string InternalPickupToken => "dagger";

        public override string PickupText => "Killing an enemy releases homing daggers.";

        public override string DescText => "Killing an enemy fires out <style=cIsDamage>3</style> <style=cIsDamage>homing daggers</style> that deal <style=cIsDamage>150%</style> <style=cStack>(+150% per stack)</style> base damage.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            ChangeProc();
        }

        public static void ChangeProc()
        {
            var c = Utils.Paths.GameObject.DaggerProjectile.Load<GameObject>().GetComponent<ProjectileController>();
            c.procCoefficient = 0f;
        }
    }
}