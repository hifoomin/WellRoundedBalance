﻿namespace WellRoundedBalance.Enemies.Standard
{
    internal class AlphaConstruct : EnemyBase<AlphaConstruct>
    {
        public override string Name => ":: Enemies :::::::: Alpha Construct";

        public override void Init()
        {
            base.Init();
        }

        [ConfigField("Should replace Blind Pests on Siphoned Forest?", "", true)]
        public static bool shouldReplaceBlindPestsOnSiphonedForest;

        [ConfigField("Should spawn on Commencement?", "", true)]
        public static bool shouldSpawnOnCommencement;

        public override void Hooks()
        {
            if (shouldSpawnOnCommencement)
            {
                var commencement = Utils.Paths.DirectorCardCategorySelection.dccsMoonMonsters.Load<DirectorCardCategorySelection>();
                var commencementDLC1 = Utils.Paths.DirectorCardCategorySelection.dccsMoonMonstersDLC1.Load<DirectorCardCategorySelection>();

                var alphaConstructCard = new DirectorCard
                {
                    spawnCard = Utils.Paths.CharacterSpawnCard.cscMinorConstruct.Load<CharacterSpawnCard>(),
                    selectionWeight = 1,
                    spawnDistance = DirectorCore.MonsterSpawnDistance.Close,
                    preventOverhead = false
                };

                var cardHolder = new DirectorAPI.DirectorCardHolder
                {
                    Card = alphaConstructCard,
                    MonsterCategory = DirectorAPI.MonsterCategory.BasicMonsters
                };

                commencement.AddCard(cardHolder);
                commencementDLC1.AddCard(cardHolder);
            }

            if (shouldReplaceBlindPestsOnSiphonedForest)
            {
                var siphoned = Utils.Paths.DirectorCardCategorySelection.dccsSnowyForestMonstersDLC1.Load<DirectorCardCategorySelection>();
                siphoned.categories[2] /* standard monsters */.cards[2] /* blind pest */.spawnCard = Utils.Paths.CharacterSpawnCard.cscMinorConstruct.Load<CharacterSpawnCard>();
            }
        }
    }
}