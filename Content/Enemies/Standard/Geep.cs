namespace WellRoundedBalance.Enemies.Standard
{
    internal class Geep : EnemyBase<Geep>
    {
        public override string Name => ":: Enemies :: Geep";

        [ConfigField("Base Max Health", "Disabled if playing Inferno.", 250f)]
        public static float baseMaxHealth;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (Main.IsInfernoDef())
            {
                return;
            }
            switch (body.name)
            {
                case "GeepBody(Clone)":
                    body.baseMoveSpeed = 24f;
                    break;
            }
        }

        private void Changes()
        {
            var geep = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Gup/GeepBody.prefab").WaitForCompletion();

            var geepBody = geep.GetComponent<CharacterBody>();
            geepBody.baseMaxHealth = baseMaxHealth;
            geepBody.levelMaxHealth = baseMaxHealth * 0.3f;
        }
    }
}