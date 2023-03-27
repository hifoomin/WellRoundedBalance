using EntityStates.Scrapper;

namespace WellRoundedBalance.Interactables
{
    public class Scrapper : InteractableBase
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
            }
            orig(self);
        }

        private void Stage_onServerStageComplete(Stage stage)
        {
            uses.Clear();
        }

        public static int GetUses(GameObject gameObject)
        {
            if (uses.ContainsKey(gameObject) && uses[gameObject] > 0)
            {
                return uses[gameObject];
            }
            return -1;
        }

        public static void InitUses(GameObject self, int use)
        {
            if (!NetworkServer.active) return;
            if (!uses.ContainsKey(self))
            {
                uses.Add(self, use);
            }
            else uses[self] = use;
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
}