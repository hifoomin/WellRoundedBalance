using RoR2;

namespace WellRoundedBalance.Global
{
    public static class RemoveRollOfPenisAndGesture
    {
        public static void Based()
        {
            SceneDirector.onPostPopulateSceneServer += SceneDirector_onPostPopulateSceneServer;
        }

        private static void SceneDirector_onPostPopulateSceneServer(RoR2.SceneDirector obj)
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