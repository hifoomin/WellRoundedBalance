using RoR2;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactable
{
    public class Scrapper : InteractableBase
    {
        public override string Name => "Interactables ::::::::: Scrapper";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var scrapper = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/Scrapper/iscScrapper.asset").WaitForCompletion();
            scrapper.maxSpawnsPerStage = 0;
        }
    }
}