
namespace UltimateCustomRun
{
    static class MonsterTooth
    {
        // honestly help me lmao
        // the stupid ass math.pow thing whyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
        // how do I do the IL :pain:

        public static void ChangeHealing()
        {
            var MonsterTooth = Resources.Load<GameObject>("Prefabs/NetworkedObjects/HealPack");
            HealthPickup cic = MonsterTooth.GetComponentInChildren<HealthPickup>();
            cic.flatHealing = MonsterToothFlatHealing.Value;
            cic.fractionalHealing = MonsterToothPercentHealing.Value;
        }
        // farthest i got
        // fuck this item j
    }
}
