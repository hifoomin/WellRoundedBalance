namespace WellRoundedBalance.Global
{
    public static class RemoveRollOfPenisAndGesture
    {
        public static void Based()
        {
            Run.onRunStartGlobal += Run_onRunStartGlobal;
        }

        private static void Run_onRunStartGlobal(Run run)
        {
            run.availableItems.Remove(RoR2Content.Items.AutoCastEquipment.itemIndex);
            run.availableItems.Remove(DLC1Content.Items.GoldOnHurt.itemIndex);
            run.availableTier1DropList.Remove(PickupCatalog.FindPickupIndex(DLC1Content.Items.GoldOnHurt.itemIndex));
            run.availableLunarItemDropList.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.AutoCastEquipment.itemIndex));
            run.availableLunarCombinedDropList.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.AutoCastEquipment.itemIndex));
        }
    }
}