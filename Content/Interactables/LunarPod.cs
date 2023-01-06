using RoR2;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Interactables
{
    public class LunarPod : InteractableBase
    {
        public override string Name => ":: Interactables ::::::: Lunar Pod";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            var lunarPod = Addressables.LoadAssetAsync<InteractableSpawnCard>("RoR2/Base/LunarChest/iscLunarChest.asset").WaitForCompletion();
            lunarPod.maxSpawnsPerStage = 1;
            lunarPod.directorCreditCost = 15;
        }
    }
}