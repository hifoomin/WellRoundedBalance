using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Enemies
{
    internal class AlphaConstruct : EnemyBase
    {
        public override string Name => ":: Enemies :::::::: Alpha Construct";

        public override void Hooks()
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

            DirectorAPI.AddCard(commencement, cardHolder);
        }
    }
}