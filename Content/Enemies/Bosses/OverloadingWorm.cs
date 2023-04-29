namespace WellRoundedBalance.Enemies.Bosses
{
    internal class OverloadingWorm : EnemyBase<OverloadingWorm>
    {
        public override string Name => "::: Bosses ::::::: Overloading Worm";

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
            var OwO = Utils.Paths.GameObject.ElectricWormBody32.Load<GameObject>();
            var UwU = OwO.GetComponent<CharacterBody>();
            UwU.baseDamage = 42f;
            UwU.levelDamage = 8.4f;
            UwU.baseMaxHealth = 9000f;
            UwU.levelMaxHealth = 2700f;
        }
    }
}