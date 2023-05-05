using EntityStates;
using RoR2.Skills;
using RoR2.ConVar;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class StoneTitan : EnemyBase<StoneTitan>
    {
        public override string Name => "::: Bosses :: Stone Titan";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.TitanMonster.RechargeRocks.OnEnter += RechargeRocks_OnEnter;
            On.EntityStates.TitanMonster.FireFist.OnEnter += FireFist_OnEnter;
            On.EntityStates.TitanMonster.FireMegaLaser.OnEnter += FireMegaLaser_OnEnter;
            Changes();
        }

        private void FireMegaLaser_OnEnter(On.EntityStates.TitanMonster.FireMegaLaser.orig_OnEnter orig, EntityStates.TitanMonster.FireMegaLaser self)
        {
            EntityStates.TitanMonster.FireMegaLaser.damageCoefficient = 1.6f;
            orig(self);
        }

        private void FireFist_OnEnter(On.EntityStates.TitanMonster.FireFist.orig_OnEnter orig, EntityStates.TitanMonster.FireFist self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.TitanMonster.FireFist.entryDuration = 1.8f;
                EntityStates.TitanMonster.FireFist.fireDuration = 1f;
                EntityStates.TitanMonster.FireFist.fistDamageCoefficient = 1.6f;
                if (self.isAuthority)
                {
                    for (int i = 1; i < 3; i++)
                    {
                        var fpi = new FireProjectileInfo
                        {
                            projectilePrefab = self.fistProjectilePrefab,
                            position = self.characterBody.footPosition + new Vector3(i * 8, 0, i * 8),
                            rotation = Quaternion.identity,
                            owner = self.gameObject,
                            damage = self.damageStat,
                            force = EntityStates.TitanMonster.FireFist.fistForce,
                            crit = self.RollCrit(),
                            fuseOverride = EntityStates.TitanMonster.FireFist.entryDuration - EntityStates.TitanMonster.FireFist.trackingDuration + (i / 3f)
                        };
                        ProjectileManager.instance.FireProjectile(fpi);
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        var fpi = new FireProjectileInfo
                        {
                            projectilePrefab = self.fistProjectilePrefab,
                            position = self.characterBody.footPosition + new Vector3(-i * 8, 0, -i * 8),
                            rotation = Quaternion.identity,
                            owner = self.gameObject,
                            damage = self.damageStat,
                            force = EntityStates.TitanMonster.FireFist.fistForce,
                            crit = self.RollCrit(),
                            fuseOverride = EntityStates.TitanMonster.FireFist.entryDuration - EntityStates.TitanMonster.FireFist.trackingDuration + (i / 3.6f)
                        };
                        ProjectileManager.instance.FireProjectile(fpi);
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        var fpi = new FireProjectileInfo
                        {
                            projectilePrefab = self.fistProjectilePrefab,
                            position = self.characterBody.footPosition + new Vector3(-i * 8, 0, i * 8),
                            rotation = Quaternion.identity,
                            owner = self.gameObject,
                            damage = self.damageStat,
                            force = EntityStates.TitanMonster.FireFist.fistForce,
                            crit = self.RollCrit(),
                            fuseOverride = EntityStates.TitanMonster.FireFist.entryDuration - EntityStates.TitanMonster.FireFist.trackingDuration + (i / 3.3f)
                        };
                        ProjectileManager.instance.FireProjectile(fpi);
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        var fpi = new FireProjectileInfo
                        {
                            projectilePrefab = self.fistProjectilePrefab,
                            position = self.characterBody.footPosition + new Vector3(i * 8, 0, -i * 8),
                            rotation = Quaternion.identity,
                            owner = self.gameObject,
                            damage = self.damageStat,
                            force = EntityStates.TitanMonster.FireFist.fistForce,
                            crit = self.RollCrit(),
                            fuseOverride = EntityStates.TitanMonster.FireFist.entryDuration - EntityStates.TitanMonster.FireFist.trackingDuration + (i / 3.9f)
                        };
                        ProjectileManager.instance.FireProjectile(fpi);
                    }
                }
            }

            orig(self);
        }

        private void RechargeRocks_OnEnter(On.EntityStates.TitanMonster.RechargeRocks.orig_OnEnter orig, EntityStates.TitanMonster.RechargeRocks self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.TitanMonster.RechargeRocks.baseDuration = 3f;

            orig(self);
        }

        private void Changes()
        {
            ContentAddition.AddEntityState(typeof(ChargeLaserSpam), out _);
            ContentAddition.AddEntityState(typeof(FireLaserSpam), out _);

            var rockController = Utils.Paths.GameObject.TitanRockController.Load<GameObject>();
            var destroyOnTimer = rockController.GetComponent<DestroyOnTimer>();
            destroyOnTimer.duration = 20f;

            var tit = rockController.GetComponent<TitanRockController>();
            tit.startDelay = 2f;
            tit.fireInterval = 0.33f;
            tit.damageCoefficient = 0.48f;
            tit.damageForce = 2000f;

            var titanBody = Utils.Paths.GameObject.TitanBody13.Load<GameObject>().GetComponent<CharacterBody>();
            titanBody.baseDamage = 25f;
            titanBody.levelDamage = 5f;

            var laserSD = Utils.Paths.SkillDef.TitanBodyLaser.Load<SkillDef>();
            laserSD.activationState = new SerializableEntityStateType(typeof(LaserAttack));

            GameObject master = Utils.Paths.GameObject.TitanMaster.Load<GameObject>();
            BaseAI ai = master.GetComponent<BaseAI>();
            ai.aimVectorMaxSpeed = 720;
            ai.aimVectorDampTime = 0.03f;
        }
    }

    public class LaserAttack : BaseState {
        public float durationPerLaser => 0.4f;
        public int baseLasers => 12;
        public static GameObject effect = Utils.Paths.GameObject.LaserGolem.Load<GameObject>();
        public static GameObject tracer = Utils.Paths.GameObject.TracerGolem.Load<GameObject>();
        private int count = 0;
        private float delay => durationPerLaser / base.attackSpeedStat;
        private float stopwatch = 0f;
        private GameObject laserInstance;
        private LineRenderer lr;
        private Vector3 targetDir;
        private uint chargeID;

        // public static IntConVar lasers = new IntConVar("titan_laser_count", ConVarFlags.None, "12", "h");
        // public static FloatConVar durationPer = new FloatConVar("titan_laser_dur", ConVarFlags.None, "0.3", "h");
        // public static IntConVar trackPen = new IntConVar("titan_laser_track", ConVarFlags.None, "4", "h");

        public override void OnEnter()
        {
            base.OnEnter();

            laserInstance = GameObject.Instantiate(effect, base.FindModelChild("MuzzleLaser"));
            lr = laserInstance.GetComponent<LineRenderer>();

            chargeID = AkSoundEngine.PostEvent(Events.Play_titanboss_R_laser_preshoot, base.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
            Destroy(laserInstance);
            AkSoundEngine.StopPlayingID(chargeID);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            stopwatch += Time.fixedDeltaTime;

            Ray aimRay = base.GetAimRay();
            float width = durationPerLaser - stopwatch < 0.1f ? 0.1f : durationPerLaser - stopwatch;

            lr.startWidth = width;
            lr.endWidth = width;
            lr.SetPosition(0, laserInstance.transform.position);
            lr.SetPosition(1, aimRay.GetPoint(1000));

            laserInstance.transform.forward = targetDir;

            if (durationPerLaser - stopwatch > durationPerLaser / 2) {
                targetDir = aimRay.direction; // grace period before firing so it's dodgeable
            }

            if (stopwatch >= delay) {
                count++;
                stopwatch = 0f;

                BulletAttack attack = new();
                attack.origin = aimRay.origin;
                attack.damage = base.damageStat * 2f;
                attack.procCoefficient = 0.6f;
                attack.falloffModel = BulletAttack.FalloffModel.None;
                attack.aimVector = targetDir;
                attack.muzzleName = "MuzzleLaser";
                attack.tracerEffectPrefab = tracer;
                attack.minSpread = 0;
                attack.maxSpread = 0;
                attack.maxDistance = 2000;
                attack.owner = base.gameObject;
                attack.smartCollision = true;
                attack.radius = 1f;
                attack.force = 400f;
                attack.queryTriggerInteraction = QueryTriggerInteraction.Ignore;

                if (base.isAuthority) {
                    attack.Fire();
                }

                AkSoundEngine.PostEvent(Events.Play_golem_laser_fire, base.gameObject);

                if (count > baseLasers) {
                    outer.SetNextStateToMain();
                }
            }
        }
    }

    public class ChargeLaserSpam : BaseState
    {
        public static float baseDuration = 3f;

        public static float laserMaxWidth = 0.2f;

        public static GameObject effectPrefab = Utils.Paths.GameObject.ChargeGolem.Load<GameObject>();

        public static GameObject laserPrefab = Utils.Paths.GameObject.LaserGolem.Load<GameObject>();

        public static string attackSoundString = "Play_titanboss_R_laser_preshoot";

        public float duration;

        public uint chargePlayID;

        public GameObject chargeEffect;

        public GameObject laserEffect;

        public LineRenderer laserLineComponent;

        public Vector3 laserDirection;

        public float flashTimer = 0f;

        public bool laserOn = true;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            var modelTransform = GetModelTransform();
            chargePlayID = Util.PlayAttackSpeedSound(attackSoundString, gameObject, attackSpeedStat);
            if (modelTransform)
            {
                var childLocator = modelTransform.GetComponent<ChildLocator>();
                if (childLocator)
                {
                    var laserMuzzle = childLocator.FindChild("MuzzleLaser");
                    if (laserMuzzle)
                    {
                        if (effectPrefab)
                        {
                            chargeEffect = Object.Instantiate(effectPrefab, laserMuzzle.position, laserMuzzle.rotation);
                            chargeEffect.transform.parent = laserMuzzle;
                            var scaleParticleSystemDuration = chargeEffect.GetComponent<ScaleParticleSystemDuration>();
                            if (scaleParticleSystemDuration)
                            {
                                scaleParticleSystemDuration.newDuration = duration;
                            }
                        }
                        if (laserPrefab)
                        {
                            laserEffect = Object.Instantiate(laserPrefab, laserMuzzle.position, laserMuzzle.rotation);
                            laserEffect.transform.parent = laserMuzzle;
                            laserLineComponent = laserEffect.GetComponent<LineRenderer>();
                        }
                    }
                }
            }
            if (characterBody)
            {
                // characterBody.SetAimTimer(duration);
            }
            flashTimer = 0f;
            laserOn = true;
        }

        public override void OnExit()
        {
            AkSoundEngine.StopPlayingID(chargePlayID);
            base.OnExit();
            if (chargeEffect)
            {
                Destroy(chargeEffect);
            }
            if (laserEffect)
            {
                Destroy(laserEffect);
            }
        }

        public override void Update()
        {
            base.Update();
            if (laserEffect && laserLineComponent)
            {
                var aimRay = GetAimRay();
                var position = laserEffect.transform.parent.position;
                var endPoint = aimRay.GetPoint(1000f);
                // laserDirection = endPoint - position;
                laserDirection = aimRay.direction;
                if (Physics.Raycast(aimRay, out RaycastHit raycastHit, 1000f, LayerIndex.world.mask | LayerIndex.entityPrecise.mask))
                {
                    endPoint = raycastHit.point;
                }
                laserLineComponent.SetPosition(0, position);
                laserLineComponent.SetPosition(1, endPoint);
                float totalWidth;
                if (duration - age > 0.5f)
                {
                    totalWidth = age / duration;
                }
                else
                {
                    flashTimer -= Time.deltaTime;
                    if (flashTimer <= 0f)
                    {
                        laserOn = !laserOn;
                        flashTimer = 0.033333335f;
                    }
                    totalWidth = (laserOn ? 1f : 0f);
                }
                totalWidth *= laserMaxWidth;
                laserLineComponent.startWidth = totalWidth;
                laserLineComponent.endWidth = totalWidth;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (fixedAge >= duration && isAuthority)
            {
                FireLaserSpam fireLaserSpam = new()
                {
                    laserDirection = laserDirection
                };
                outer.SetNextState(fireLaserSpam);
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }

    public class FireLaserSpam : BaseState
    {
        public static GameObject effectPrefab = Utils.Paths.GameObject.MuzzleflashGolem.Load<GameObject>();

        public static GameObject hitEffectPrefab = Utils.Paths.GameObject.ExplosionGolem.Load<GameObject>();

        public static GameObject tracerEffectPrefab = Utils.Paths.GameObject.TracerGolem.Load<GameObject>();

        public static float damageCoefficient = 1f;

        public static float blastRadius = 3f;

        public static float force = 1500f;

        public static float minSpread = 0f;

        public static float maxSpread = 1f;

        public static float maxBaseDuration = 5f;
        public static float baseDuration = 0.5f;

        public static string attackSoundString = "Play_golem_laser_fire";

        public Vector3 laserDirection;

        public float duration;

        public Ray modifiedAimRay;
        public float timer;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = baseDuration / attackSpeedStat;
            PlayCrossfade("Gesture, Additive", "FireLaserLoop", 0.25f);
            if (characterBody)
            {
                characterBody.SetAimTimer(maxBaseDuration);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            PlayCrossfade("Gesture, Additive", "FireLaserEnd", 0.25f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer += Time.fixedDeltaTime;
            if (timer >= duration)
            {
                FireLaser();
                timer = 0f;
            }
            if (fixedAge >= maxBaseDuration && isAuthority)
            {
                outer.SetNextStateToMain();
                return;
            }
        }

        public void FireLaser()
        {
            modifiedAimRay = GetAimRay();
            modifiedAimRay.direction = laserDirection;
            GetModelAnimator();
            Util.PlaySound(attackSoundString, gameObject);
            var laserMuzzle = "MuzzleLaser";
            PlayAnimation("Gesture", "FireLaser", "FireLaser.playbackRate", duration);
            EffectManager.SimpleMuzzleFlash(effectPrefab, gameObject, laserMuzzle, false);
            var modelTransform = GetModelTransform();
            if (isAuthority)
            {
                float num = 1000f;
                Vector3 vector = modifiedAimRay.origin + modifiedAimRay.direction * num;
                if (Physics.Raycast(modifiedAimRay, out RaycastHit raycastHit, num, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
                {
                    vector = raycastHit.point;
                }
                new BlastAttack
                {
                    attacker = gameObject,
                    inflictor = gameObject,
                    teamIndex = TeamComponent.GetObjectTeam(gameObject),
                    baseDamage = damageStat * damageCoefficient,
                    baseForce = force * 0.2f,
                    position = vector,
                    radius = blastRadius,
                    falloffModel = BlastAttack.FalloffModel.SweetSpot,
                    bonusForce = force * modifiedAimRay.direction
                }.Fire();
                if (modelTransform)
                {
                    var childLocator = modelTransform.GetComponent<ChildLocator>();
                    if (childLocator)
                    {
                        var laserMuzzleIndex = childLocator.FindChildIndex(laserMuzzle);
                        EffectData effectData = new()
                        {
                            origin = vector,
                            start = modifiedAimRay.origin
                        };
                        effectData.SetChildLocatorTransformReference(gameObject, laserMuzzleIndex);
                        EffectManager.SpawnEffect(tracerEffectPrefab, effectData, true);
                        EffectManager.SpawnEffect(hitEffectPrefab, effectData, true);
                    }
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}