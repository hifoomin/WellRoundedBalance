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

            lunarPod.AddComponent<UnityTechnologies>();
        }
    }

    public class UnityTechnologies : MonoBehaviour
    {
        public void Start()
        {
            // have to do this in a component because Unity Technologies
            GetComponent<PurchaseInteraction>().onPurchase.AddListener(Open);
        }

        public void Open(Interactor interactor)
        {
            GetComponent<OptionChestBehavior>().Open();
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
            foreach (PickupIndex index in Run.instance.availableLunarCombinedDropList)
            {
                ItemDef def = ItemCatalog.GetItemDef(index.itemIndex);
                if (def && !string.IsNullOrEmpty(def.name) && !def.name.ToLower().Contains("replacement"))
                {
                    weighted.AddChoice(index, 1f);
                }
            }
        }

        public override PickupIndex[] GenerateUniqueDropsPreReplacement(int maxDrops, Xoroshiro128Plus rng)
        {
            GenerateWeightedSelection();
            PickupIndex[] drops = GenerateUniqueDropsFromWeightedSelection(maxDrops, rng, weighted);
            foreach (PickupIndex index in drops)
            {
                // Debug.Log(index);
            }
            return drops;
        }

        public override PickupIndex GenerateDropPreReplacement(Xoroshiro128Plus rng)
        {
            GenerateWeightedSelection();
            Debug.Log(GenerateDropFromWeightedSelection(rng, weighted));
            return GenerateDropFromWeightedSelection(rng, weighted);
        }
    }
}