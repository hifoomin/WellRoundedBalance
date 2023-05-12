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
            ContentAddition.AddEntityState(typeof(LaserAttack), out _);

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

    public class LaserAttack : BaseState
    {
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

            laserInstance = Object.Instantiate(effect, FindModelChild("MuzzleLaser"));
            lr = laserInstance.GetComponent<LineRenderer>();

            chargeID = AkSoundEngine.PostEvent(Events.Play_titanboss_R_laser_preshoot, gameObject);
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
            lr.SetPosition(1, aimRay.GetPoint(1000));

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
                    procCoefficient = 0.6f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    aimVector = targetDir,
                    muzzleName = "MuzzleLaser",
                    tracerEffectPrefab = tracer,
                    minSpread = 0,
                    maxSpread = 0,
                    maxDistance = 2000,
                    owner = gameObject,
                    smartCollision = true,
                    radius = 1f,
                    force = 400f,
                    queryTriggerInteraction = QueryTriggerInteraction.Ignore
                };

                if (base.isAuthority)
                {
                    attack.Fire();
                }

                AkSoundEngine.PostEvent(Events.Play_golem_laser_fire, base.gameObject);

                if (count > baseLasers)
                {
                    outer.SetNextStateToMain();
                }
            }
        }
    }
}