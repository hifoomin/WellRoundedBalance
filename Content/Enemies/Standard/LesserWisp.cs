namespace WellRoundedBalance.Enemies.Standard
{
    internal class LesserWisp : EnemyBase<LesserWisp>
    {
        public override string Name => ":: Enemies :: Lesser Wisp";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Wisp1Monster.FireEmbers.OnEnter += FireEmbers_OnEnter;
            Changes();
        }

        private void FireEmbers_OnEnter(On.EntityStates.Wisp1Monster.FireEmbers.orig_OnEnter orig, EntityStates.Wisp1Monster.FireEmbers self)
        {
            EntityStates.Wisp1Monster.FireEmbers.minSpread = 0f;
            EntityStates.Wisp1Monster.FireEmbers.maxSpread = 0f;
            EntityStates.Wisp1Monster.FireEmbers.bulletCount = 1;
            EntityStates.Wisp1Monster.FireEmbers.baseDuration = 2.5f;
            EntityStates.Wisp1Monster.FireEmbers.damageCoefficient = 2f;
            orig(self);
        }

        private void Changes()
        {
            var wisp = Utils.Paths.GameObject.WispBody16.Load<GameObject>();
            var wispBody = wisp.GetComponent<CharacterBody>();
            wispBody.baseDamage = 12f;
            wispBody.levelDamage = 2.4f;
            wispBody.baseMaxHealth = 70f;
            wispBody.levelMaxHealth = 21f;

            var csc = Utils.Paths.CharacterSpawnCard.cscLesserWisp.Load<CharacterSpawnCard>();
            csc.directorCreditCost = 15;
        }
    }
}