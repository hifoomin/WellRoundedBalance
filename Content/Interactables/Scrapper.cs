using EntityStates.Scrapper;

namespace WellRoundedBalance.Interactables
{
    public class Scrapper : InteractableBase
    {
        public override string Name => ":: Interactables ::::: Scrapper";

        [ConfigField("Max Spawns Per Stage", "", 1)]
        public static int maxSpawnsPerStage;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var scrapper = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Scrapper/iscScrapper.asset").WaitForCompletion();
            scrapper.maxSpawnsPerStage = maxSpawnsPerStage;
            scrapper.directorCreditCost = 0;

            On.EntityStates.Scrapper.ScrappingToIdle.OnEnter += ScrappingToIdle_OnEnter;
        }

        private void ScrappingToIdle_OnEnter(On.EntityStates.Scrapper.ScrappingToIdle.orig_OnEnter orig, ScrappingToIdle self)
        {
            Util.PlaySound(ScrappingToIdle.enterSoundString, self.gameObject);
            self.PlayAnimation("Base", "ScrappingToIdle", "Scrapping.playbackRate", ScrappingToIdle.duration);
            if (ScrappingToIdle.muzzleflashEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(ScrappingToIdle.muzzleflashEffectPrefab, self.gameObject, ScrappingToIdle.muzzleString, false);
            }
            if (!NetworkServer.active)
            {
                return;
            }
            self.foundValidScrap = true;
            self.scrapperController.itemsEaten++;
        }
    }
}