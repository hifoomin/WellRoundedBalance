using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Enemies
{
    internal class Geep : EnemyBase
    {
        public override string Name => "Enemies :: Geep";

        public override void Hooks()
        {
            var geep = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Gup/GeepBody.prefab").WaitForCompletion();
            var geepDeathRewards = geep.GetComponent<DeathRewards>();
            Main.WRBLogger.LogFatal("gup gold reward is" + geepDeathRewards.goldReward);
            Main.WRBLogger.LogFatal("gup gold reward fallback is" + geepDeathRewards.fallbackGold);

            var geepBody = geep.GetComponent<CharacterBody>();
            geepBody.baseMaxHealth = 250f;
            geepBody.levelMaxHealth = 75f;
        }
    }
}