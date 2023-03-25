namespace WellRoundedBalance.Artifacts.New
{
    internal class Arrangement : ArtifactAddBase
    {
        public override string ArtifactName => "Arrangement";

        public override string ArtifactLangTokenName => "ARRANGEMENT";

        public override string ArtifactDescription => "Category chests are much more common. Category chests have unique spawn rates from each other.";

        public override Sprite ArtifactEnabledIcon => Main.wellroundedbalance.LoadAsset<Sprite>("texBuffHappiestMaskReady.png");

        public override Sprite ArtifactDisabledIcon => Main.wellroundedbalance.LoadAsset<Sprite>("texBuffDelicateWatchIcon.png");

        public override string Name => ":: Artifacts ::::::::::::::::: Arrangement";

        public override void Hooks()
        {
            Run.onRunStartGlobal += Run_onRunStartGlobal;
        }

        private void Run_onRunStartGlobal(Run run)
        {
            if (ArtifactEnabled)
            {
                On.RoR2.ClassicStageInfo.Start += ClassicStageInfo_Start;
            }
            else
            {
                On.RoR2.ClassicStageInfo.Start -= ClassicStageInfo_Start;
            }
        }

        private void ClassicStageInfo_Start(On.RoR2.ClassicStageInfo.orig_Start orig, ClassicStageInfo self)
        {
            orig(self);
            var categories = self.interactableCategories.categories;
            var chest = Utils.Paths.InteractableSpawnCard.iscChest1.Load<InteractableSpawnCard>();
            float chestWeight = 1f;
            int categoryChestsFound = 0;
            int totalCategoryChests = GetTotalCategoryChestCount(categories);

            for (int i = 0; i < categories.Length && categoryChestsFound < totalCategoryChests; i++)
            {
                var categoryIndex = categories[i];
                for (int j = 0; j < categoryIndex.cards.Length && categoryChestsFound < totalCategoryChests; j++)
                {
                    var cardIndex = categoryIndex.cards[j];
                    if (cardIndex.spawnCard == chest)
                    {
                        chestWeight = cardIndex.selectionWeight;
                    }
                    if (cardIndex.spawnCard.name.Contains("CategoryChest"))
                    {
                        Logger.LogError("Found CategoryChest " + cardIndex.spawnCard.name);
                        cardIndex.selectionWeight = Mathf.RoundToInt(cardIndex.selectionWeight * chestWeight * Run.instance.treasureRng.RangeFloat(3.5f, 8f));
                        categoryChestsFound++;
                    }
                }
            }
        }

        private int GetTotalCategoryChestCount(DirectorCardCategorySelection.Category[] categories)
        {
            int count = 0;
            for (int i = 0; i < categories.Length; i++)
            {
                var categoryIndex = categories[i];
                for (int j = 0; j < categoryIndex.cards.Length; j++)
                {
                    var cardIndex = categoryIndex.cards[j];
                    if (cardIndex.spawnCard.name.Contains("CategoryChest"))
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public override void Init()
        {
            base.Init();
        }
    }
}