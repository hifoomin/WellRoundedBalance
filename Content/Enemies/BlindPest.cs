namespace WellRoundedBalance.Enemies
{
    internal class BlindPest : EnemyBase
    {
        public override string Name => ":: Enemies :::: Blind Pest";

        public override void Hooks()
        {
            var blindPest = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/FlyingVerminBody.prefab").WaitForCompletion();
            var blindPestBody = blindPest.GetComponent<CharacterBody>();
            blindPestBody.baseDamage = 10f;
            blindPestBody.levelDamage = 2f;

            var blindPestSC = Utils.Paths.CharacterSpawnCard.cscFlyingVermin.Load<CharacterSpawnCard>();
            blindPestSC.directorCreditCost = 35;

            var blindPestSC2 = Utils.Paths.CharacterSpawnCard.cscFlyingVerminSnowy.Load<CharacterSpawnCard>();
            blindPestSC2.directorCreditCost = 35;

            var siphoned = Utils.Paths.DirectorCardCategorySelection.dccsSnowyForestMonstersDLC1.Load<DirectorCardCategorySelection>();
            siphoned.categories[2] /* standard monsters */.cards[2] /* blind pest */.spawnCard = Utils.Paths.CharacterSpawnCard.cscMinorConstruct.Load<CharacterSpawnCard>();
        }
    }
}