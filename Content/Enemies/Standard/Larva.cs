namespace WellRoundedBalance.Enemies.Standard
{
    internal class Larva : EnemyBase<Larva>
    {
        public override string Name => ":: Enemies :: Larva";

        [ConfigField("Base Move Speed", "Disabled if playing Inferno.", 1f)]
        public static float baseMoveSpeed;

        [ConfigField("Director Credit Cost", "", 25)]
        public static int directorCreditCost;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.AcidLarva.LarvaLeap.OnEnter += LarvaLeap_OnEnter;
        }

        private void LarvaLeap_OnEnter(On.EntityStates.AcidLarva.LarvaLeap.orig_OnEnter orig, EntityStates.AcidLarva.LarvaLeap self)
        {
            self.airControl = 0.5f;
            self.detonateSelfDamageFraction = 0.15f;
            orig(self);
        }

        private void Changes()
        {
            var larva = Utils.Paths.GameObject.AcidLarvaBody8.Load<GameObject>();
            var larvaBody = larva.GetComponent<CharacterBody>();
            larvaBody.baseMoveSpeed = baseMoveSpeed;
            larvaBody.baseMaxHealth = 60f;
            larvaBody.levelMaxHealth = 18f;

            var larvaSC = Utils.Paths.CharacterSpawnCard.cscAcidLarva.Load<CharacterSpawnCard>();
            larvaSC.directorCreditCost = directorCreditCost;
        }
    }
}