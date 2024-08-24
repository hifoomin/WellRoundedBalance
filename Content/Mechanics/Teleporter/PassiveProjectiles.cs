namespace WellRoundedBalance.Mechanics.Teleporter
{
    internal class PassiveProjectiles : MechanicBase<PassiveProjectiles>
    {
        public override string Name => ":: Mechanics ::::::::::::::::: Teleporter Passive Projectiles";

        [ConfigField("Seconds To Complete A Full Revolution", "", 7f)]
        public static float rotationTime;

        [ConfigField("Projectile Firing Interval", "", 1f)]
        public static float projectileInterval;

        public static GameObject teleporterProjectile;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            teleporterProjectile = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.ArtifactShellSeekingSolarFlare.Load<GameObject>(), "Teleporter Projectile");

            teleporterProjectile.GetComponent<ProjectileSteerTowardTarget>().enabled = false;
            teleporterProjectile.GetComponent<ProjectileDirectionalTargetFinder>().enabled = false;
            teleporterProjectile.GetComponent<ProjectileTargetComponent>().enabled = false;

            var projectileSimple = teleporterProjectile.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = 7f;
            projectileSimple.enableVelocityOverLifetime = true;
            projectileSimple.desiredForwardSpeed = 180f;

            ContentAddition.AddProjectile(teleporterProjectile);
            PrefabAPI.RegisterNetworkPrefab(teleporterProjectile);

            RoR2.TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction teleporterInteraction)
        {
            if (teleporterInteraction.GetComponent<TeleporterPassiveProjectilesController>() == null)
            {
                teleporterInteraction.gameObject.AddComponent<TeleporterPassiveProjectilesController>();
            }
        }
    }

    public class TeleporterPassiveProjectilesController : MonoBehaviour
    {
        public float timer = 0f;
        public float interval = PassiveProjectiles.projectileInterval;
        public int projectileFireCounter = 0;

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= interval)
            {
                FireProjectiles();
                timer = 0f;
            }
        }

        public void FireProjectiles()
        {
            var damage = 20f * Mathf.Sqrt(Run.instance.difficultyCoefficient);
            var scalar = projectileFireCounter / 8f;
            var fpi = new FireProjectileInfo()
            {
                crit = false,
                damage = damage,
                damageColorIndex = DamageColorIndex.Nearby,
                force = 0,
                owner = gameObject,
                rotation = Util.QuaternionSafeLookRotation(Vector3.forward + (Vector3.right * scalar)),
                projectilePrefab = PassiveProjectiles.teleporterProjectile,
                position = gameObject.transform.position + new Vector3(0f, 5f, 0f),
                damageTypeOverride = DamageType.BypassArmor | DamageType.BypassBlock
            };

            if (Util.HasEffectiveAuthority(gameObject))
            {
                ProjectileManager.instance.FireProjectile(fpi);
            }

            projectileFireCounter++;

            if (projectileFireCounter >= 8)
            {
                projectileFireCounter = 0;
            }
        }
    }
}