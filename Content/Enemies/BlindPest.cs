using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Enemies
{
    internal class BlindPest : EnemyBase
    {
        public override string Name => "Enemies :::: Blind Pest";

        public override void Hooks()
        {
            var blindPest = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/FlyingVermin/FlyingVerminBody.prefab").WaitForCompletion();
            var blindPestBody = blindPest.GetComponent<CharacterBody>();
            blindPestBody.baseDamage = 10f;
            blindPestBody.levelDamage = 2f;
        }
    }
}