using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using WellRoundedBalance.Buffs;
using WellRoundedBalance.Eclipse;

namespace WellRoundedBalance.Elites
{
    internal class Blazing : EliteBase
    {
        public override string Name => ":: Elites :: Blazing";

        [ConfigField("Fire Projectile Interval", "", 5f)]
        public static float fireProjectileInterval;

        [ConfigField("Fire Projectile Interval Eclipse 3+", "Only applies if you have Eclipse Changes enabled.", 4f)]
        public static float fireProjectileIntervalE3;

        [ConfigField("Fire Trail Length", "", 10f)]
        public static float fireTrailLength;

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
                self.fireTrail.radius = 5f * self.radius;
            }
            orig(self);
        }

        private void CharacterBody_UpdateFireTrail1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f)))
            {
                c.Index += 1;
                c.EmitDelegate<Func<float, float>>((useless) =>
                {
                    return Run.instance ? 2f + Mathf.Sqrt(Run.instance.ambientLevel * 0.22f) / Mathf.Sqrt(Run.instance.participatingPlayerCount) : 0f;
                });
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
            startSize.constant = 5f;

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
        public void Start()
        {
            body = GetComponent<CharacterBody>();
            float maxInterval = Eclipse3.CheckEclipse() ? Blazing.fireProjectileIntervalE3 : Blazing.fireProjectileIntervalE3;
            float minInterval = maxInterval / 2f;
            projectileCount = Mathf.RoundToInt(Util.Remap(body.baseMaxHealth, 20, 2500, 2, 6));
            interval = Mathf.RoundToInt(Util.Remap(body.baseMaxHealth, 20, 2500, minInterval, maxInterval));
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            if (timer >= interval)
            {
                timer = 0f;
                float num = 360f / projectileCount;
                Vector3 normalized = Vector3.ProjectOnPlane(UnityEngine.Random.onUnitSphere, Vector3.up);
                Vector3 position = body.corePosition + new Vector3(0, 3, 0);
                for (int i = 0; i < projectileCount; i++) {
                    Vector3 forward = Quaternion.AngleAxis(num * i, Vector3.up) * normalized;
                    FireProjectileInfo info = new();
                    info.owner = body.gameObject;
                    info.damage = body.damage * 0.4f;
                    info.crit = false;
                    info.position = position;
                    info.rotation = Quaternion.LookRotation(forward);
                    info.projectilePrefab = projectile;
                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }
    }
}