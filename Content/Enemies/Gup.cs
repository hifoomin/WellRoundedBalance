using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Enemies
{
    internal class Gup : EnemyBase
    {
        public override string Name => "Enemies : Gup";

        public override void Hooks()
        {
            var gup = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Gup/GupBody.prefab").WaitForCompletion();
            var gupDeathRewards = gup.GetComponent<DeathRewards>();
            Main.WRBLogger.LogFatal("gup gold reward is" + gupDeathRewards.goldReward);
            Main.WRBLogger.LogFatal("gup gold reward fallback is" + gupDeathRewards.fallbackGold);

            var gupBody = gup.GetComponent<CharacterBody>();
            gupBody.baseMaxHealth = 500f;
            gupBody.levelMaxHealth = 150f;
        }
    }
}