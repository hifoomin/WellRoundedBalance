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
            projectileSimple.desiredForwardSpeed = 60f;
            projectileSimple.enableVelocityOverLifetime = true;
            projectileSimple.velocityOverLifetime = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 60f), new Keyframe(12f, 90f));

            var projectileSingleTargetImpact = prefab.GetComponent<ProjectileSingleTargetImpact>();

            var newImpact = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.OmniExplosionVFXQuick.Load<GameObject>(), "Gxp Spike Impact", false);
            var effectComponent = newImpact.GetComponent<EffectComponent>();
            effectComponent.soundName = "Play_gup_step";

            projectileSingleTargetImpact.impactEffect = newImpact;

            ContentAddition.AddEffect(newImpact);

            var projectileController = prefab.GetComponent<ProjectileController>();

            var newGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.FireballGhost.Load<GameObject>(), "Gxp Spike Ghost", false);
            newGhost.transform.localScale = new Vector3(3f, 3f, 3f);

            var theLightCantSeemToUnderstandItCantSeemToKnoooow = newGhost.transform.GetChild(1).GetComponent<Light>();
            theLightCantSeemToUnderstandItCantSeemToKnoooow.color = new Color32(242, 151, 0, 255);
            theLightCantSeemToUnderstandItCantSeemToKnoooow.range = 7f;
            theLightCantSeemToUnderstandItCantSeemToKnoooow.intensity = 2f;

            var flames = newGhost.transform.GetChild(0).GetComponent<ParticleSystemRenderer>();

            flames.renderMode = ParticleSystemRenderMode.Mesh;

            var newMat = Object.Instantiate(Utils.Paths.Material.matGenericFire.Load<Material>());

            newMat.SetColor("_TintColor", new Color32(72, 5, 3, 255));
            newMat.SetTexture("_MainTex", Utils.Paths.Texture2D.texMageBoltMask.Load<Texture2D>());

            flames.material = newMat;

            prefab.transform.localScale = new Vector3(0.15f, 0.15f, 3f);

            projectileController.ghostPrefab = newGhost;

            PrefabAPI.RegisterNetworkPrefab(prefab);
        }
    }
}