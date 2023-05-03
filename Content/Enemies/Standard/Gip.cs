namespace WellRoundedBalance.Enemies.Standard
{
    internal class Gip : EnemyBase<Gip>
    {
        public override string Name => ":: Enemies :: Gip";

        [ConfigField("Base Max Health", "Disabled if playing Inferno.", 125f)]
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
                case "GipBody(Clone)":
                    body.baseMoveSpeed = 29f;
                    break;
            }
        }

        public void Changes()
        {
            var gip = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Gup/GipBody.prefab").WaitForCompletion();

            var gipBody = gip.GetComponent<CharacterBody>();
            gipBody.baseMaxHealth = baseMaxHealth;
            gipBody.levelMaxHealth = baseMaxHealth * 0.3f;
        }
    }
}