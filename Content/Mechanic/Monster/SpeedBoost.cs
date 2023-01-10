namespace WellRoundedBalance.Mechanic.Monster
{
    internal class SpeedBoost : GlobalBase
    {
        public override string Name => ":: Mechanic ::::::: Monsters : Movement Speed Buff";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            AddSpeedBoost();
        }

        [SystemInitializer(typeof(BodyCatalog))]
        public static void AddSpeedBoost()
        {
            foreach (CharacterBody body in BodyCatalog.allBodyPrefabBodyBodyComponents)
            {
                body.baseMoveSpeed += 1f;
                body.baseMoveSpeed *= 1.1f;
            }
        }
    }
}