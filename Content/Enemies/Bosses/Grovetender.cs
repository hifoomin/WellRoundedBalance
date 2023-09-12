namespace WellRoundedBalance.Enemies.Bosses
{
    internal class Grovetender : EnemyBase<Grovetender>
    {
        public override string Name => "::: Bosses :: Grovetender";

        [ConfigField("Should replace Clay Dunestriders on Sundered Grove?", "", true)]
        public static bool shouldReplaceClayDunestriderOnSunderedGrove;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.GravekeeperBoss.PrepHook.OnEnter += PrepHook_OnEnter;
            On.EntityStates.GravekeeperBoss.FireHook.OnEnter += FireHook_OnEnter;
            Changes();
        }

        public static GameObject hookPrefab = Utils.Paths.GameObject.GravekeeperHookProjectile.Load<GameObject>();

        private void FireHook_OnEnter(On.EntityStates.GravekeeperBoss.FireHook.orig_OnEnter orig, EntityStates.GravekeeperBoss.FireHook self)
        {
            EntityStates.GravekeeperBoss.FireHook.projectilePrefab = hookPrefab;
            EntityStates.GravekeeperBoss.FireHook.projectileForce = -800f;
            EntityStates.GravekeeperBoss.FireHook.projectileDamageCoefficient = 0.45f;
            orig(self);
        }

        private void PrepHook_OnEnter(On.EntityStates.GravekeeperBoss.PrepHook.orig_OnEnter orig, EntityStates.GravekeeperBoss.PrepHook self)
        {
            EntityStates.GravekeeperBoss.PrepHook.baseDuration = 1.3f;
            orig(self);
        }

        private void Changes()
        {
            if (shouldReplaceClayDunestriderOnSunderedGrove)
            {
                var sunderedGrove = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonsters.Load<DirectorCardCategorySelection>();
                sunderedGrove.categories[0] /* champions */.cards[0] /* clay dunestrider */.spawnCard = Utils.Paths.CharacterSpawnCard.cscGravekeeper.Load<CharacterSpawnCard>();

                var sunderedGroveDLC1 = Utils.Paths.DirectorCardCategorySelection.dccsRootJungleMonstersDLC1.Load<DirectorCardCategorySelection>();
                sunderedGroveDLC1.categories[0] /* champions */.cards[0] /* clay dunestrider */.spawnCard = Utils.Paths.CharacterSpawnCard.cscGravekeeper.Load<CharacterSpawnCard>();
            }

            var wisp = Utils.Paths.GameObject.GravekeeperTrackingFireball.Load<GameObject>().GetComponent<CharacterBody>();
            wisp.baseMaxHealth = 35f;
            wisp.levelMaxHealth = 10.5f;
        }
    }
}