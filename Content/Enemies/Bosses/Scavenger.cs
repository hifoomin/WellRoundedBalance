namespace WellRoundedBalance.Enemies.Bosses
{
    internal class Scavenger : EnemyBase<Scavenger>
    {
        public override string Name => "::: Bosses :: Scavenger";

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
            var grove1 = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonsters.Load<DirectorCardCategorySelection>();
            grove1.categories[3].cards[0].minimumStageCompletions = 4;

            var grove2 = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonstersDLC1.Load<DirectorCardCategorySelection>();
            grove2.categories[3].cards[0].minimumStageCompletions = 4;
        }
    }
}