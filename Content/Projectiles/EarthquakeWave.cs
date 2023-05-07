namespace WellRoundedBalance.Projectiles
{
    public static class EarthQuakeWave
    {
        public static GameObject prefab;

        public static void Create()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.BrotherSunderWave.Load<GameObject>(), "EarthquakeWave", false);
            var projectileDamage = prefab.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;

            var projectileCharacterController = prefab.GetComponent<ProjectileCharacterController>();
            projectileCharacterController.velocity = 45f;

            var projectileOverlapAttack = prefab.GetComponent<ProjectileOverlapAttack>();
            projectileOverlapAttack.forceVector = new Vector3(0f, 0f, 0f);

            var newGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.BrotherSunderWaveGhost.Load<GameObject>(), "EarthquakeWaveGhost", false);
            var @base = newGhost.transform.GetChild(0);
            var infection = @base.GetChild(0).GetComponent<ParticleSystemRenderer>();
            infection.material.SetTexture("_EmTex", Main.wellroundedbalance.LoadAsset<Texture2D>("Assets/WellRoundedBalance/texRampWave.png"));
            infection.material.SetTexture("_MainTex", Main.wellroundedbalance.LoadAsset<Texture2D>("Assets/WellRoundedBalance/texRampWave.png"));

            var water = @base.GetChild(3);
            water.gameObject.SetActive(false);

            var hitbox = newGhost.transform.GetChild(1);
            hitbox.gameObject.transform.localScale = new Vector3(30f, 2f, 1f);
            hitbox.GetComponent<MeshRenderer>().enabled = false;
            var projectileController = prefab.GetComponent<ProjectileController>();
            projectileController.flightSoundLoop = null;
            projectileController.ghostPrefab = newGhost;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}