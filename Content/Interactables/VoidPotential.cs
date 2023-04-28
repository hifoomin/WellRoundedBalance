namespace WellRoundedBalance.Interactables
{
    internal class VoidPotential : InteractableBase<VoidPotential>
    {
        public override string Name => ":: Interactables :::: Void Potential";

        [ConfigField("Max Spawns Per Stage", "", 1)]
        public static int maxSpawnsPerStage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var dropTable = Utils.Paths.BasicPickupDropTable.dtVoidTriple.Load<BasicPickupDropTable>();
            dropTable.tier1Weight = 0.35f;
            dropTable.tier2Weight = 0.6f;
            dropTable.tier3Weight = 0.012f;
            dropTable.voidTier3Weight = 0.075f;
            dropTable.voidBossWeight = 0.075f;
        }
    }
}