using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using WellRoundedBalance.Buffs;

namespace WellRoundedBalance.Elites
{
    internal class Glacial : EliteBase
    {
        public static BuffDef slow;
        public static GameObject iceExplosionPrefab;
        public override string Name => ":: Elites :::: Glacial";
        public static GameObject ShieldPrefab;
        public static GameObject ShieldGhostPrefab;
        public static GameObject mdlGlacialShield;

        public override void Init()
        {
            iceExplosionPrefab = Utils.Paths.GameObject.AffixWhiteExplosion.Load<GameObject>();

            var slow80 = Utils.Paths.Texture2D.texBuffSlow50Icon.Load<Texture2D>();

            slow = ScriptableObject.CreateInstance<BuffDef>();
            slow.name = "Glacial Elite Slow";
            slow.buffColor = new Color32(165, 222, 237, 255);
            slow.iconSprite = Sprite.Create(slow80, new Rect(0f, 0f, (float)slow80.width, (float)slow80.height), new Vector2(0f, 0f));
            slow.isDebuff = true;
            slow.canStack = false;
            slow.isHidden = false;

            ContentAddition.AddBuffDef(slow);

            ShieldPrefab = PrefabAPI.InstantiateClone(new(""), "GlacialShield");
            ShieldGhostPrefab = PrefabAPI.InstantiateClone(new(""), "GlacialShieldGhost", false);
            mdlGlacialShield = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.mdlAegis.Load<GameObject>(), "mdlGlacialShield", false);
            ShieldPrefab.AddComponent<TeamFilter>();
            ShieldPrefab.AddComponent<NetworkIdentity>();
            ProjectileController controller = ShieldPrefab.AddComponent<ProjectileController>();
            controller.ghostPrefab = ShieldGhostPrefab;
            controller.procCoefficient = 0;
            controller.allowPrediction = false;

            Rigidbody rigidbody = ShieldPrefab.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = false;
            rigidbody.drag = 0;
            rigidbody.mass = 1000;

            SphereCollider collider = ShieldPrefab.AddComponent<SphereCollider>();
            collider.radius = 1f;
            /*
            ProjectileImpactExplosion impact = ShieldPrefab.AddComponent<ProjectileImpactExplosion>();
            impact.blastRadius = 3;
            impact.lifetime = 5;
            impact.destroyOnEnemy = false;
            impact.destroyOnWorld = false;
            impact.impactOnWorld = false;
            impact.impactEffect = Utils.Paths.GameObject.IceRingExplosion.Load<GameObject>();
            */
            mdlGlacialShield.transform.SetParent(ShieldPrefab.transform);
            mdlGlacialShield.GetComponentInChildren<MeshRenderer>().material = Utils.Paths.Material.matIceOrbCore.Load<Material>();
            mdlGlacialShield.transform.localScale *= 0.6f;

            ProjectileTargetComponent target = ShieldPrefab.AddComponent<ProjectileTargetComponent>();

            // GlacialRotationController glacialRotation = ShieldPrefab.AddComponent<GlacialRotationController>();

            var shieldCharacterBody = ShieldPrefab.AddComponent<CharacterBody>();
            shieldCharacterBody.baseMaxHealth = 60;
            shieldCharacterBody.levelMaxHealth = 18;
            var skillLocator = ShieldPrefab.AddComponent<SkillLocator>();
            var shieldHealthComponent = ShieldPrefab.AddComponent<HealthComponent>();

            ShieldPrefab.layer = LayerIndex.entityPrecise.intVal;

            PrefabAPI.RegisterNetworkPrefab(ShieldPrefab);
            ContentAddition.AddProjectile(ShieldPrefab);
            base.Init();
        }

        public override void Hooks()
        {
            IL.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            IL.RoR2.CharacterModel.UpdateOverlays += CharacterModel_UpdateOverlays;
        }

        private void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (attackerBody)
                {
                    if (attackerBody.HasBuff(RoR2Content.Buffs.AffixWhite))
                    {
                        var procType = (ProcType)1258907;
                        if (!damageInfo.procChainMask.HasProc(procType) && Util.CheckRoll(100f * damageInfo.procCoefficient))
                        {
                            ProcChainMask mask = new();
                            mask.AddProc(procType);
                            DebuffSphere(slow.buffIndex, attackerBody.teamComponent.teamIndex, damageInfo.position, 4f, 1.5f, iceExplosionPrefab, null, false, true, null);
                        }
                    }
                }
                orig(self, damageInfo, hitObject);
            }
        }

        private void CharacterModel_UpdateOverlays(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                 x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Slow80")))
            {
                c.Index += 2;
                c.Emit(OpCodes.Ldarg_0);
                c.EmitDelegate<Func<bool, CharacterModel, bool>>((hasBuff, self) =>
                {
                    return hasBuff || (self.body.HasBuff(slow));
                });
            }
            else
            {
                Logger.LogError("Failed to apply Glacial Elite Overlay hook");
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(slow))
            {
                args.moveSpeedReductionMultAdd += 0.8f;
            }

            bool flag = sender.HasBuff(RoR2Content.Buffs.AffixWhite);
            GlacialShieldsController controller = sender.GetComponent<GlacialShieldsController>();

            if (flag != controller)
            {
                if (flag)
                {
                    sender.gameObject.AddComponent<GlacialShieldsController>();
                }
                else
                {
                    sender.gameObject.RemoveComponent<GlacialShieldsController>();
                }
            }
        }

        private void GlobalEventManager_OnHitEnemy(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "AffixWhite")))
            {
                c.Remove();
                c.Emit<Useless>(OpCodes.Ldsfld, nameof(Useless.uselessBuff));
            }
            else
            {
                Logger.LogError("Failed to apply Glacial Elite On Hit hook");
            }
        }

        private void DebuffSphere(BuffIndex buff, TeamIndex team, Vector3 position, float radius, float debuffDuration, GameObject effect, GameObject hitEffect, bool ignoreImmunity, bool falloff, NetworkSoundEventDef buffSound)
        {
            if (!NetworkServer.active)
            {
                return;
            }

            if (effect != null)
            {
                EffectManager.SpawnEffect(effect, new EffectData
                {
                    origin = position,
                    scale = radius
                }, true);
            }
            float radiusHalfwaySqr = radius * radius * 0.25f;
            List<HealthComponent> healthComponentList = new();
            Collider[] colliders = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < colliders.Length; i++)
            {
                HurtBox hurtBox = colliders[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    var healthComponent = hurtBox.healthComponent;
                    var projectileController = colliders[i].GetComponentInParent<ProjectileController>();
                    if (healthComponent && !projectileController && !healthComponentList.Contains(healthComponent))
                    {
                        healthComponentList.Add(healthComponent);
                        if (healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            if (ignoreImmunity || (!healthComponent.body.HasBuff(RoR2Content.Buffs.Immune) && !healthComponent.body.HasBuff(RoR2Content.Buffs.HiddenInvincibility)))
                            {
                                float effectiveness = 1f;
                                if (falloff)
                                {
                                    float distSqr = (position - hurtBox.collider.ClosestPoint(position)).sqrMagnitude;
                                    if (distSqr > radiusHalfwaySqr)  //Reduce effectiveness when over half the radius away
                                    {
                                        effectiveness *= 0.5f;  //0.25 is vanilla sweetspot
                                    }
                                }
                                bool alreadyHasBuff = healthComponent.body.HasBuff(buff);
                                healthComponent.body.AddTimedBuff(buff, effectiveness * debuffDuration);
                                if (!alreadyHasBuff)
                                {
                                    if (hitEffect != null)
                                    {
                                        EffectManager.SpawnEffect(hitEffect, new EffectData
                                        {
                                            origin = healthComponent.body.corePosition
                                        }, true);
                                    }
                                    if (buffSound != null)
                                    {
                                        EffectManager.SimpleSoundEffect(buffSound.index, healthComponent.body.corePosition, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public class GlacialShieldsController : MonoBehaviour
    {
        /*
        private float stopwatch = 0f;
        private float delay = 3f;
        private float stopwatchClear = 0f;
        private float delayClear = 9f;
        private TeamIndex index;
        private List<CharacterBody> bodies = new();

        public void Start()
        {
            index = GetComponent<TeamComponent>().teamIndex;
        }

        public void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            stopwatchClear += Time.fixedDeltaTime;
            if (stopwatch >= delay)
            {
                Main.WRBLogger.LogFatal("stopwatch is past delay");
                stopwatch = 0f;
                Collider[] cols = Physics.OverlapSphere(transform.position, 25).Where(x => x.GetComponent<CharacterBody>()).ToArray();
                foreach (Collider col in cols)
                {
                    CharacterBody body = col.GetComponent<CharacterBody>();
                    if (body.teamComponent.teamIndex == index && body != GetComponent<CharacterBody>() && !bodies.Contains(body))
                    {
                        Main.WRBLogger.LogFatal("found body, initializing orbiter");
                        bodies.Add(body);
                        var projectileOwnerOrbiter = body.gameObject.AddComponent<ProjectileOwnerOrbiter>();
                        var lunarSunProjectileController = body.gameObject.AddComponent<LunarSunProjectileController>();
                        InitializeOrbiter(body, projectileOwnerOrbiter, lunarSunProjectileController);
                    }
                }
            }

            if (stopwatchClear >= delayClear)
            {
                Main.WRBLogger.LogFatal("deleting bodies");
                stopwatchClear = 0f;
                bodies.Clear();
            }
        }

        public void InitializeOrbiter(CharacterBody body, ProjectileOwnerOrbiter orbiter, LunarSunProjectileController controller)
        {
            float randomRadius = body.radius + 2f + UnityEngine.Random.Range(0.25f, 0.25f);
            float what = randomRadius / 2f;
            what *= what;
            float degreesPerSecond = 180f * Mathf.Pow(0.9f, what);
            Quaternion quaternion = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 360f), Vector3.up);
            Quaternion quaternion2 = Quaternion.AngleAxis(UnityEngine.Random.Range(0f, 0f), Vector3.forward);
            Vector3 planeNormal = quaternion * quaternion2 * Vector3.up;
            float initialDegreesFromOwnerForward = UnityEngine.Random.Range(0f, 360f);
            orbiter.Initialize(planeNormal, randomRadius, degreesPerSecond, initialDegreesFromOwnerForward);
            onDisabled += DestroyOrbiter;
            Main.WRBLogger.LogFatal("initialized orbiter");
            void DestroyOrbiter(GlacialShieldsController glacialShield)
            {
                if (controller)
                {
                    Main.WRBLogger.LogFatal("trying to detonate lunarSunProjectileController");
                    controller.Detonate();
                }
            }
        }

        public event Action<GlacialShieldsController> onDisabled;

        public void OnDestroy()
        {
            Main.WRBLogger.LogFatal("ondisabled event called in ondestroy");
            onDisabled?.Invoke(this);
            onDisabled = null;
        }
        */
    }
}