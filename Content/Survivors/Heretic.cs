namespace WellRoundedBalance.Survivors
{
    internal class Heretic : SurvivorBase
    {
        public override string Name => ":: Survivors :: Heretic";

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var hereticBody = Utils.Paths.GameObject.HereticBody.Load<GameObject>().GetComponent<CharacterBody>();
            hereticBody.baseDamage = 12f;
            hereticBody.levelDamage = 2.4f;
            hereticBody.baseMaxHealth = 110f;
            hereticBody.levelMaxHealth = 33f;
            hereticBody.baseMoveSpeed = 7f;
            hereticBody.baseJumpCount = 2;
            hereticBody.baseRegen = -2f;
            hereticBody.levelRegen = -0.4f;
        }
    }
}