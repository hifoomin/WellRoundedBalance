using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace WellRoundedBalance.Enemies
{
    internal class SolusProbe : EnemyBase
    {
        public override string Name => ":: Enemies ::::::: Solus Probe";

        public override void Hooks()
        {
            var solus = Utils.Paths.GameObject.RoboBallMiniBody14.Load<GameObject>();
            var solusBody = solus.GetComponent<CharacterBody>();
            solusBody.baseDamage = 12f;
            solusBody.levelDamage = 2.4f;
        }
    }
}