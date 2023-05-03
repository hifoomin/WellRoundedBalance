namespace WellRoundedBalance.Enemies.Standard
{
    internal class VoidBarnacle : EnemyBase<VoidBarnacle>
    {
        public override string Name => ":: Enemies :: Void Barnacle";

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
            var barnacleBody = Utils.Paths.GameObject.VoidBarnacleBody9.Load<GameObject>().GetComponent<CharacterBody>();
            barnacleBody.baseRegen = 0f;
            barnacleBody.levelRegen = 0f;
        }
    }
}