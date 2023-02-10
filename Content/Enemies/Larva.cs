namespace WellRoundedBalance.Enemies
{
    internal class Larva : EnemyBase
    {
        public override string Name => ":: Enemies :::::: Larva";

        public override void Hooks()
        {
            var larva = Utils.Paths.GameObject.AcidLarvaBody8.Load<GameObject>();
            var larvaBody = larva.GetComponent<CharacterBody>();
            larvaBody.baseMoveSpeed = 1f;

            var larvaSC = Utils.Paths.CharacterSpawnCard.cscAcidLarva.Load<CharacterSpawnCard>();
            larvaSC.directorCreditCost = 25;
        }
    }
}