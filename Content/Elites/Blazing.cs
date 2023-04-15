using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Collections;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Gamemodes.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Blazing : EliteBase<Blazing>
    {
        public override string Name => ":: Elites :: Blazing";

        [ConfigField("Fire Projectile Interval", "", 5f)]
        public static float fireProjectileInterval;

        [ConfigField("Fire Projectile Interval Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 4f)]
        public static float fireProjectileIntervalE3;

        [ConfigField("Fire Trail Length", "", 10f)]
        public static float fireTrailLength;

        [ConfigField("Fire Trail Radius", "", 5f)]
        public static float fireTrailRadius;

        [ConfigField("Fire Trail Damage Per Second", "Decimal.", 1.25f)]
        public static float fireTrailDamagePerSecond;

        [ConfigField("Fire Pool Damage Per Second", "Decimal.", 2f)]
        public static float firePoolDamagePerSecond;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.UpdateFireTrail += CharacterBody_UpdateFireTrail;
            IL.RoR2.CharacterBody.UpdateFireTrail += CharacterBody_UpdateFireTrail1;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            Changes();
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody characterBody)
        {
            var sfp = characterBody.GetComponent<BlazingController>();
            if (characterBody.HasBuff(RoR2Content.Buffs.AffixRed))
            {
                if (sfp == null)
                {
                    characterBody.gameObject.AddComponent<BlazingController>();
                }
            }
            else if (sfp != null)
            {
                characterBody.gameObject.RemoveComponent<BlazingController>();
            }
        }

        private void CharacterBody_UpdateFireTrail(On.RoR2.CharacterBody.orig_UpdateFireTrail orig, CharacterBody self)
        {
            if (self && self.fireTrail)
            {
                self.fireTrail.radius = fireTrailRadius * self.radius;
            }
            orig(self);
        }

        private void CharacterBody_UpdateFireTrail1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f)))
            {
                c.Next.Operand = fireTrailDamagePerSecond;
            }
            else
            {
                Logger.LogError("Failed to apply Blazing Elite Firetrail Damage hook");
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixRed")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Blazing Elite On Hit hook");
            }
        }

        private void Changes()
        {
            var trail = Utils.Paths.GameObject.FireTrail.Load<GameObject>().GetComponent<DamageTrail>();
            var trailVFX = Utils.Paths.GameObject.FireTrailSegment.Load<GameObject>();
            var trailPS = trailVFX.GetComponent<ParticleSystem>();
            var trailDoT = trailVFX.GetComponent<DestroyOnTimer>();

            trail.pointLifetime = fireTrailLength;
            // trail.radius = 5f;

            trailDoT.duration = fireTrailLength + 0.6f;

            var main = trailPS.main;
            main.duration = fireTrailLength + 0.5f;
            var startSize = main.startSize;
            startSize.mode = ParticleSystemCurveMode.Constant;
            startSize.constant = fireTrailRadius;

            var shape = trailPS.shape;
            shape.scale = new Vector3(2f, 0f, 2f);
        }
    }

    public class BlazingController : MonoBehaviour
    {
        public CharacterBody body;
        public GameObject projectile = Projectiles.Molotov.singlePrefab;
        private float timer;
        public float interval;
        public int projectileCount;
        public float angle;
        public float delayBetweenProjectiles = 0.1f;

        public void Start()
        {
            body = GetComponent<CharacterBody>();
            float maxInterval = Eclipse3.CheckEclipse() ? Blazing.fireProjectileIntervalE3 : Blazing.fireProjectileIntervalE3;
            float minInterval = maxInterval / 2f;
            projectileCount = Mathf.RoundToInt(Util.Remap(body.baseMaxHealth, 20, 2500, 2, 6));
            interval = Mathf.RoundToInt(Util.Remap(body.baseMaxHealth, 20, 2500, minInterval, maxInterval));
            angle = 360f / projectileCount;
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                StartCoroutine(FireProjectiles());
            }
        }

        /*
        public IEnumerator FireProjectiles()
        {
            Vector3 normalized = Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up);
            Vector3 position = body.corePosition + new Vector3(0, 3, 0);
            for (int i = 0; i < projectileCount; i++)
            {
                Vector3 forward = Quaternion.AngleAxis(angle * i, Vector3.up) * normalized;
                if (Util.HasEffectiveAuthority(gameObject))
                {
                    FireProjectileInfo info = new()
                    {
                        owner = body.gameObject,
                        damage = body.damage * Blazing.firePoolDamagePerSecond * 0.2f,
                        crit = false,
                        position = position,
                        rotation = Quaternion.LookRotation(forward),
                        projectilePrefab = projectile
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }
                yield return new WaitForSeconds(delayBetweenProjectiles);
            }
        }
        */

        public IEnumerator FireProjectiles()
        {
            var normalized = Vector3.ProjectOnPlane(Random.onUnitSphere, Vector3.up);
            var position = body.corePosition + new Vector3(0, 3, 0);
            var rotationIncrement = Quaternion.AngleAxis(angle / (float)projectileCount, Vector3.up);
            var rotation = Quaternion.LookRotation(normalized);
            for (int i = 0; i < projectileCount; i++)
            {
                if (Util.HasEffectiveAuthority(gameObject))
                {
                    FireProjectileInfo info = new()
                    {
                        owner = body.gameObject,
                        damage = body.damage * Blazing.firePoolDamagePerSecond * 0.2f,
                        crit = false,
                        position = position,
                        rotation = rotation,
                        projectilePrefab = projectile
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }
                rotation *= rotationIncrement;
                yield return new WaitForSeconds(delayBetweenProjectiles);
            }
        }
    }
}