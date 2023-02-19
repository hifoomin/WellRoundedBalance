using Inferno.Stat_AI;
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
                    return Run.instance ? 2f + Mathf.Sqrt(Run.instance.ambientLevel * 0.22f) : 0f;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Blazing Elite Firetrail Damage hook");
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
                Main.WRBLogger.LogError("Failed to apply Blazing Elite On Hit hook");
            }
        }

        private void Changes()
        {
            var trail = Utils.Paths.GameObject.FireTrail.Load<GameObject>().GetComponent<DamageTrail>();
            var trailVFX = Utils.Paths.GameObject.FireTrailSegment.Load<GameObject>();
            var trailPS = trailVFX.GetComponent<ParticleSystem>();
            var trailDoT = trailVFX.GetComponent<DestroyOnTimer>();

            trail.pointLifetime = 10f;
            // trail.radius = 5f;

            trailDoT.duration = 10.6f;

            var main = trailPS.main;
            main.duration = 10.5f;
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
        public GameObject projectile = Projectiles.Molotov.prefab;
        private float timer;
        public float interval;

        public void Start()
        {
            body = GetComponent<CharacterBody>();
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3 && Eclipse3.instance.isEnabled)
            {
                interval = 4f;
                timer = 2f;
            }
            else
            {
                interval = 5f;
                timer = 3f;
            }
        }

        public void FixedUpdate()
        {
            timer += Time.fixedDeltaTime;
            var angle = body.inputBank.GetAimRay().direction;
            if (timer >= interval)
            {
                Vector3 randomVector = new(Run.instance.runRNG.RangeInt(0, 360), 0f, Run.instance.runRNG.RangeInt(0, 360));
                var fpi = new FireProjectileInfo
                {
                    owner = gameObject,
                    rotation = Util.QuaternionSafeLookRotation(randomVector),
                    projectilePrefab = projectile,
                    crit = Util.CheckRoll(body.crit, body.master),
                    position = body.corePosition,
                    damage = Run.instance ? Mathf.Sqrt(Run.instance.ambientLevel * 9.05f) : 0f
                };
                if (Util.HasEffectiveAuthority(gameObject)) ProjectileManager.instance.FireProjectile(fpi);
                timer = 0f;
            }
        }
    }
}