using WellRoundedBalance.Misc;

namespace WellRoundedBalance.Projectiles
{
    public static class EarthQuakeWave
    {
        public static GameObject prefab;

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.BrotherSunderWave.Load<GameObject>(), "EarthquakeWave", false);
            prefab.AddComponent<DestroyStuckObject>();
            var projectileDamage = prefab.GetComponent<ProjectileDamage>();
            projectileDamage.damageType = DamageType.Generic;

            // var characterController = prefab.GetComponent<CharacterController>();
            // characterController.slopeLimit = 70f;

            var projectileCharacterController = prefab.GetComponent<ProjectileCharacterController>();
            projectileCharacterController.velocity = 35f;
            // projectileCharacterController.lifetime = 2f;

            var projectileOverlapAttack = prefab.GetComponent<ProjectileOverlapAttack>();
            projectileOverlapAttack.forceVector = new Vector3(0f, 0f, 0f);

            var newGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.BrotherSunderWaveGhost.Load<GameObject>(), "EarthquakeWaveGhost", false);
            var @base = newGhost.transform.GetChild(0);
            var infection = @base.GetChild(0).GetComponent<ParticleSystemRenderer>();
            infection.gameObject.SetActive(false);

            var hitboxReal = prefab.transform.GetChild(0);
            hitboxReal.transform.localScale = new Vector3(30f, 1.33f, 1.1f);

            var hitboxFake = newGhost.transform.GetChild(1);
            hitboxFake.localScale = new Vector3(30f, 1.33f, 1.1f);

            var mr = hitboxFake.GetComponent<MeshRenderer>();
            mr.material.SetTexture("_RemapTex", Main.wellroundedbalance.LoadAsset<Texture2D>("Assets/WellRoundedBalance/texRampInspire.png"));

            var projectileController = prefab.GetComponent<ProjectileController>();
            projectileController.flightSoundLoop = null;
            projectileController.ghostPrefab = newGhost;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}