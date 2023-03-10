namespace WellRoundedBalance.Interactables
{
    internal class CleansingPool : InteractableBase
    {
        public override string Name => ":: Interactables :::::::::: Cleansing Pool";

        [ConfigField("Max Spawns Per Stage", "", 1)]
        public static int maxSpawnsPerStage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var pool1 = Utils.Paths.InteractableSpawnCard.iscShrineCleanse.Load<InteractableSpawnCard>();
            pool1.maxSpawnsPerStage = maxSpawnsPerStage;

            var pool2 = Utils.Paths.InteractableSpawnCard.iscShrineCleanseSandy.Load<InteractableSpawnCard>();
            pool2.maxSpawnsPerStage = maxSpawnsPerStage;

            var pool3 = Utils.Paths.InteractableSpawnCard.iscShrineCleanseSnowy.Load<InteractableSpawnCard>();
            pool3.maxSpawnsPerStage = maxSpawnsPerStage;
        }
    }
}