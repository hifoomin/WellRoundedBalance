using EntityStates;
using RoR2.Skills;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class StoneTitan : EnemyBase<StoneTitan>
    {
        public static Material overlayMat;
        public override string Name => "::: Bosses :: Stone Titan";

        [ConfigField("Apply to Stone Titan", true)]
        public static bool stoneTitan;

        [ConfigField("Apply to Aurellionite", true)]
        public static bool aurellionite;

        [ConfigField("Laser Damage", "1 = 100%, vanilla: 1", 1.6f)]
        public static float NEW_FireMegaLaser_damageCoefficient;

        private static float FireMegaLaser_damageCoefficient;

        [ConfigField("Fist Entry Duration", "in seconds, vanilla: 2", 1.8f)]
        public static float NEW_FireFist_entryDuration;

        private static float FireFist_entryDuration;

        [ConfigField("Fist Fire Duration", "in seconds, vanilla: 1.2", 1f)]
        public static float NEW_FireFist_fireDuration;

        private static float FireFist_fireDuration;

        [ConfigField("Laser Damage", "1 = 100%, vanilla: 1", 1.6f)]
        public static float NEW_FireFist_fistDamageCoefficient;

        private static float FireFist_fistDamageCoefficient;

        [ConfigField("Rock Recharge Duration", "in seconds, vanilla: 9", 3f)]
        public static float NEW_RechargeRocks_baseDuration;

        private static float RechargeRocks_baseDuration;

        [ConfigField("Rock Duration", "in seconds, vanilla: 25", 20f)]
        public static float NEW_destroyOnTimer_duration;

        private static float destroyOnTimer_duration;

        [ConfigField("Rock Spinup Time", "in seconds, vanilla: 6", 2f)]
        public static float NEW_tit_startDelay;

        private static float tit_startDelay;

        [ConfigField("Rock Fire Interval", "in seconds, vanilla: 1.5", 0.33f)]
        public static float NEW_tit_fireInterval;

        private static float tit_fireInterval;

        [ConfigField("Rock Damage", "1 = 100%, vanilla: 0.3", 0.48f)]
        public static float NEW_tit_damageCoefficient;

        private static float tit_damageCoefficient;

        [ConfigField("Rock Damage Force", "unity mass, vanilla: 1600", 2000f)]
        public static float NEW_tit_damageForce;

        private static float tit_damageForce;

        [ConfigField("Base Damage", "vanilla: 40", 25f)]
        public static float NEW_titanBody_baseDamage;

        private static float titanBody_baseDamage;

        [ConfigField("Damage per Level", "vanilla: 8", 5f)]
        public static float NEW_titanBody_levelDamage;

        private static float titanBody_levelDamage;

        [ConfigField("Enable new Laser Attack", true)]
        public static bool enableLaserSD;

        public static SerializableEntityStateType NEW_laserSD_activationState;
        private static SerializableEntityStateType laserSD_activationState;

        [ConfigField("Aim Speed", "vanilla: 180", 720f)]
        public static float NEW_ai_aimVectorMaxSpeed;

        private static float ai_aimVectorMaxSpeed;

        [ConfigField("Aim Damp Time", "in seconds, vanilla: 0.1", 0.03f)]
        public static float NEW_ai_aimVectorDampTime;

        private static float ai_aimVectorDampTime;

        [ConfigField("Fist Ring Amount", "ring of fists, set to 0 to disable", 2)]
        public static int fistring_v;

        [ConfigField("Fist Ring Fists per Ring", "horizontally", 4)]
        public static int fistring_h;

        [ConfigField("Fist Ring Range", "total range for fist ring in radius (16 root 2 by default)", 22.6274f)]
        public static float fistring_range;

        [ConfigField("Fist Ring Delay", "in seconds, extra delay per ring", 0.256f)]
        public static float fistring_vs;

        [ConfigField("Fist Ring Delay per Lane", "in seconds, extra delay within a ring", 0.025f)]
        public static float fistring_hs;

        [ConfigField("New Laser Attack Amount", 12)]
        public static int SD_amount;

        [ConfigField("New Laser Attack Interval", "in seconds", 0.4f)]
        public static float SD_interval;

        [ConfigField("New Laser Attack Force", 400f)]
        public static float SD_force;

        [ConfigField("New Laser Attack Range", 2000f)]
        public static float SD_range;

        [ConfigField("New Laser Attack Proc Coefficient", 0.6f)]
        public static float SD_proc;

        private static BodyIndex titanIndex;
        private static BodyIndex aurellioniteIndex;
        private static MasterCatalog.MasterIndex titanMasterIndex;
        private static MasterCatalog.MasterIndex aurellioniteMasterIndex;
        private static int laserIndex;

        public override void Init()
        {
            base.Init();
            overlayMat = Object.Instantiate(Utils.Paths.Material.matHuntressFlashBright.Load<Material>());
            overlayMat.SetColor("_TintColor", new Color32(191, 4, 3, 42));
        }

        public static void PostInit()
        {
            // get every "base" values, getting it instead of hardcoding for interop -p

            titanIndex = BodyCatalog.FindBodyIndex("TitanBody");
            aurellioniteIndex = BodyCatalog.FindBodyIndex("TitanGoldBody");
            titanMasterIndex = MasterCatalog.FindMasterIndex("TitanMaster");
            aurellioniteMasterIndex = MasterCatalog.FindMasterIndex("TitanGoldMaster");
            laserIndex = Utils.Paths.SkillDef.TitanBodyLaser.Load<SkillDef>().skillIndex;

            FireMegaLaser_damageCoefficient = EntityStates.TitanMonster.FireMegaLaser.damageCoefficient;
            FireFist_entryDuration = EntityStates.TitanMonster.FireFist.entryDuration;
            FireFist_fireDuration = EntityStates.TitanMonster.FireFist.fireDuration;
            FireFist_fistDamageCoefficient = EntityStates.TitanMonster.FireFist.fistDamageCoefficient;
            RechargeRocks_baseDuration = EntityStates.TitanMonster.RechargeRocks.baseDuration;

            var rockController = Utils.Paths.GameObject.TitanRockController.Load<GameObject>();
            var destroyOnTimer = rockController.GetComponent<DestroyOnTimer>();
            destroyOnTimer_duration = destroyOnTimer.duration;

            var titanRockProjectile = Utils.Paths.GameObject.TitanRockProjectile.Load<GameObject>();
            var projectileSimple = titanRockProjectile.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = 7f;
            projectileSimple.desiredForwardSpeed = 30f;
            projectileSimple.enableVelocityOverLifetime = true;
            projectileSimple.velocityOverLifetime = new AnimationCurve(new Keyframe(0f, 30f), new Keyframe(1f, 50f));

            var tit = rockController.GetComponent<TitanRockController>();
            tit_startDelay = tit.startDelay;
            tit_fireInterval = tit.fireInterval;
            tit_damageCoefficient = tit.damageCoefficient;
            tit_damageForce = tit.damageForce;

            var titanBody = Utils.Paths.GameObject.TitanBody13.Load<GameObject>().GetComponent<CharacterBody>();
            titanBody_baseDamage = titanBody.baseDamage;
            titanBody_levelDamage = titanBody.levelDamage;

            GameObject master = Utils.Paths.GameObject.TitanMaster.Load<GameObject>();
            BaseAI ai = master.GetComponent<BaseAI>();
            ai_aimVectorMaxSpeed = ai.aimVectorMaxSpeed;
            ai_aimVectorDampTime = ai.aimVectorDampTime;
        }

        public override void Hooks()
        {
            RoR2Application.onLoad += PostInit;
            On.EntityStates.TitanMonster.RechargeRocks.OnEnter += RechargeRocks_OnEnter;
            On.EntityStates.TitanMonster.FireFist.OnEnter += FireFist_OnEnter;
            On.EntityStates.TitanMonster.FireMegaLaser.OnEnter += FireMegaLaser_OnEnter;
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.TitanRockController.Start += TitanRockController_Start;
            Changes();
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster master)
        {
            MasterCatalog.MasterIndex idx = master.masterIndex;
            if (idx == titanMasterIndex || idx == aurellioniteMasterIndex)
            {
                var ai = master.GetComponent<BaseAI>();
                ai.aimVectorMaxSpeed = Check(idx, NEW_ai_aimVectorMaxSpeed, ai_aimVectorMaxSpeed);
                ai.aimVectorDampTime = Check(idx, NEW_ai_aimVectorDampTime, ai_aimVectorDampTime);
            }
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            BodyIndex idx = body.bodyIndex;
            if (idx == titanIndex || idx == aurellioniteIndex)
            {
                body.baseDamage = Check(idx, NEW_titanBody_baseDamage, titanBody_baseDamage);
                body.levelDamage = Check(idx, NEW_titanBody_levelDamage, titanBody_levelDamage);
                var laser = body.skillLocator.special.skillDef;
                if (laser.skillIndex == laserIndex) laser.activationState = Check(idx, NEW_laserSD_activationState, laserSD_activationState);
            }
        }

        private void TitanRockController_Start(On.RoR2.TitanRockController.orig_Start orig, TitanRockController self)
        {
            BodyIndex idx = self.Networkowner.GetComponent<CharacterBody>().bodyIndex;
            if (idx == titanIndex || idx == aurellioniteIndex)
            {
                self.GetComponent<DestroyOnTimer>().duration = Check(idx, NEW_destroyOnTimer_duration, destroyOnTimer_duration);
                self.startDelay = Check(idx, NEW_tit_startDelay, tit_startDelay);
                self.fireInterval = Check(idx, NEW_tit_fireInterval, tit_fireInterval);
                self.damageCoefficient = Check(idx, NEW_tit_damageCoefficient, tit_damageCoefficient);
                self.damageForce = Check(idx, NEW_tit_damageForce, tit_damageForce);
            }
            orig(self);
        }

        private static T Check<T>(EntityState self, T NEW, T OLD) => Check(self.outer.commonComponents.characterBody.bodyIndex, NEW, OLD);

        private static T Check<T>(BodyIndex i, T NEW, T OLD)
        {
            if (i == titanIndex) return stoneTitan ? NEW : OLD;
            if (i == aurellioniteIndex) return aurellionite ? NEW : OLD;
            return NEW;
        }

        private static T Check<T>(MasterCatalog.MasterIndex i, T NEW, T OLD)
        {
            if (i == titanMasterIndex) return stoneTitan ? NEW : OLD;
            if (i == aurellioniteMasterIndex) return aurellionite ? NEW : OLD;
            return NEW;
        }

        private void FireMegaLaser_OnEnter(On.EntityStates.TitanMonster.FireMegaLaser.orig_OnEnter orig, EntityStates.TitanMonster.FireMegaLaser self)
        {
            EntityStates.TitanMonster.FireMegaLaser.damageCoefficient = Check(self, NEW_FireMegaLaser_damageCoefficient, FireMegaLaser_damageCoefficient);
            orig(self);
        }

        private void FireFist_OnEnter(On.EntityStates.TitanMonster.FireFist.orig_OnEnter orig, EntityStates.TitanMonster.FireFist self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.TitanMonster.FireFist.entryDuration = Check(self, NEW_FireFist_entryDuration, FireFist_entryDuration);
                EntityStates.TitanMonster.FireFist.fireDuration = Check(self, NEW_FireFist_fireDuration, FireFist_fireDuration);
                EntityStates.TitanMonster.FireFist.fistDamageCoefficient = Check(self, NEW_FireFist_fistDamageCoefficient, FireFist_fistDamageCoefficient);
                if (self.isAuthority && Check(self, true, false))
                {
                    for (int i = 1; i <= fistring_v; i++)
                    {
                        for (int j = 0; j < fistring_h; j++)
                        {
                            var fpi = new FireProjectileInfo
                            {
                                projectilePrefab = self.fistProjectilePrefab,
                                position = self.characterBody.footPosition + (Quaternion.AngleAxis(360f * (j + 0.5f) / fistring_h + Vector3.Angle(Vector3.forward, Vector3.ProjectOnPlane(self.GetAimRay().direction, Vector3.up)), Vector3.up) * new Vector3(i * fistring_range, 0, 0)),
                                rotation = Quaternion.identity,
                                owner = self.gameObject,
                                damage = self.damageStat,
                                force = EntityStates.TitanMonster.FireFist.fistForce,
                                crit = self.RollCrit(),
                                fuseOverride = EntityStates.TitanMonster.FireFist.entryDuration - EntityStates.TitanMonster.FireFist.trackingDuration + (i * fistring_vs) + (j * fistring_hs)
                            };
                            ProjectileManager.instance.FireProjectile(fpi);
                        }
                    }
                }
            }
            orig(self);
        }

        private void RechargeRocks_OnEnter(On.EntityStates.TitanMonster.RechargeRocks.orig_OnEnter orig, EntityStates.TitanMonster.RechargeRocks self)
        {
            if (!Main.IsInfernoDef())
                EntityStates.TitanMonster.RechargeRocks.baseDuration = Check(self, NEW_RechargeRocks_baseDuration, RechargeRocks_baseDuration);

            orig(self);
        }

        private void Changes()
        {
            ContentAddition.AddEntityState(typeof(LaserAttack), out _);
            NEW_laserSD_activationState = new SerializableEntityStateType(typeof(LaserAttack));
        }
    }

    public class LaserAttack : BaseState
    {
        public float durationPerLaser => StoneTitan.SD_interval;
        public int baseLasers => StoneTitan.SD_amount;
        public static GameObject effect = Utils.Paths.GameObject.LaserGolem.Load<GameObject>();
        public static GameObject tracer = Utils.Paths.GameObject.TracerGolem.Load<GameObject>();
        private int count = 0;
        private float delay => durationPerLaser / attackSpeedStat;
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

            laserInstance = Object.Instantiate(effect, FindModelChild("MuzzleLaser"));
            lr = laserInstance.GetComponent<LineRenderer>();

            chargeID = AkSoundEngine.PostEvent(Events.Play_titanboss_R_laser_preshoot, gameObject);

            var modelTransform = GetModelTransform();
            if (modelTransform)
            {
                var temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = durationPerLaser * baseLasers * 1.25f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, durationPerLaser * baseLasers, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = StoneTitan.overlayMat;
                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
            }
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

            Ray aimRay = GetAimRay();
            float width = durationPerLaser - stopwatch < 0.1f ? 0.1f : durationPerLaser - stopwatch;

            lr.startWidth = width;
            lr.endWidth = width;
            lr.SetPosition(0, laserInstance.transform.position);
            lr.SetPosition(1, aimRay.GetPoint(StoneTitan.SD_range / 2));

            laserInstance.transform.forward = targetDir;

            if (durationPerLaser - stopwatch > durationPerLaser / 2)
            {
                targetDir = aimRay.direction; // grace period before firing so it's dodgeable
            }

            if (stopwatch >= delay)
            {
                count++;
                stopwatch = 0f;

                BulletAttack attack = new()
                {
                    origin = aimRay.origin,
                    damage = damageStat,
                    procCoefficient = StoneTitan.SD_proc,
                    falloffModel = BulletAttack.FalloffModel.None,
                    aimVector = targetDir,
                    muzzleName = "MuzzleLaser",
                    tracerEffectPrefab = tracer,
                    minSpread = 0,
                    maxSpread = 0,
                    maxDistance = StoneTitan.SD_range,
                    owner = gameObject,
                    smartCollision = true,
                    radius = 1f,
                    force = StoneTitan.SD_force,
                    queryTriggerInteraction = QueryTriggerInteraction.Ignore
                };

                if (isAuthority) attack.Fire();

                AkSoundEngine.PostEvent(Events.Play_golem_laser_fire, gameObject);

                if (count > baseLasers)
                {
                    outer.SetNextStateToMain();
                }
            }
        }
    }
}