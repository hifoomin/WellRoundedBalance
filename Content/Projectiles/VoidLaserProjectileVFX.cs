namespace WellRoundedBalance.Projectiles
{
    public static class VoidLaserProjectileVFX
    {
        public static GameObject laserPrefab;
        public static GameObject impactPrefab;

        public static void Create()
        {
            impactPrefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.LaserImpactEffect.Load<GameObject>(), "VoidtouchedSpinnyImpact", false);
            impactPrefab.transform.localScale = new Vector3(10f, 10f, 10f);

            laserPrefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.VoidRaidCrabSpinBeamVFX.Load<GameObject>(), "VoidtouchedSpinnyGhost", false);
            laserPrefab.transform.localScale = new Vector3(5f, 5f, 2.5f);
            laserPrefab.AddComponent<EffectComponent>();

            ContentAddition.AddEffect(impactPrefab);
            ContentAddition.AddEffect(laserPrefab);
        }
    }
}