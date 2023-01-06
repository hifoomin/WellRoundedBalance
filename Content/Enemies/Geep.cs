using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Enemies
{
    internal class Geep : EnemyBase
    {
        public override string Name => ":: Enemies :: Geep";

        public override void Hooks()
        {
            var geep = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Gup/GeepBody.prefab").WaitForCompletion();
            var geepDeathRewards = geep.GetComponent<DeathRewards>();
            // get in a run then change

            var geepBody = geep.GetComponent<CharacterBody>();
            geepBody.baseMaxHealth = 250f;
            geepBody.levelMaxHealth = 75f;
        }
    }
}