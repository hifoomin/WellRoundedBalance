namespace WellRoundedBalance.Enemies.Standard
{
    internal class LesserWisp : EnemyBase<LesserWisp>
    {
        public override string Name => ":: Enemies :: Lesser Wisp";

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
            var wisp = Utils.Paths.GameObject.WispBody16.Load<GameObject>();
            var wispBody = wisp.GetComponent<CharacterBody>();
            wispBody.baseDamage = 5f;
            wispBody.levelDamage = 1f;
        }
    }
}