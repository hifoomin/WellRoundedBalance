namespace WellRoundedBalance.Interactables
{
    internal class VoidSeed : InteractableBase
    {
        public override string Name => ":: Interactables :::::: Void Seed";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var voidSeed = Utils.Paths.InteractableSpawnCard.iscVoidCamp.Load<InteractableSpawnCard>();
            voidSeed.maxSpawnsPerStage = 1;
        }
    }
}