namespace WellRoundedBalance.Interactables
{
    internal class Barrel : InteractableBase
    {
        public override string Name => ":: Interactables : Barrel";

        [ConfigField("Max Spawns Per Stage", "", 25)]
        public static int maxSpawnsPerStage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var barrel = Utils.Paths.InteractableSpawnCard.iscBarrel1.Load<InteractableSpawnCard>();
            barrel.maxSpawnsPerStage = maxSpawnsPerStage;
        }
    }
}