using static UnityEngine.SpookyHash;

namespace WellRoundedBalance.Interactables
{
    public class LunarPod : InteractableBase<LunarPod>
    {
        public override string Name => ":: Interactables ::::::: Lunar Pod";

        [ConfigField("Choice Count", "", 2)]
        public static int choiceCount;

        [ConfigField("Max Spawns Per Stage", "", 1)]
        public static int maxSpawnsPerStage;

        [ConfigField("Director Credit Cost", "", 15)]
        public static int directorCreditCost;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var iscLunarPod = Utils.Paths.InteractableSpawnCard.iscLunarChest.Load<InteractableSpawnCard>();
            iscLunarPod.maxSpawnsPerStage = maxSpawnsPerStage;
            iscLunarPod.directorCreditCost = directorCreditCost;

            var lunarPod = Utils.Paths.GameObject.LunarChest.Load<GameObject>();
            Object.Destroy(lunarPod.GetComponent<ChestBehavior>());
            var optionChestBehavior = lunarPod.AddComponent<OptionChestBehavior>();

            var lunarPodDropTable = ScriptableObject.CreateInstance<LunarPodDropTable>();

            var lunarOptionPickup = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.OptionPickup.Load<GameObject>(), "LunarOptionPickup");
            var pickupPickerController = lunarOptionPickup.GetComponent<PickupPickerController>();
            pickupPickerController.contextString = "OPTION_PICKUP_INTERACTION_PROMPT_LUNAR";
            LanguageAPI.Add("OPTION_PICKUP_INTERACTION_PROMPT_LUNAR", "Open Lunar Potential");

            PrefabAPI.RegisterNetworkPrefab(lunarOptionPickup);

            optionChestBehavior.dropTable = lunarPodDropTable;
            // optionChestBehavior.dropTransform =
            optionChestBehavior.dropUpVelocityStrength = 20f;
            optionChestBehavior.dropForwardVelocityStrength = 3f;
            optionChestBehavior.openState = new(typeof(EntityStates.Barrel.OpeningLunar));
            optionChestBehavior.pickupPrefab = lunarOptionPickup;
            optionChestBehavior.numOptions = choiceCount;
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
                ItemDef itemDef = ItemCatalog.GetItemDef(index.itemIndex);
                EquipmentDef equipmentDef = EquipmentCatalog.GetEquipmentDef(index.equipmentIndex);
                if (itemDef && !string.IsNullOrEmpty(itemDef.name) && !itemDef.name.ToLower().Contains("replacement"))
                {
                    // Main.WRBLogger.LogError("Adding Item: " + Language.GetString(itemDef.nameToken));
                    weighted.AddChoice(index, 1f);
                }
                if (equipmentDef)
                {
                    // Main.WRBLogger.LogError("Adding Equipment: " + Language.GetString(equipmentDef.nameToken));
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