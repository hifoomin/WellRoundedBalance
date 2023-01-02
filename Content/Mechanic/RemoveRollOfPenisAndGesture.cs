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
            if (Run.instance.availableItems.Contains(RoR2Content.Items.AutoCastEquipment.itemIndex))
            {
                Run.instance.availableItems.Remove(RoR2Content.Items.AutoCastEquipment.itemIndex);
            }
            if (Run.instance.availableItems.Contains(DLC1Content.Items.GoldOnHurt.itemIndex))
            {
                Run.instance.availableItems.Remove(DLC1Content.Items.GoldOnHurt.itemIndex);
            }
        }
    }
}