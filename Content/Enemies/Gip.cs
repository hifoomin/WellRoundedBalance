using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Enemies
{
    internal class Gip : EnemyBase
    {
        public override string Name => ":: Enemies ::: Gip";

        public override void Hooks()
        {
            var gip = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Gup/GipBody.prefab").WaitForCompletion();
            var gipDeathRewards = gip.GetComponent<DeathRewards>();
            // get in a run then change

            var gipBody = gip.GetComponent<CharacterBody>();
            gipBody.baseMaxHealth = 125f;
            gipBody.levelMaxHealth = 37.5f;
        }
    }
}