using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API.Utils;
using System;
using UnityEngine;
using WellRoundedBalance.Enemies;

namespace WellRoundedBalance.Elites
{
    internal class Blazing : EnemyBase
    {
        public static BuffDef useless;
        public override string Name => "::: Elites :: Blazing";

        public override void Init()
        {
            useless = ScriptableObject.CreateInstance<BuffDef>();
            useless.name = "Useless Buff";
            useless.isHidden = true;
            base.Init();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.UpdateFireTrail += CharacterBody_UpdateFireTrail;
            IL.RoR2.CharacterBody.UpdateFireTrail += CharacterBody_UpdateFireTrail1;
            Changes();
        }

        private void CharacterBody_UpdateFireTrail(On.RoR2.CharacterBody.orig_UpdateFireTrail orig, CharacterBody self)
        {
            if (self && self.fireTrail)
            {
                self.fireTrail.radius = 5f * self.radius;
            }
            orig(self);
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (body.HasBuff(RoR2Content.Buffs.AffixRed))
            {
                var sfp = body.GetComponent<BlazingController>();
                if (sfp == null)
                {
                    body.gameObject.AddComponent<BlazingController>();
                }
            }
        }

        private void CharacterBody_UpdateFireTrail1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(1.5f)))
            {
                c.Next.Operand = 1.75f;
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
                c.Emit<Blazing>(OpCodes.Ldsfld, nameof(useless));
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

            trail.pointLifetime = 6f;
            // trail.radius = 5f;

            trailDoT.duration = 6.6f;

            var main = trailPS.main;
            main.duration = 6.5f;
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
            if (Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse3)
            {
                interval = 3f;
                timer = 1f;
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
                var fpi = new FireProjectileInfo
                {
                    owner = gameObject,
                    rotation = Util.QuaternionSafeLookRotation(angle),
                    projectilePrefab = projectile,
                    crit = Util.CheckRoll(body.crit, body.master),
                    position = body.corePosition,
                };
                ProjectileManager.instance.FireProjectile(fpi);
                timer = 0f;
            }
        }
    }
}