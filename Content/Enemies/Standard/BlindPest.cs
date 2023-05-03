namespace WellRoundedBalance.Enemies.Standard
{
    internal class BlindPest : EnemyBase<BlindPest>
    {
        public override string Name => ":: Enemies :: Blind Pest";

        [ConfigField("Base Damage", "Disabled if playing Inferno.", 11f)]
        public static float baseDamage;

        [ConfigField("Director Credit Cost", "", 35)]
        public static int directorCreditCost;

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
            var blindPest = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/FlyingVerminBody.prefab").WaitForCompletion();
            var blindPestBody = blindPest.GetComponent<CharacterBody>();
            blindPestBody.baseDamage = baseDamage;
            blindPestBody.levelDamage = baseDamage * 0.2f;

            var blindPestSC = Utils.Paths.CharacterSpawnCard.cscFlyingVermin.Load<CharacterSpawnCard>();
            blindPestSC.directorCreditCost = directorCreditCost;

            var blindPestSC2 = Utils.Paths.CharacterSpawnCard.cscFlyingVerminSnowy.Load<CharacterSpawnCard>();
            blindPestSC2.directorCreditCost = directorCreditCost;
        }
    }
}