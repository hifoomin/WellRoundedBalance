using UnityEngine;

namespace WellRoundedBalance.Projectiles
{
    public static class GupSpike
    {
        public static GameObject prefab;

        public static void Init()
        {
            prefab = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.Fireball.Load<GameObject>(), "Gxp Spike", true);

            foreach (Component component in prefab.GetComponents<Component>())
            {
                if (component is AkEvent || component is AkTriggerDisable)
                {
                    Object.Destroy(component);
                }
            }

            prefab.RemoveComponent<SphereCollider>();

            var capsuleCollider = prefab.AddComponent<CapsuleCollider>();
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 3f;

            var projectileSimple = prefab.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = 12f;
            projectileSimple.desiredForwardSpeed = 50f;

            var projectileController = prefab.AddComponent<ProjectileController>();

            var newGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.FireballGhost.Load<GameObject>(), "Gxp Spike Ghost", false);
            newGhost.transform.localScale = new Vector3(2f, 2f, 2f);

            var theLightCantSeemToUnderstandItCantSeemToKnoooow = newGhost.transform.GetChild(1).GetComponent<Light>();
            theLightCantSeemToUnderstandItCantSeemToKnoooow.color = new Color32(242, 151, 0, 255);
            theLightCantSeemToUnderstandItCantSeemToKnoooow.range = 7f;
            theLightCantSeemToUnderstandItCantSeemToKnoooow.intensity = 2f;

            var flames = newGhost.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();

            var newMat = Object.Instantiate(Utils.Paths.Material.matGenericFire.Load<Material>());

            newMat.SetColor("_TintColor", new Color32(243, 163, 84, 255));
            newMat.SetTexture("_MainTex", Utils.Paths.Texture2D.texMageBoltMask.Load<Texture2D>());

            flames.material = newMat;

            prefab.transform.localScale = new Vector3(0.1f, 0.1f, 2f);

            projectileController.ghostPrefab = newGhost;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}