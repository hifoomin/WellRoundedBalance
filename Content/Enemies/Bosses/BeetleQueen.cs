using EntityStates;
using RoR2.Skills;
using System;

namespace WellRoundedBalance.Enemies.Bosses
{
    internal class BeetleQueen : EnemyBase<BeetleQueen>
    {
        public static Material overlayMat;
        public override string Name => "::: Bosses :: Beetle Queen";

        public override void Init()
        {
            base.Init();
            overlayMat = GameObject.Instantiate(Utils.Paths.Material.matHuntressFlashBright.Load<Material>());
            overlayMat.SetColor("_TintColor", new Color32(191, 30, 3, 35));
        }

        public override void Hooks()
        {
            On.EntityStates.BeetleQueenMonster.SummonEggs.OnEnter += SummonEggs_OnEnter;
            On.EntityStates.BeetleQueenMonster.FireSpit.OnEnter += FireSpit_OnEnter;
            On.EntityStates.BeetleQueenMonster.SpawnWards.OnEnter += SpawnWards_OnEnter;
            On.RoR2.CharacterAI.BaseAI.Awake += BaseAI_Awake;
            Changes();
        }

        private void BaseAI_Awake(On.RoR2.CharacterAI.BaseAI.orig_Awake orig, BaseAI self)
        {
            orig(self);
            if (self.master && self.master.masterIndex == MasterCatalog.FindMasterIndex("BeetleQueenMaster"))
            {
                if (self.skillDrivers[0].customName != "SummonEarthquake")
                {
                    var driversList = self.skillDrivers.ToList();
                    var lastNumber = driversList[driversList.Count - 1];
                    driversList.RemoveAt(driversList.Count - 1);
                    driversList.Insert(0, lastNumber);
                    self.skillDrivers = driversList.ToArray();
                }
            }
        }

        public static CharacterMaster queen = Utils.Paths.GameObject.BeetleQueenMaster.Load<GameObject>().GetComponent<CharacterMaster>();

        private void SpawnWards_OnEnter(On.EntityStates.BeetleQueenMonster.SpawnWards.orig_OnEnter orig, EntityStates.BeetleQueenMonster.SpawnWards self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.SpawnWards.baseDuration = 3f;
                EntityStates.BeetleQueenMonster.SpawnWards.orbTravelSpeed = 20f;
            }

            orig(self);
        }

        private void FireSpit_OnEnter(On.EntityStates.BeetleQueenMonster.FireSpit.orig_OnEnter orig, EntityStates.BeetleQueenMonster.FireSpit self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.FireSpit.damageCoefficient = 0.4f;
                EntityStates.BeetleQueenMonster.FireSpit.force = 1200f;
                EntityStates.BeetleQueenMonster.FireSpit.yawSpread = 20f;
                EntityStates.BeetleQueenMonster.FireSpit.minSpread = 15f;
                EntityStates.BeetleQueenMonster.FireSpit.maxSpread = 30f;
                EntityStates.BeetleQueenMonster.FireSpit.projectileHSpeed = 40f;
                EntityStates.BeetleQueenMonster.FireSpit.projectileCount = 10;
            }
            orig(self);
        }

        private void SummonEggs_OnEnter(On.EntityStates.BeetleQueenMonster.SummonEggs.orig_OnEnter orig, EntityStates.BeetleQueenMonster.SummonEggs self)
        {
            if (!Main.IsInfernoDef())
            {
                EntityStates.BeetleQueenMonster.SummonEggs.summonInterval = 2f;
                EntityStates.BeetleQueenMonster.SummonEggs.randomRadius = 13f;
                EntityStates.BeetleQueenMonster.SummonEggs.baseDuration = 3f;
            }

            orig(self);
        }

        private void Changes()
        {
            ContentAddition.AddEntityState(typeof(Earthquake), out _);

            var beetleQueen = Utils.Paths.GameObject.BeetleQueen2Body9.Load<GameObject>();

            var esm = beetleQueen.AddComponent<EntityStateMachine>();
            esm.customName = "Earthquake";
            esm.initialStateType = new(typeof(EntityStates.BeetleQueenMonster.SpawnState));
            esm.mainStateType = new(typeof(GenericCharacterMain));

            var nsm = beetleQueen.GetComponent<NetworkStateMachine>();
            Array.Resize(ref nsm.stateMachines, nsm.stateMachines.Length + 1);
            nsm.stateMachines[nsm.stateMachines.Length - 1] = esm;

            var utilitySD = ScriptableObject.CreateInstance<SkillDef>();
            utilitySD.activationState = new SerializableEntityStateType(typeof(Earthquake));
            utilitySD.activationStateMachineName = "Earthquake";
            utilitySD.interruptPriority = InterruptPriority.Skill;
            utilitySD.baseRechargeInterval = 10f;
            utilitySD.baseMaxStock = 1;
            utilitySD.rechargeStock = 1;
            utilitySD.requiredStock = 1;
            utilitySD.stockToConsume = 1;
            utilitySD.resetCooldownTimerOnUse = false;
            utilitySD.fullRestockOnAssign = true;
            utilitySD.dontAllowPastMaxStocks = false;
            utilitySD.beginSkillCooldownOnSkillEnd = false;
            utilitySD.cancelSprintingOnActivation = true;
            utilitySD.forceSprintDuringState = false;
            utilitySD.beginSkillCooldownOnSkillEnd = true;
            utilitySD.isCombatSkill = true;
            utilitySD.mustKeyPress = false;
            (utilitySD as ScriptableObject).name = "EarthquakeSkill";

            ContentAddition.AddSkillDef(utilitySD);

            var utilityFamily = ScriptableObject.CreateInstance<SkillFamily>();
            Array.Resize(ref utilityFamily.variants, 1);
            utilityFamily.variants[0].skillDef = utilitySD;
            (utilityFamily as ScriptableObject).name = "UtilityFamily";

            ContentAddition.AddSkillFamily(utilityFamily);

            var utility = beetleQueen.AddComponent<GenericSkill>();
            utility._skillFamily = utilityFamily;

            var master = Utils.Paths.GameObject.BeetleQueenMaster.Load<GameObject>();
            var ed = master.AddComponent<AISkillDriver>();
            ed.customName = "SummonEarthquake";
            ed.skillSlot = SkillSlot.Utility;
            ed.requireSkillReady = true;
            ed.minUserHealthFraction = Mathf.NegativeInfinity;
            ed.maxUserHealthFraction = Mathf.Infinity;
            ed.minTargetHealthFraction = Mathf.NegativeInfinity;
            ed.maxTargetHealthFraction = Mathf.Infinity;
            ed.minDistance = 17f;
            ed.maxDistance = 80f;
            ed.selectionRequiresTargetLoS = false;
            ed.selectionRequiresOnGround = false;
            ed.selectionRequiresAimTarget = false;
            ed.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            ed.activationRequiresAimTargetLoS = false;
            ed.activationRequiresAimConfirmation = false;
            ed.activationRequiresTargetLoS = false;
            ed.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            ed.moveInputScale = 1;
            ed.aimType = AISkillDriver.AimType.AtMoveTarget;
            ed.ignoreNodeGraph = false;
            ed.shouldSprint = false;
            ed.shouldFireEquipment = false;
            ed.shouldTapButton = false;
            ed.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            ed.driverUpdateTimerOverride = -1;
            ed.resetCurrentEnemyOnNextDriverSelection = false;
            ed.noRepeat = false;

            var locator = beetleQueen.GetComponent<SkillLocator>();
            locator.utility = utility;

            var summonBeetleGuards = Utils.Paths.SkillDef.BeetleQueen2BodySummonEggs.Load<SkillDef>();
            summonBeetleGuards.baseRechargeInterval = 60f;

            var spitProjectile = Utils.Paths.GameObject.BeetleQueenSpit.Load<GameObject>();
            var projectileImpactExplosion = spitProjectile.GetComponent<ProjectileImpactExplosion>();
            projectileImpactExplosion.falloffModel = BlastAttack.FalloffModel.None;

            var spitDoT = Utils.Paths.GameObject.BeetleQueenAcid.Load<GameObject>();
            var projectileDotZone = spitDoT.GetComponent<ProjectileDotZone>();
            projectileDotZone.lifetime = 9f;
            projectileDotZone.damageCoefficient = 3f;
            spitDoT.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);

            var hitBox = spitDoT.transform.GetChild(0).GetChild(2);
            hitBox.localPosition = new Vector3(0f, 0f, -0.5f);
            hitBox.localScale = new Vector3(4f, 1.5f, 4f);

            var beetleWard = Utils.Paths.GameObject.BeetleWard.Load<GameObject>();
            var buffWard = beetleWard.GetComponent<BuffWard>();
            buffWard.radius = 7f;
            buffWard.interval = 0.5f;
            buffWard.buffDuration = 3f;
            buffWard.expireDuration = 10f;

            var egg = Utils.Paths.SkillDef.BeetleQueen2BodySpawnWards.Load<SkillDef>();
            egg.baseRechargeInterval = 12f;
        }
    }

    // replace projectile if ai reoreder infally works ioajdocjnduie54ryguifdvbhncuijuero89isdfjvcu9ix

    public class Earthquake : BaseState
    {
        public static float baseDuration = 6f;
        public float durationBetweenWaves = 1.25f;

        public static string tellString = "Play_beetle_guard_attack2_initial";
        public static string impactString = "Play_Player_footstep";

        public static GameObject waveProjectilePrefab = Projectiles.EarthQuakeWave.prefab;

        public static int waveProjectileCount = 12;

        public static float waveProjectileDamageCoefficient = 0.4f;

        public static float waveProjectileForce = 600f;

        public float timer;
        public float tellTimer;

        public override void OnEnter()
        {
            base.OnEnter();
            var modelTransform = GetModelTransform();
            if (modelTransform)
            {
                var temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 6f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 6f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = BeetleQueen.overlayMat;
                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
            }
            var aimAnimator = GetAimAnimator();
            if (aimAnimator)
            {
                aimAnimator.enabled = true;
            }
        }

        private void FireWave()
        {
            Util.PlaySound(impactString, gameObject);
            var slices = 360f / waveProjectileCount;
            var upVector = Vector3.ProjectOnPlane(inputBank.aimDirection, Vector3.up);
            var footPosition = characterBody.footPosition;
            for (int i = 0; i < waveProjectileCount; i++)
            {
                var quat = Quaternion.AngleAxis(slices * i, Vector3.up) * upVector;
                if (isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(waveProjectilePrefab, footPosition, Util.QuaternionSafeLookRotation(quat), gameObject, characterBody.damage * waveProjectileDamageCoefficient, waveProjectileForce, Util.CheckRoll(characterBody.crit, characterBody.master), DamageColorIndex.Default, null, -1f);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            timer += Time.fixedDeltaTime;
            tellTimer += Time.fixedDeltaTime;
            if (isAuthority)
            {
                if (tellTimer >= durationBetweenWaves - 0.25f)
                {
                    Util.PlaySound(tellString, gameObject);
                    tellTimer -= durationBetweenWaves - 0.25f;
                }
                if (timer >= durationBetweenWaves)
                {
                    FireWave();
                    timer -= durationBetweenWaves;
                }
                if (fixedAge > baseDuration)
                {
                    outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}