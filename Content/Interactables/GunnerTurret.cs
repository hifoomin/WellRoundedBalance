namespace WellRoundedBalance.Interactables
{
    internal class GunnerTurret : InteractableBase
    {
        public override string Name => ":: Interactables ::::::::: Gunner Turret";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var gunnerTurret = Utils.Paths.InteractableSpawnCard.iscBrokenTurret1.Load<InteractableSpawnCard>();
            gunnerTurret.maxSpawnsPerStage = 2;
        }
    }
}