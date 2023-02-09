namespace WellRoundedBalance.Interactables
{
    public class LunarPod : InteractableBase
    {
        public override string Name => ":: Interactables ::::::: Lunar Pod";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var iscLunarPod = Utils.Paths.InteractableSpawnCard.iscLunarChest.Load<InteractableSpawnCard>();
            iscLunarPod.maxSpawnsPerStage = 1;
            iscLunarPod.directorCreditCost = 15;

            var lunarPod = Utils.Paths.GameObject.LunarChest.Load<GameObject>();
            Object.Destroy(lunarPod.GetComponent<ChestBehavior>());
            var optionChestBehavior = lunarPod.AddComponent<OptionChestBehavior>();

            var lunarPodDropTable = ScriptableObject.CreateInstance<LunarPodDropTable>();

            optionChestBehavior.dropTable = lunarPodDropTable;
            // optionChestBehavior.dropTransform =
            optionChestBehavior.dropUpVelocityStrength = 20f;
            optionChestBehavior.dropForwardVelocityStrength = 3f;
            optionChestBehavior.openState = new(typeof(EntityStates.Barrel.OpeningLunar));
            optionChestBehavior.pickupPrefab = Utils.Paths.GameObject.OptionPickup.Load<GameObject>();
            optionChestBehavior.numOptions = 2;
            optionChestBehavior.displayTier = ItemTier.Lunar;
        }
    }

    public class LunarPodDropTable : PickupDropTable
    {
        public WeightedSelection<PickupIndex> weighted = new();

        public override int GetPickupCount()
        {
            return weighted.Count;
        }

        public void GenerateWeightedSelection()
        {
            weighted.Clear();
            foreach (ItemDef itemDef in ItemCatalog.itemDefs.Where(x => x.deprecatedTier == ItemTier.Lunar))
            {
                if (itemDef.name.ToLower().Contains("replacement") || itemDef.name.ToLower().Contains("heresy"))
                {
                    continue;
                }
                weighted.AddChoice(PickupCatalog.FindPickupIndex(itemDef.itemIndex), 1);
            }
        }

        public override PickupIndex[] GenerateUniqueDropsPreReplacement(int maxDrops, Xoroshiro128Plus rng)
        {
            GenerateWeightedSelection();
            return GenerateUniqueDropsFromWeightedSelection(maxDrops, rng, weighted);
        }

        public override PickupIndex GenerateDropPreReplacement(Xoroshiro128Plus rng)
        {
            GenerateWeightedSelection();
            Debug.Log(GenerateDropFromWeightedSelection(rng, weighted));
            return GenerateDropFromWeightedSelection(rng, weighted);
        }
    }
}