using UnityEngine.SceneManagement;

namespace WellRoundedBalance.Mechanics.Bazaar
{
    internal class Rework : MechanicBase<Rework>
    {
        public static GameObject lunarPod;
        public static GameObject heresyStation;
        public override string Name => ":: Mechanics :::::::: Bazaar Rework";

        public override void Init()
        {
            lunarPod = Utils.Paths.GameObject.LunarChest.Load<GameObject>();
            heresyStation = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.LunarShopTerminal.Load<GameObject>(), "HeresyStation");

            var heresyDropTable = ScriptableObject.CreateInstance<HeresyDropTable>();

            var shopBehavior = heresyStation.GetComponent<ShopTerminalBehavior>();
            shopBehavior.dropTable = heresyDropTable;

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
                table.gameObject.AddComponent<NetworkIdentity>();

                List<PurchaseInteraction> interactions = GameObject.FindObjectsOfType<PurchaseInteraction>().Where(x => x.gameObject.name.Contains("LunarShopTerminal")).ToList();
                for (int i = 0; i < 5; i++)
                {
                    if (i == 1 || i == 5)
                    {
                        GameObject.Destroy(interactions[i].gameObject);
                        continue;
                    }

                    Vector3 position = interactions[i].transform.position;
                    GameObject.Destroy(interactions[i].gameObject);
                    var lunarPodServer = Object.Instantiate(lunarPod, position, Quaternion.identity);
                    NetworkServer.Spawn(lunarPodServer);
                }

                var table2Server = Object.Instantiate(table, lunarShop);
                table2Server.transform.localPosition = new Vector3(11f, -7.5f, 9f);
                table2Server.transform.eulerAngles = new Vector3(270f, 250f, 0f);
                NetworkServer.Spawn(table2Server.gameObject);

                var heresyItemServer = Object.Instantiate(heresyStation, table2Server);
                heresyItemServer.transform.localPosition = new Vector3(1.695f, -1.307f, 1.196f);
                heresyItemServer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                NetworkServer.Spawn(heresyItemServer);

                var heresyItemServer2 = Object.Instantiate(heresyStation, table2Server);
                heresyItemServer2.transform.localPosition = new Vector3(0.315f, -2.07f, 1.196f);
                heresyItemServer2.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                NetworkServer.Spawn(heresyItemServer2);

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

        public void GenerateWeightedSelection()
        {
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
            // Debug.Log(GenerateDropFromWeightedSelection(rng, weighted));
            return GenerateDropFromWeightedSelection(rng, weighted);
        }
    }
}