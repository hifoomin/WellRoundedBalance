using WellRoundedBalance.Items;
using WellRoundedBalance.Items.Yellows;

namespace WellRoundedBalance.Projectiles
{
    public static class TitanFist
    {
        public static GameObject prefab;

        public static void Create()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.TitanPreFistProjectile.Load<GameObject>(), "TitanicKnurlFistProjectile");
            var projectileImpactExplosion = prefab.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.blastProcCoefficient = TitanicKnurl.procCoefficient * ItemBase.globalProc;
            projectileImpactExplosion.bonusBlastForce = new Vector3(0f, 3500f, 0f);
            prefab.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

            var projectileController = prefab.GetComponent<ProjectileController>();
            var newGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.TitanPreFistGhost.Load<GameObject>(), "TitanicKnurlFistGhost", false);

            newGhost.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);

            projectileController.ghostPrefab = newGhost;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}