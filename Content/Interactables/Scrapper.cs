using System.Collections;
using EntityStates.Scrapper;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2.Hologram;
using UnityEngine;

namespace WellRoundedBalance.Interactables
{
    public class Scrapper : InteractableBase<Scrapper>
    {
        public override string Name => ":: Interactables ::::: Scrapper";

        [ConfigField("Max Spawns Per Stage", "", 1)]
        public static int maxSpawnsPerStage;

        [ConfigField("Max Uses", "", 3)]
        public static int maxUses;

        [ConfigField("Max Scrap Count Per Use", "", 1)]
        public static int maxScrapCountPerUse;

        [ConfigField("Weight Multiplier", "", 0.5f)]
        public static float weightMultiplier;

        public static Dictionary<GameObject, int> uses;

        public static InteractableSpawnCard scrapper = Utils.Paths.InteractableSpawnCard.iscScrapper.Load<InteractableSpawnCard>();

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var scrapper = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Scrapper/iscScrapper.asset").WaitForCompletion();
            scrapper.maxSpawnsPerStage = maxSpawnsPerStage;
            scrapper.directorCreditCost = 0;

            var scrapperGO = Utils.Paths.GameObject.Scrapper.Load<GameObject>();
            var counter = scrapperGO.AddComponent<ScrapperUseCounter>();
            counter.useCount = maxUses;
            var hologram = scrapperGO.AddComponent<HologramProjector>();
            hologram.displayDistance = 15f;
            hologram.hologramPivot = scrapperGO.transform.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(0); // head.end heheheha
            hologram.hologramPivot.transform.localScale *= 2f;
            hologram.hologramPivot.transform.localPosition += new Vector3(0f, 1f, 0f);
            hologram.disableHologramRotation = false;
            var hologram2 = scrapperGO.AddComponent<ScrapperHologram>();

            uses = new();

            Stage.onServerStageComplete += Stage_onServerStageComplete;
            On.EntityStates.Scrapper.ScrapperBaseState.OnEnter += ScrapperBaseState_OnEnter;
            On.EntityStates.Scrapper.Scrapping.OnEnter += Scrapping_OnEnter;
            On.RoR2.ScrapperController.AssignPotentialInteractor += ScrapperController_Start;
            On.RoR2.ClassicStageInfo.Start += ClassicStageInfo_Start;
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
            GlobalEventManager.OnInteractionsGlobal += GlobalEventManager_OnInteractionsGlobal;

            NetworkingAPI.RegisterMessageType<ScrapperUseSync>();
        }

        private void GlobalEventManager_OnInteractionsGlobal(Interactor interactor, IInteractable interactable, GameObject interactableObject)
        {
            if (!interactableObject)
            {
                return;
            }
            if (interactableObject.TryGetComponent<ScrapperUseCounter>(out var scrapperUseCounter))
            {
                if (scrapperUseCounter.useCount <= 0)
                {
                    EffectManager.SpawnEffect(Utils.Paths.GameObject.ExplosionVFX.Load<GameObject>(), new EffectData
                    {
                        origin = interactableObject.transform.position,
                        scale = 3f
                    }, true);
                    NetworkServer.Destroy(interactableObject);
                }
            }
        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            orig(self);
            if (!NetworkServer.active) {
                return;
            }

            ScrapperController[] scrappers = GameObject.FindObjectsOfType<ScrapperController>();
            foreach (ScrapperController controller in scrappers)
            {
                var counter = controller.gameObject.GetComponent<ScrapperUseCounter>();
                if (!counter) {
                    return;
                }
                counter.useCount = maxUses * Run.instance.participatingPlayerCount;
                counter.StartCoroutine(counter.WaitAndSync());
                new ScrapperUseSync(counter.gameObject, counter.useCount).Send(NetworkDestination.Clients);
            }
        }

        private void ClassicStageInfo_Start(On.RoR2.ClassicStageInfo.orig_Start orig, ClassicStageInfo self)
        {
            orig(self);
            if (NetworkServer.active && self.interactableCategories)
            {
                var categories = self.interactableCategories.categories;
                for (int i = 0; i < categories.Length; i++)
                {
                    var categoryIndex = categories[i];
                    for (int j = 0; j < categoryIndex.cards.Length; j++)
                    {
                        var cardIndex = categoryIndex.cards[j];
                        if (cardIndex.spawnCard == scrapper)
                        {
                            cardIndex.selectionWeight = Mathf.RoundToInt(cardIndex.selectionWeight * weightMultiplier);
                            break;
                        }
                    }
                }
            }
            
        }

        private void ScrapperController_Start(On.RoR2.ScrapperController.orig_AssignPotentialInteractor orig, ScrapperController self, Interactor interactor)
        {
            self.maxItemsToScrapAtATime = maxScrapCountPerUse;

            orig(self, interactor);
        }

        private void Scrapping_OnEnter(On.EntityStates.Scrapper.Scrapping.orig_OnEnter orig, Scrapping self)
        {
            orig(self);
            var scrapper = self.outer.gameObject;
            if (scrapper != null && NetworkServer.active)
            {
                var counter = self.outer.gameObject.GetComponent<ScrapperUseCounter>();

                if (counter)
                {
                    counter.useCount--;
                    new ScrapperUseSync(counter.gameObject, counter.useCount).Send(NetworkDestination.Clients);
                }
            }
        }

        private void Stage_onServerStageComplete(Stage stage)
        {
            uses.Clear();
        }

        private void ScrapperBaseState_OnEnter(On.EntityStates.Scrapper.ScrapperBaseState.orig_OnEnter orig, ScrapperBaseState self)
        {
            orig(self);
            var scrapper = self.outer.gameObject;
            var counter = self.outer.GetComponent<ScrapperUseCounter>();

            if (counter && counter.useCount <= 0)
            {
                self.outer.GetComponent<PickupPickerController>().SetAvailable(false);
                counter.shouldExplode = true;
            }
        }
    }

    public class ScrapperUseCounter : MonoBehaviour
    {
        public int useCount;
        public float timer;
        public float explosionInterval = 0.7f;
        public float deleteInterval = 0.8f;
        public bool shouldExplode = false;
        
        private void FixedUpdate()
        {
            if (useCount <= 0 && NetworkServer.active && shouldExplode)
            {
                timer += Time.fixedDeltaTime;
                if (timer >= explosionInterval)
                {
                    EffectManager.SpawnEffect(Utils.Paths.GameObject.ExplosionVFX.Load<GameObject>(), new EffectData
                    {
                        origin = transform.position,
                        scale = 3f
                    }, true);
                }
                if (timer >= deleteInterval)
                {
                    NetworkServer.Destroy(base.gameObject);
                }
            }
        }

        public IEnumerator WaitAndSync() {
            yield return new WaitForSeconds(5f);
            new ScrapperUseSync(this.gameObject, this.useCount).Send(NetworkDestination.Clients);
        }
        
    }

    public class ScrapperUseSync : INetMessage
    {
        public ScrapperUseCounter counter;
        public GameObject obj;
        public int uses;
        public void Deserialize(NetworkReader reader)
        {
            obj = reader.ReadGameObject();
            uses = reader.ReadInt32();
        }

        public void OnReceived()
        {
            obj.GetComponent<ScrapperUseCounter>().useCount = uses;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(obj);
            writer.Write(uses);
        }

        public ScrapperUseSync() {}
        public ScrapperUseSync(GameObject source, int newUses) {
            obj = source;
            uses = newUses;
        }
    }

    public class ScrapperHologram : MonoBehaviour, IHologramContentProvider
    {
        public ScrapperUseCounter counter;

        private void Start()
        {
            counter = gameObject.GetComponent<ScrapperUseCounter>();
        }

        public GameObject GetHologramContentPrefab()
        {
            return PlainHologram.hologramContentPrefab;
        }

        public bool ShouldDisplayHologram(GameObject viewer)
        {
            var distance = Vector3.Distance(viewer.transform.position, gameObject.transform.position);
            if (distance <= 15f)
            {
                return true;
            }
            return false;
        }

        public void UpdateHologramContent(GameObject self, Transform viewerBody)
        {
            var hologram = self.GetComponent<PlainHologram.PlainHologramContent>();
            if (hologram)
            {
                hologram.text = counter.useCount + (counter.useCount == 1 ? " use left" : " uses left");
                hologram.color = Color.white;
            }
        }
    }
}