using MonoMod.Cil;
using System;
using RoR2.UI;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;
using BepInEx;
using RoR2.Hologram;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using IL.RoR2.Navigation;
using HG;

namespace WellRoundedBalance.Interactables
{
    internal class HalcyonShrine : InteractableBase<HalcyonShrine>
    {
        public override string Name => ":: Interactables :::::::: Halcyon Shrine";
        public static GameObject LightningStorm;
        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var shrine = Paths.GameObject.ShrineHalcyonite;
            shrine.GetComponent<HologramProjector>().enabled = true;
            shrine.GetComponent<HologramProjector>().hologramPivot.transform.localPosition = new Vector3(0.1f, 4.5f, 0f);
            shrine.AddComponent<HalcyonShrineController>();
            shrine.RemoveComponents<PortalSpawner>();
            
            var pi = shrine.GetComponent<PurchaseInteraction>();
            pi.cost = 150;
            pi.costType = CostTypeIndex.Money;

            var director = shrine.GetComponent<CombatDirector>();
            director.monsterCards = Paths.DirectorCardCategorySelection.dccsHalcyoniteShrineHalcyonite;
            director.moneyWaveIntervals = new RangeFloat[] {
                new() {
                    min = 1, max = 1
                }
            };
            director.shouldSpawnOneWave = false;
            director.moneyWaves = new CombatDirector.DirectorMoneyWave[] {
                new CombatDirector.DirectorMoneyWave() {
                    interval = 0.5f,
                    multiplier = 80f,
                },
            };
            Paths.DirectorCardCategorySelection.dccsHalcyoniteShrineHalcyonite.categories[0].cards[0].spawnCard = Paths.CharacterSpawnCard.cscHalcyonite;

            /*On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteActivatedState.OnEnter += (orig, self) => {
                Util.PlaySound("Play_obj_shrineHalcyonite_activate", self.gameObject);
            };*/
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteNoQuality.OnEnter += (orig, self) => {};
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteNoQuality.OnExit += (orig, self) => {};
            On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteBaseState.FixedUpdate += (orig, self) => {};
            // On.EntityStates.ShrineHalcyonite.ShrineHalcyoniteActivatedState.FixedUpdate += (orig, self) => {};
            On.RoR2.LightningStormController.FireLightningBolt_Vector3 += DontAllowLightningInTP;
            On.RoR2.GlobalEventManager.HandleDamageWithNoAttacker += ForceLunarRuin;
            SceneManager.activeSceneChanged += (o, n) => {
                HalcyonShrineController.isActive = false;
            };

            LightningStorm = PrefabAPI.InstantiateClone(Paths.GameObject.LightningStormController, "HSLightningStorm");
            var ppv1 = LightningStorm.transform.Find("PostProcess, In + Run").GetComponent<PostProcessVolume>();
            var ppv2 = LightningStorm.transform.Find("PostProcess, Out").GetComponent<PostProcessVolume>();
            var storm = Paths.PostProcessProfile.ppSceneArenaSick;
            ppv1.sharedProfile = storm;
            ppv1.profile = storm;
            ppv2.sharedProfile = storm;
            ppv2.profile = storm;
            ContentAddition.AddNetworkedObject(LightningStorm);
            PrefabAPI.RegisterNetworkPrefab(LightningStorm);

            Paths.InteractableSpawnCard.iscShrineHalcyonite.maxSpawnsPerStage = 0;
            Paths.InteractableSpawnCard.iscShrineHalcyoniteTier1.maxSpawnsPerStage = 0;

            On.RoR2.AccessCodesMissionController.OnEnable += SpawnHalcyonShrine;
            On.RoR2.AccessCodesMissionController.ActivateCode += DisableShrine;
        }

        private void DisableShrine(On.RoR2.AccessCodesMissionController.orig_ActivateCode orig, AccessCodesMissionController self)
        {
            orig(self);

            if (HalcyonShrineController.instance && NetworkServer.active) {
                HalcyonShrineController.instance.purchaseInteraction.SetAvailable(false);
            }
        }

        private void SpawnHalcyonShrine(On.RoR2.AccessCodesMissionController.orig_OnEnable orig, AccessCodesMissionController self)
        {
            orig(self);
            if (NetworkServer.active) {
                DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(Paths.InteractableSpawnCard.iscShrineHalcyonite, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Random
                }, Run.instance.stageRng));
            }
        }

        private void ForceLunarRuin(On.RoR2.GlobalEventManager.orig_HandleDamageWithNoAttacker orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (HalcyonShrineController.isActive && damageInfo.damageType.damageTypeExtended.HasFlag(DamageTypeExtended.LunarRuin) && damageInfo.damageType.damageTypeExtended.HasFlag(DamageTypeExtended.ApplyBuffPermanently)) {
                damageInfo.damageType.damageTypeExtended &= ~DamageTypeExtended.ApplyBuffPermanently;
            }

            orig(self, damageInfo, victim);
        }

        private void DontAllowLightningInTP(On.RoR2.LightningStormController.orig_FireLightningBolt_Vector3 orig, Vector3 position)
        {
            if (TeleporterInteraction.instance && TeleporterInteraction.instance.isCharging) {
                if (HoldoutZoneController.IsPointInChargingRadius(TeleporterInteraction.instance.holdoutZoneController,
                    TeleporterInteraction.instance.holdoutZoneController.transform.position,
                    TeleporterInteraction.instance.holdoutZoneController.currentRadius * TeleporterInteraction.instance.holdoutZoneController.currentRadius,
                    position
                )) {
                    return;
                }     
            }

            orig(position);
        }

        public class HalcyonShrineController : MonoBehaviour {
            public PurchaseInteraction purchaseInteraction;
            private CombatDirector director;
            public static bool isActive = false;
            private GameObject storm;
            private float[] lightningTimers = [0f, 0f, 0f];
            private float[] lightningDelays = [0.7f, 1.6f, 2.2f];
            private float positionTimer = 0f;
            private Vector3[] positions = [];
            private bool doLightning = false;
            public static HalcyonShrineController instance;
            public void OnEnable() {
                director = base.GetComponent<CombatDirector>();
                purchaseInteraction = base.GetComponent<PurchaseInteraction>();
                purchaseInteraction.onDetailedPurchaseServer.RemoveAllListeners();
                purchaseInteraction.onPurchase.RemoveAllListeners();
                purchaseInteraction.onDetailedPurchaseServer.AddListener(OnPurchase);

                TeleporterInteraction.onTeleporterBeginChargingGlobal += OnChargeStart;
                TeleporterInteraction.onTeleporterChargedGlobal += OnTPFinish;
                
                if (NetworkServer.active) {
                    storm = GameObject.Instantiate(LightningStorm);
                    NetworkServer.Spawn(storm);
                }

                instance = this;
            }

            private void FixedUpdate() {
                if (doLightning && NetworkServer.active) {
                    for (int i = 0; i < 3; i++) {
                        lightningTimers[i] += Time.fixedDeltaTime;

                        if (lightningTimers[i] >= lightningDelays[i] && positions.Length > 0) {
                            lightningTimers[i] = 0f;
                            int count = UnityEngine.Random.Range(0, 3);
                            StartCoroutine(ProcessSpawns(count));
                        }
                    }

                    positionTimer += Time.fixedDeltaTime;
                    if (positionTimer >= 2.5f) {
                        positionTimer = 0f;

                        var nodes = SceneInfo.instance.groundNodes.nodes.Where(x => Vector3.Distance(x.position, TeleporterInteraction.instance.holdoutZoneController.transform.position) > TeleporterInteraction.instance.holdoutZoneController.currentRadius * 1.5f).ToArray();
                        positions = new Vector3[nodes.Length];
                        for (int i = 0; i < positions.Length; i++) {
                            positions[i] = nodes[i].position;
                        }
                    }
                }
            }

            private IEnumerator ProcessSpawns(int count) {
                for (int j = 0; j < count; j++) {
                    LightningStormController.FireLightningBolt(HG.ArrayUtils.GetRandom(positions));
                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();
                }
            }

            private void OnTPFinish(TeleporterInteraction interaction)
            {
                if (isActive) {
                    director.enabled = false;

                    if (storm && NetworkServer.active) {
                        storm.GetComponent<LightningStormController>().ServerSetStormActive(false);
                        doLightning = false;
                    }

                    if (NetworkServer.active) {
                        DropItems();
                    }

                    isActive = false;
                }
            }

            private void OnChargeStart(TeleporterInteraction interaction)
            {
                purchaseInteraction.available = false;

                if (isActive) {
                    if (NetworkServer.active) {
                        if (storm) {
                            storm.GetComponent<LightningStormController>().ServerSetStormActive(true);
                            doLightning = true;
                        }
                        
                        director.enabled = true;
                        director.SetMonsterCredit(400 * (1 + (0.5f * (Run.instance.participatingPlayerCount - 1))));
                        director.currentSpawnTarget = interaction.gameObject;
                    }

                    var fx = base.transform.Find("meshHalcyoniteShrineStorm");
                    fx.SetParent(interaction.transform
                    .Find("TeleporterBaseMesh").Find("BuiltInEffects").Find("ChargingEffect").Find("RadiusScaler").Find("ClearAreaIndicator"));
                    fx.transform.localPosition = Vector3.zero;
                    fx.gameObject.SetActive(true);
                    fx.transform.localScale = new Vector3(0.0069f, 0.0069f, 0.0069f);
                }
            }

            public void OnPurchase(CostTypeDef.PayCostContext context, CostTypeDef.PayCostResults results) {
                if (NetworkServer.active) {
                    foreach (PortalSpawner portal in TeleporterInteraction.instance.portalSpawners) {
                        if (portal.portalSpawnCard.name == "iscColossusPortal" && portal.previewChild && portal.previewChild.activeSelf == false) {
                            portal.spawnChance = 1f;
                            portal.minStagesCleared = 0;
                            portal.validStages = [];
                            portal.invalidStages = [];
                            portal.validStageTiers = [];
                            portal.Start();
                        }
                    }


                    isActive = true;
                    TeleporterInteraction.instance.AddShrineStack();
                    TeleporterInteraction.instance.bossGroup.bonusRewardCount--;

                    if (AccessCodesMissionController.instance) {
                        AccessCodesMissionController.instance.TurnOffNodes(TeleporterInteraction.instance);
                    }
                }
            }

            public void DropItems() {
                int num = Run.instance.AsValidOrNull()?.participatingPlayerCount ?? 0;
                HalcyoniteShrineInteractable self = GetComponent<HalcyoniteShrineInteractable>();
                self.rewardDropTable = self.halcyoniteDropTableTier3;
                if (num <= 0 || !self.rewardDropTable)
                {
                    return;
                }

                float angle = 360f / (float)num;
                Vector3 vector = Quaternion.AngleAxis(UnityEngine.Random.Range(0, 360), Vector3.up) * (Vector3.up * 40f + Vector3.forward * 5f);
                Quaternion quaternion = Quaternion.AngleAxis(angle, Vector3.up);
                Vector3 position = TeleporterInteraction.instance.transform.position + self.rewardOffset;
                int num3 = 0;
                while (num3 < num)
                {
                    if (HalcyoniteShrineInteractable.isCommandEnabled)
                    {
                        int num4 = 3;
                        for (int i = 0; i < num4; i++)
                        {
                            UniquePickup pickup = self.rewardDropTable.GeneratePickup(Run.instance.treasureRng);
                            GenericPickupController.CreatePickupInfo createPickupInfo = default(GenericPickupController.CreatePickupInfo);
                            createPickupInfo.pickup = pickup;
                            createPickupInfo.rotation = Quaternion.identity;
                            createPickupInfo.position = position;
                            GenericPickupController.CreatePickupInfo pickupInfo = createPickupInfo;
                            PickupDropletController.CreatePickupDroplet(pickupInfo, pickupInfo.position, vector);
                        }
                    }
                    else
                    {
                        GenericPickupController.CreatePickupInfo createPickupInfo = default(GenericPickupController.CreatePickupInfo);
                        createPickupInfo.pickup = new UniquePickup(PickupCatalog.FindPickupIndex(self.rewardDisplayTier));
                        createPickupInfo.pickerOptions = PickupPickerController.GenerateOptionsFromDropTable(5, self.halcyoniteDropTableTier3, Run.instance.treasureRng);
                        createPickupInfo.rotation = Quaternion.identity;
                        createPickupInfo.position = position;
                        createPickupInfo.prefabOverride = self.rewardPickupPrefab;
                        PickupDropletController.CreatePickupDroplet(createPickupInfo, position, vector);
                    }
                    num3++;
                    vector = quaternion * vector;
                }
            }
        }
    }
}