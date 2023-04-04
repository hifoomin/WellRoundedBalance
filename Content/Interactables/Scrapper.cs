using EntityStates.Scrapper;
using RoR2.Hologram;

namespace WellRoundedBalance.Interactables
{
    public class Scrapper : InteractableBase<Scrapper>
    {
        public override string Name => ":: Interactables ::::: Scrapper";

        [ConfigField("Max Spawns Per Stage", "", 1)]
        public static int maxSpawnsPerStage;

        [ConfigField("Max Uses", "", 2)]
        public static int maxUses;

        [ConfigField("Max Scrap Count Per Use", "", 1)]
        public static int maxScrapCountPerUse;

        [ConfigField("Weight Multiplier", "", 0.5f)]
        public static float weightMultiplier;

        public static Dictionary<GameObject, int> uses;

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
            On.RoR2.ScrapperController.Start += ScrapperController_Start;
            On.RoR2.ClassicStageInfo.Start += ClassicStageInfo_Start;
        }

        private void ClassicStageInfo_Start(On.RoR2.ClassicStageInfo.orig_Start orig, ClassicStageInfo self)
        {
            orig(self);
            var scrapper = Utils.Paths.InteractableSpawnCard.iscScrapper.Load<InteractableSpawnCard>();
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

        private void ScrapperController_Start(On.RoR2.ScrapperController.orig_Start orig, ScrapperController self)
        {
            self.maxItemsToScrapAtATime = maxScrapCountPerUse;
            orig(self);
        }

        private void Scrapping_OnEnter(On.EntityStates.Scrapper.Scrapping.orig_OnEnter orig, Scrapping self)
        {
            var scrapper = self.outer.gameObject;
            if (scrapper != null && uses.ContainsKey(scrapper))
            {
                uses[scrapper]--;
                var counter = self.outer.gameObject.GetComponent<ScrapperUseCounter>();
                if (counter)
                {
                    counter.useCount--;
                }
            }
            orig(self);
        }

        private void Stage_onServerStageComplete(Stage stage)
        {
            uses.Clear();
        }

        private void ScrapperBaseState_OnEnter(On.EntityStates.Scrapper.ScrapperBaseState.orig_OnEnter orig, ScrapperBaseState self)
        {
            var scrapper = self.outer.gameObject;
            if (!uses.ContainsKey(scrapper))
            {
                uses.Add(scrapper, maxUses);
            }
            orig(self);
            if (uses[scrapper] <= 0)
            {
                self.outer.GetComponent<PickupPickerController>().SetAvailable(false);
            }
        }
    }

    public class ScrapperUseCounter : MonoBehaviour
    {
        public int useCount;
        public float timer;
        public float explosionInterval = 0.4f;
        public float deleteInterval = 0.5f;

        private void FixedUpdate()
        {
            if (useCount <= 0)
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
                    gameObject.SetActive(false);
                }
            }
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

        public void UpdateHologramContent(GameObject self)
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