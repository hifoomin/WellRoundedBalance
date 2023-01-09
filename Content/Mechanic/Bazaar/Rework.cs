using UnityEngine;
using UnityEngine.SceneManagement;

namespace WellRoundedBalance.Mechanic.Bazaar
{
    internal class Rework : GlobalBase
    {
        public static GameObject lunarPod;
        public static GameObject heresyStation;
        public override string Name => ":: Mechanic :::::::: Bazaar Rework";

        public override void Init()
        {
            lunarPod = Utils.Paths.GameObject.LunarChest.Load<GameObject>();
            heresyStation = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.LunarShopTerminal.Load<GameObject>(), "HeresyStation");
            var heresyDropTable = ScriptableObject.CreateInstance<BasicPickupDropTable>();
            heresyDropTable.tier1Weight = 0;
            heresyDropTable.tier2Weight = 0;
            heresyDropTable.tier3Weight = 0;
            heresyDropTable.bossWeight = 0;
            heresyDropTable.lunarItemWeight = 1;

            Object.Destroy(heresyStation.GetComponent<ShopTerminalBehavior>());

            var drops = new HeresyDropTable();

            var chestBehavior = heresyStation.AddComponent<ChestBehavior>();
            chestBehavior.dropTable = drops;

            PrefabAPI.RegisterNetworkPrefab(heresyStation);
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            if (SceneManager.GetActiveScene().name == "bazaar")
            {
                var lunarShop = GameObject.Find("HOLDER: Store").transform.GetChild(0);
                var table = lunarShop.GetChild(2);

                var objectList = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
                foreach (GameObject go in objectList)
                {
                    if (go.name.Contains("LunarShopTerminal"))
                    {
                        Object.Destroy(go);
                    }
                }

                var pod1 = Object.Instantiate(lunarPod, table);
                pod1.transform.localPosition = new Vector3(1.695f, -1.307f, 1.196f);
                var pod2 = Object.Instantiate(lunarPod, table);
                pod2.transform.localPosition = new Vector3(0.315f, -2.07f, 1.196f);
                var pod3 = Object.Instantiate(lunarPod, table);
                pod3.transform.localPosition = new Vector3(-1.106f, -1.681f, 1.196f);

                var table2 = Object.Instantiate(table, lunarShop);
                table2.transform.localPosition = new Vector3(11f, -7.5f, 9f);
                table2.transform.eulerAngles = new Vector3(270f, 250f, 0f);

                var object1 = Object.Instantiate(heresyStation, table2);
                object1.transform.localPosition = new Vector3(1.695f, -1.307f, 1.196f);
                var object2 = Object.Instantiate(heresyStation, table2);
                object2.transform.localPosition = new Vector3(0.315f, -2.07f, 1.196f);

                var slab = lunarShop.GetChild(3).gameObject;
                slab.SetActive(false);

                orig(self);
            }
        }
    }

    public class HeresyDropTable : PickupDropTable
    {
        public WeightedSelection<PickupIndex> weighted = new();

        public override int GetPickupCount()
        {
            return weighted.Count;
        }

        public override void Regenerate(Run run)
        {
            base.Regenerate(run);
            weighted.Clear();
            weighted.AddChoice(RoR2Content.Items.LunarPrimaryReplacement.CreatePickupDef().pickupIndex, 1f);
            weighted.AddChoice(RoR2Content.Items.LunarSecondaryReplacement.CreatePickupDef().pickupIndex, 1f);
            weighted.AddChoice(RoR2Content.Items.LunarUtilityReplacement.CreatePickupDef().pickupIndex, 1f);
            weighted.AddChoice(RoR2Content.Items.LunarSpecialReplacement.CreatePickupDef().pickupIndex, 1f);
        }

        public override PickupIndex[] GenerateUniqueDropsPreReplacement(int maxDrops, Xoroshiro128Plus rng)
        {
            return GenerateUniqueDropsFromWeightedSelection(maxDrops, rng, weighted);
        }

        public override PickupIndex GenerateDropPreReplacement(Xoroshiro128Plus rng)
        {
            return GenerateDropFromWeightedSelection(rng, weighted);
        }
    }
}