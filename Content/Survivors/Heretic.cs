namespace WellRoundedBalance.Survivors
{
    internal class Heretic : SurvivorBase<Heretic>
    {
        public override string Name => ":: Survivors :: Heretic";

        [ConfigField("Base Damage", "", 12f)]
        public static float baseDamage;

        [ConfigField("Base Max Health", "", 110f)]
        public static float baseMaxHealth;

        [ConfigField("Base Move Speed", "", 7f)]
        public static float baseMoveSpeed;

        [ConfigField("Base Regeneneration", "", -2f)]
        public static float baseRegeneration;

        [ConfigField("Base Jump Count", "", 2)]
        public static int baseJumpCount;

        [ConfigField("Base Armor", "", 0f)]
        public static float baseArmor;

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var hereticBody = Utils.Paths.GameObject.HereticBody.Load<GameObject>().GetComponent<CharacterBody>();
            hereticBody.baseDamage = baseDamage;
            hereticBody.levelDamage = baseDamage * 0.2f;
            hereticBody.baseMaxHealth = baseMaxHealth;
            hereticBody.levelMaxHealth = baseMaxHealth * 0.3f;
            hereticBody.baseMoveSpeed = baseMoveSpeed;
            hereticBody.baseJumpCount = baseJumpCount;
            hereticBody.baseRegen = baseRegeneration;
            hereticBody.levelRegen = baseRegeneration * 0.2f;
            hereticBody.baseArmor = baseArmor;
        }
    }
}