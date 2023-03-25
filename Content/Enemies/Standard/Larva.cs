namespace WellRoundedBalance.Enemies.Standard
{
    internal class Larva : EnemyBase
    {
        public override string Name => ":: Enemies :::::: Larva";

        [ConfigField("Base Move Speed", "Disabled if playing Inferno.", 1f)]
        public static float baseMoveSpeed;

        [ConfigField("Director Credit Cost", "", 25)]
        public static int directorCreditCost;

        public override void Hooks()
        {
            var larva = Utils.Paths.GameObject.AcidLarvaBody8.Load<GameObject>();
            var larvaBody = larva.GetComponent<CharacterBody>();
            larvaBody.baseMoveSpeed = baseMoveSpeed;

            var larvaSC = Utils.Paths.CharacterSpawnCard.cscAcidLarva.Load<CharacterSpawnCard>();
            larvaSC.directorCreditCost = directorCreditCost;
        }
    }
}