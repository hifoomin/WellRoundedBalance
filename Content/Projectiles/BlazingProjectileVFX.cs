namespace WellRoundedBalance.Projectiles
{
    public static class BlazingProjectileVFX
    {
        public static GameObject prefab;

        public static void Create()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.FireballGhost.Load<GameObject>(), "BlazingProjectileGhost", false);
            prefab.transform.localScale = new Vector3(2f, 2f, 2f);
            var flames = prefab.transform.GetChild(0);
            var flamesParticleSystemRenderer = flames.GetComponent<ParticleSystemRenderer>();
            flamesParticleSystemRenderer.material.SetColor("_TintColor", new Color32(255, 134, 105, 255));
        }
    }
}