namespace WellRoundedBalance.Enemies.Bosses
{
    internal class OverloadingWorm : EnemyBase<OverloadingWorm>
    {
        public override string Name => "::: Bosses :: Overloading Worm";

        [ConfigField("Remove on Abyssal Depths Stage 4?", "", true)]
        public static bool removeAbyssal;

        [ConfigField("Remove on Sky Meadow Stage 5?", "", true)]
        public static bool removeMeadow;

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
            var worm = Utils.Paths.GameObject.ElectricWormBody32.Load<GameObject>();
            var wormBody = worm.GetComponent<CharacterBody>();
            wormBody.baseDamage = 42f;
            wormBody.levelDamage = 8.4f;
            wormBody.baseMaxHealth = 9000f;
            wormBody.levelMaxHealth = 2700f;

            var abyssal1 = Utils.Paths.DirectorCardCategorySelection.dccsDampCaveMonsters.Load<DirectorCardCategorySelection>();
            abyssal1.categories[0].cards[3].minimumStageCompletions = 5;

            var abyssal2 = Utils.Paths.DirectorCardCategorySelection.dccsDampCaveMonstersDLC1.Load<DirectorCardCategorySelection>();
            abyssal2.categories[0].cards[3].minimumStageCompletions = 5;

            var meadow1 = Utils.Paths.DirectorCardCategorySelection.dccsSkyMeadowMonsters.Load<DirectorCardCategorySelection>();
            meadow1.categories[0].cards[2].minimumStageCompletions = 6;

            var meadow2 = Utils.Paths.DirectorCardCategorySelection.dccsSkyMeadowMonstersDLC1.Load<DirectorCardCategorySelection>();
            meadow2.categories[0].cards[1].minimumStageCompletions = 6;
        }
    }
}