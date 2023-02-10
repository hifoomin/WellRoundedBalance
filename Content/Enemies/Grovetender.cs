namespace WellRoundedBalance.Enemies
{
    internal class Grovetender : EnemyBase
    {
        public override string Name => ":: Enemies ::::::: Grovetender";

        public override void Hooks()
        {
            var sunderedGrove = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonsters.Load<DirectorCardCategorySelection>();
            sunderedGrove.categories[0] /* champions */.cards[0] /* clay dunestrider */.spawnCard = Utils.Paths.CharacterSpawnCard.cscGravekeeper.Load<CharacterSpawnCard>();

            var sunderedGroveDLC1 = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonstersDLC1.Load<DirectorCardCategorySelection>();
            sunderedGroveDLC1.categories[0] /* champions */.cards[0] /* clay dunestrider */.spawnCard = Utils.Paths.CharacterSpawnCard.cscGravekeeper.Load<CharacterSpawnCard>();
        }
    }
}