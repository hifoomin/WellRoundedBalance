namespace WellRoundedBalance.Enemies.Bosses
{
    internal class Grovetender : EnemyBase
    {
        public override string Name => ":: Enemies :::::::: Grovetender";

        [ConfigField("Should replace Clay Dunestriders on Sundered Grove?", "", true)]
        public static bool shouldReplaceClayDunestriderOnSunderedGrove;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            if (shouldReplaceClayDunestriderOnSunderedGrove)
            {
                var sunderedGrove = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonsters.Load<DirectorCardCategorySelection>();
                sunderedGrove.categories[0] /* champions */.cards[0] /* clay dunestrider */.spawnCard = Utils.Paths.CharacterSpawnCard.cscGravekeeper.Load<CharacterSpawnCard>();

                var sunderedGroveDLC1 = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonstersDLC1.Load<DirectorCardCategorySelection>();
                sunderedGroveDLC1.categories[0] /* champions */.cards[0] /* clay dunestrider */.spawnCard = Utils.Paths.CharacterSpawnCard.cscGravekeeper.Load<CharacterSpawnCard>();
            }
        }
    }
}