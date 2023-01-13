﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

namespace WellRoundedBalance.Mechanic.Bazaar
{
    internal class Rework : MechanicBase
    {
        public static GameObject lunarPod;
        public static GameObject heresyStation;
        public override string Name => ":: Mechanic :::::::: Bazaar Rework";

        public override void Init()
        {
            lunarPod = Utils.Paths.GameObject.LunarChest.Load<GameObject>();
            heresyStation = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.LunarShopTerminal.Load<GameObject>(), "HeresyStation");

            var drops = ScriptableObject.CreateInstance<HeresyDropTable>();

            var shopBehavior = heresyStation.GetComponent<ShopTerminalBehavior>();
            shopBehavior.dropTable = drops;

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

                
                List<PurchaseInteraction> interactions = GameObject.FindObjectsOfType<PurchaseInteraction>().Where(x => x.gameObject.name.Contains("LunarShopTerminal")).ToList();
                for (int i = 0; i < 5; i++) {
                    if (i == 0 || i == 3) {
                        GameObject.Destroy(interactions[i].gameObject);
                        continue;
                    }

                    Vector3 position = interactions[i].transform.position;
                    GameObject.Destroy(interactions[i].gameObject);
                    GameObject.Instantiate(lunarPod, position, Quaternion.identity);
                }

                var table2 = Object.Instantiate(table, lunarShop);


                table2.transform.localPosition = new Vector3(11f, -7.5f, 9f);
                table2.transform.eulerAngles = new Vector3(270f, 250f, 0f);

                var object1 = Object.Instantiate(heresyStation, table2);
                object1.transform.localPosition = new Vector3(1.695f, -1.307f, 1.196f);
                var object2 = Object.Instantiate(heresyStation, table2);
                object2.transform.localPosition = new Vector3(0.315f, -2.07f, 1.196f);

                var slab = lunarShop.GetChild(3).gameObject;
                slab.SetActive(false);
            }
            orig(self);
        }
    }

    public class HeresyDropTable : PickupDropTable
    {
        public WeightedSelection<PickupIndex> weighted = new();

        public override int GetPickupCount()
        {
            return weighted.Count;
        }

        public void GenerateWeightedSelection() {
            weighted.Clear();
            weighted.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarPrimaryReplacement.itemIndex), 1f);
            weighted.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarSecondaryReplacement.itemIndex), 1f);
            weighted.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarUtilityReplacement.itemIndex), 1f);
            weighted.AddChoice(PickupCatalog.FindPickupIndex(RoR2Content.Items.LunarSpecialReplacement.itemIndex), 1f);
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