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
            dropTable.tier1Weight = 0.25f;
            dropTable.tier2Weight = 0.45f;
            dropTable.tier3Weight = 0.01f;
            dropTable.voidTier3Weight = 0.2f;
            dropTable.voidBossWeight = 0.2f;
        }
    }
}