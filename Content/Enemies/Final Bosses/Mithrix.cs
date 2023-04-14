using EntityStates.BrotherMonster;
using Inferno.Stat_AI;
using Rewired.ComponentControls.Effects;
using System;
using UnityEngine.SceneManagement;

namespace WellRoundedBalance.Enemies.FinalBosses
{
    public class Mithrix : EnemyBase<Mithrix>
    {
        [ConfigField("Phase 1 & 3 Base Move Speed", "Disabled if playing Inferno.", 17f)]
        public static float phase13BaseMoveSpeed;

        [ConfigField("Phase 4 Base Move Speed", "Disabled if playing Inferno.", 10f)]
        public static float phase4BaseMoveSpeed;

        [ConfigField("Phase 4 Base Max Health", "Disabled if playing Inferno.", 700f)]
        public static float phase4BaseMaxHealth;

        [ConfigField("Phase 4 Sprinting Speed Multiplier", "Disabled if playing Inferno.", 1.45f)]
        public static float phase4SprintingSpeedMultiplier;

        [ConfigField("Remove Fall Damage?", "Disabled if playing Inferno.", true)]
        public static bool removeFallDamage;

        [ConfigField("Should Sprint in Any Direction?", "Disabled if playing Inferno.", true)]
        public static bool shouldSprintInAnyDirection;

        [ConfigField("Enable Skill and AI Changes?", "Disabled if playing Inferno.", true)]
        public static bool enableSkillAndAIChanges;

        [ConfigField("Disable Ramps?", "Disabled if playing Inferno.", true)]
        public static bool disableRamps;

        [ConfigField("Disable Phase 2?", "Disabled if playing Inferno.", true)]
        public static bool disablePhase2;

        [ConfigField("Disable Big Spinny Cripple?", "", true)]
        public static bool disableCripple;

        [ConfigField("Enable Moving Escape Lines?", "", true)]
        public static bool enableMovingEscapeLines;

        public static GameObject ramp1;
        public static GameObject ramp2;
        public static GameObject ramp3;
        public static GameObject rocks;

        public override string Name => ":::: Final Bosses : Mithrix";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            CharacterBody.onBodyAwakeGlobal += CharacterBody_onBodyAwakeGlobal;
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
            On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += Phase1_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += Phase2_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += Phase3_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += Phase4_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.OnExit += BrotherEncounterPhaseBaseState_OnExit;
            On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.FixedUpdate += BrotherEncounterPhaseBaseState_FixedUpdate;
            On.EntityStates.BrotherMonster.HoldSkyLeap.OnEnter += HoldSkyLeap_OnEnter;
            On.EntityStates.BrotherMonster.ExitSkyLeap.OnEnter += ExitSkyLeap_OnEnter;
            On.EntityStates.BrotherMonster.WeaponSlam.OnEnter += WeaponSlam_OnEnter;
            On.EntityStates.BrotherMonster.BaseSlideState.OnEnter += BaseSlideState_OnEnter;
            On.EntityStates.BrotherMonster.SprintBash.OnEnter += SprintBash_OnEnter;
            On.EntityStates.BrotherMonster.Weapon.FireLunarShards.OnEnter += FireLunarShards_OnEnter;
            On.EntityStates.BrotherMonster.FistSlam.OnEnter += FistSlam_OnEnter;
            On.EntityStates.BrotherMonster.FistSlam.OnExit += FistSlam_OnExit;
            On.EntityStates.BrotherMonster.SpellChannelEnterState.OnEnter += SpellChannelEnterState_OnEnter;
            On.EntityStates.BrotherMonster.SpellChannelState.OnEnter += SpellChannelState_OnEnter;
            On.EntityStates.BrotherMonster.SpellChannelExitState.OnEnter += SpellChannelExitState_OnEnter;
            On.EntityStates.BrotherMonster.StaggerEnter.OnEnter += StaggerEnter_OnEnter;
            On.EntityStates.BrotherMonster.StaggerExit.OnEnter += StaggerExit_OnEnter;
            On.EntityStates.BrotherMonster.StaggerLoop.OnEnter += StaggerLoop_OnEnter;
            On.EntityStates.BrotherMonster.TrueDeathState.OnEnter += TrueDeathState_OnEnter;
            On.EntityStates.BrotherMonster.UltChannelState.OnEnter += UltChannelState_OnEnter;
            On.EntityStates.BrotherHaunt.FireRandomProjectiles.OnEnter += FireRandomProjectiles_OnEnter;
            On.RoR2.MasterSummon.Perform += MasterSummon_Perform;
            Changes();
        }

        private void FistSlam_OnExit(On.EntityStates.BrotherMonster.FistSlam.orig_OnExit orig, FistSlam self)
        {
            orig(self);
            if (enableSkillAndAIChanges)
            {
                var body = self.characterBody;
                if (self.isAuthority)
                {
                    var projectileCount = 10;
                    var slices = 360f / projectileCount;
                    var upVector = Vector3.ProjectOnPlane(body.inputBank.aimDirection, Vector3.up);
                    var footPosition = body.footPosition;
                    for (int i = 0; i < projectileCount; i++)
                    {
                        var vector2 = Quaternion.AngleAxis(slices * i, Vector3.up) * upVector;
                        if (self.isAuthority)
                        {
                            ProjectileManager.instance.FireProjectile(ExitSkyLeap.waveProjectilePrefab, footPosition, Util.QuaternionSafeLookRotation(vector2), body.gameObject, body.damage * 0.66f, ExitSkyLeap.waveProjectileForce, Util.CheckRoll(body.crit, body.master), DamageColorIndex.Default, null, -1f);
                        }
                    }
                }
            }
        }

        private void BrotherEncounterPhaseBaseState_OnExit(On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.orig_OnExit orig, EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState self)
        {
            if (self is EntityStates.Missions.BrotherEncounter.Phase4)
            {
                disableAllyNPC = false;
            }
            orig(self);
        }

        public static bool disableAllyNPC = false;

        private CharacterMaster MasterSummon_Perform(On.RoR2.MasterSummon.orig_Perform orig, MasterSummon self)
        {
            var master = self.masterPrefab.GetComponent<CharacterMaster>();
            if (disableAllyNPC && NetworkServer.active)
            {
                if (master && (self.teamIndexOverride == TeamIndex.Player || master.teamIndex == TeamIndex.Player))
                    return null;
            }
            return orig(self);
        }

        public static float timer;
        public static float interval = 0.1f;

        private void BrotherEncounterPhaseBaseState_FixedUpdate(On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.orig_FixedUpdate orig, EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState self)
        {
            if (self is EntityStates.Missions.BrotherEncounter.Phase4 && NetworkServer.active)
            {
                disableAllyNPC = true;

                timer += Time.fixedDeltaTime;
                if (timer >= interval)
                {
                    for (int i = 0; i < CharacterBody.readOnlyInstancesList.Count; i++)
                    {
                        var body = CharacterBody.readOnlyInstancesList[i];
                        if (body.teamComponent.teamIndex == TeamIndex.Player && !body.isPlayerControlled)
                        {
                            body.healthComponent.Suicide();
                        }
                    }
                }
            }
            orig(self);
        }

        private void UltChannelState_OnEnter(On.EntityStates.BrotherMonster.UltChannelState.orig_OnEnter orig, UltChannelState self)
        {
            if (enableSkillAndAIChanges)
            {
                UltChannelState.totalWaves = 8;
                UltChannelState.maxDuration = 8f;
            }
            orig(self);
        }

        private void FireRandomProjectiles_OnEnter(On.EntityStates.BrotherHaunt.FireRandomProjectiles.orig_OnEnter orig, EntityStates.BrotherHaunt.FireRandomProjectiles self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherHaunt.FireRandomProjectiles.maximumCharges = 150;
                EntityStates.BrotherHaunt.FireRandomProjectiles.chargeRechargeDuration = 0.06f;
                EntityStates.BrotherHaunt.FireRandomProjectiles.chanceToFirePerSecond = 0.33f;
                EntityStates.BrotherHaunt.FireRandomProjectiles.damageCoefficient = 7.5f;
            }
            orig(self);
        }

        private void TrueDeathState_OnEnter(On.EntityStates.BrotherMonster.TrueDeathState.orig_OnEnter orig, TrueDeathState self)
        {
            if (enableSkillAndAIChanges)
            {
                TrueDeathState.dissolveDuration = 5f;
            }
            orig(self);
        }

        private void StaggerLoop_OnEnter(On.EntityStates.BrotherMonster.StaggerLoop.orig_OnEnter orig, StaggerLoop self)
        {
            if (enableSkillAndAIChanges)
            {
                self.duration = 0f;
            }
            orig(self);
        }

        private void StaggerExit_OnEnter(On.EntityStates.BrotherMonster.StaggerExit.orig_OnEnter orig, StaggerExit self)
        {
            if (enableSkillAndAIChanges)
            {
                self.duration = 0f;
            }
            orig(self);
        }

        private void StaggerEnter_OnEnter(On.EntityStates.BrotherMonster.StaggerEnter.orig_OnEnter orig, StaggerEnter self)
        {
            if (enableSkillAndAIChanges)
            {
                self.duration = 0f;
            }
            orig(self);
        }

        private void SpellChannelExitState_OnEnter(On.EntityStates.BrotherMonster.SpellChannelExitState.orig_OnEnter orig, SpellChannelExitState self)
        {
            if (enableSkillAndAIChanges)
            {
                SpellChannelExitState.lendInterval = 0f;
                SpellChannelExitState.duration = 2.5f;
            }
            orig(self);
        }

        private void SpellChannelState_OnEnter(On.EntityStates.BrotherMonster.SpellChannelState.orig_OnEnter orig, SpellChannelState self)
        {
            if (enableSkillAndAIChanges)
            {
                SpellChannelState.stealInterval = 0f;
                SpellChannelState.delayBeforeBeginningSteal = 0f;
                SpellChannelState.maxDuration = 1f;
            }
            orig(self);
        }

        private void SpellChannelEnterState_OnEnter(On.EntityStates.BrotherMonster.SpellChannelEnterState.orig_OnEnter orig, SpellChannelEnterState self)
        {
            if (enableSkillAndAIChanges)
            {
                SpellChannelEnterState.duration = 3f;
            }
            orig(self);
        }

        private void FistSlam_OnEnter(On.EntityStates.BrotherMonster.FistSlam.orig_OnEnter orig, FistSlam self)
        {
            if (enableSkillAndAIChanges)
            {
                FistSlam.waveProjectileDamageCoefficient = 1.2f;
                FistSlam.healthCostFraction = 0f;
                FistSlam.waveProjectileCount = 20;
                FistSlam.baseDuration = 3.5f;
            }

            orig(self);
        }

        private static void FireLunarShards_OnEnter(On.EntityStates.BrotherMonster.Weapon.FireLunarShards.orig_OnEnter orig, EntityStates.BrotherMonster.Weapon.FireLunarShards self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.Weapon.FireLunarShards.spreadBloomValue = 20f;
                EntityStates.BrotherMonster.Weapon.FireLunarShards.recoilAmplitude = 2f;
            }
            orig(self);
        }

        private void SprintBash_OnEnter(On.EntityStates.BrotherMonster.SprintBash.orig_OnEnter orig, SprintBash self)
        {
            if (enableSkillAndAIChanges)
            {
                self.baseDuration = 1.4f;
                self.damageCoefficient = 1.5f;
                self.pushAwayForce = 1500f;
                self.forceVector = new Vector3(0f, 750f, 0f);
            }
            orig(self);
        }

        private static void BaseSlideState_OnEnter(On.EntityStates.BrotherMonster.BaseSlideState.orig_OnEnter orig, BaseSlideState self)
        {
            if (enableSkillAndAIChanges)
            {
                switch (self)
                {
                    case SlideBackwardState:
                        self.slideRotation = Quaternion.identity;
                        break;

                    case SlideLeftState:
                        self.slideRotation = Quaternion.AngleAxis(-40f, Vector3.up);
                        break;

                    case SlideRightState:
                        self.slideRotation = Quaternion.AngleAxis(40f, Vector3.up);
                        break;
                }
            }
            orig(self);
        }

        private void WeaponSlam_OnEnter(On.EntityStates.BrotherMonster.WeaponSlam.orig_OnEnter orig, WeaponSlam self)
        {
            if (enableSkillAndAIChanges)
            {
                WeaponSlam.waveProjectileArc = 360f;
                WeaponSlam.waveProjectileCount = 8;
                WeaponSlam.waveProjectileForce = -2000f;
                WeaponSlam.weaponForce = -3000f;
                WeaponSlam.duration = 3f;
            }
            orig(self);
        }

        private void ExitSkyLeap_OnEnter(On.EntityStates.BrotherMonster.ExitSkyLeap.orig_OnEnter orig, ExitSkyLeap self)
        {
            if (enableSkillAndAIChanges)
            {
                ExitSkyLeap.waveProjectileCount = 20;
                ExitSkyLeap.waveProjectileDamageCoefficient = 2.5f;
            }
            orig(self);
        }

        private void HoldSkyLeap_OnEnter(On.EntityStates.BrotherMonster.HoldSkyLeap.orig_OnEnter orig, HoldSkyLeap self)
        {
            if (enableSkillAndAIChanges)
            {
                HoldSkyLeap.duration = 2f;
                if (NetworkServer.active)
                {
                    Util.CleanseBody(self.characterBody, true, false, false, true, true, false);
                }
            }
            orig(self);
        }

        private void Phase4_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase4.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase4 self)
        {
            if (disableRamps)
            {
                ramp1.SetActive(false);
                ramp2.SetActive(false);
                ramp3.SetActive(false);
                rocks.SetActive(false);
            }
            orig(self);
            if (enableSkillAndAIChanges)
            {
                var players = CharacterBody.readOnlyInstancesList.Where(x => x.isPlayerControlled);
                foreach (CharacterBody body in players)
                {
                    var directorSpawnRequest = new DirectorSpawnRequest(LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscBrotherGlass"), new DirectorPlacementRule
                    {
                        placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                        minDistance = 25f,
                        maxDistance = 45f,
                        spawnOnTarget = body.transform,
                    }, RoR2Application.rng);
                    directorSpawnRequest.summonerBodyObject = self.gameObject;
                    directorSpawnRequest.teamIndexOverride = TeamIndex.Monster;
                    directorSpawnRequest.ignoreTeamMemberLimit = true;

                    DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                }
            }
        }

        private void Phase3_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
        {
            if (disableRamps)
            {
                ramp1.SetActive(false);
                ramp2.SetActive(false);
                ramp3.SetActive(false);
                rocks.SetActive(false);
            }
            orig(self);
        }

        private void Phase2_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
        {
            if (disableRamps)
            {
                ramp1.SetActive(false);
                ramp2.SetActive(false);
                ramp3.SetActive(false);
                rocks.SetActive(false);
            }
            orig(self);
            if (disablePhase2)
            {
                self.PreEncounterBegin();
                self.outer.SetNextState(new EntityStates.Missions.BrotherEncounter.Phase3());
            }
        }

        private void Phase1_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
        {
            if (disableRamps)
            {
                ramp1.SetActive(false);
                ramp2.SetActive(false);
                ramp3.SetActive(false);
                rocks.SetActive(false);
            }
            orig(self);
        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            if (SceneManager.GetActiveScene().name == "moon2")
            {
                var Arena = GameObject.Find("HOLDER: Final Arena").transform;
                ramp1 = Arena.GetChild(0).gameObject;
                ramp2 = Arena.GetChild(1).gameObject;
                ramp3 = Arena.GetChild(2).gameObject;
                rocks = Arena.GetChild(6).gameObject;
            }
            orig(self);
        }

        private void CharacterMaster_onStartGlobal(CharacterMaster cm)
        {
            if (Main.IsInfernoDef())
            {
                // pass
            }
            else
            {
                switch (cm.name)
                {
                    case "BrotherMaster(Clone)":
                        {
                            if (enableSkillAndAIChanges)
                            {
                                AISkillDriver MithrixFireShards = (from x in cm.GetComponents<AISkillDriver>()
                                                                   where x.customName == "Sprint and FireLunarShards"
                                                                   select x).First();
                                MithrixFireShards.minDistance = 0f;
                                MithrixFireShards.maxUserHealthFraction = Mathf.Infinity;

                                AISkillDriver MithrixSprint = (from x in cm.GetComponents<AISkillDriver>()
                                                               where x.customName == "Sprint After Target"
                                                               select x).First();
                                MithrixSprint.minDistance = 40f;

                                AISkillDriver DashStrafe = (from x in cm.GetComponents<AISkillDriver>()
                                                            where x.customName == "DashStrafe"
                                                            select x).First();
                                DashStrafe.nextHighPriorityOverride = MithrixFireShards;
                            }
                        }

                        break;

                    case "BrotherHurtMaster(Clone)":
                        if (enableSkillAndAIChanges)
                        {
                            AISkillDriver MithrixWeakSlam = (from x in cm.GetComponents<AISkillDriver>()
                                                             where x.customName == "SlamGround"
                                                             select x).First();
                            MithrixWeakSlam.maxUserHealthFraction = Mathf.Infinity;
                            MithrixWeakSlam.movementType = AISkillDriver.MovementType.StrafeMovetarget;

                            AISkillDriver MithrixWeakShards = (from x in cm.GetComponents<AISkillDriver>()
                                                               where x.customName == "Shoot"
                                                               select x).First();
                            MithrixWeakShards.movementType = AISkillDriver.MovementType.StrafeMovetarget;
                        }

                        break;
                }
            }
        }

        private void CharacterBody_onBodyAwakeGlobal(CharacterBody cb)
        {
            if (Main.IsInfernoDef())
            {
                // pass
            }
            else
            {
                switch (cb.name)
                {
                    case "BrotherBody(Clone)":
                        cb.baseAcceleration = 200f;
                        cb.baseMoveSpeed = phase13BaseMoveSpeed;
                        if (removeFallDamage)
                        {
                            cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                        }

                        if (shouldSprintInAnyDirection)
                        {
                            cb.bodyFlags |= CharacterBody.BodyFlags.SprintAnyDirection;
                        }

                        break;

                    case "BrotherHurtBody(Clone)":
                        cb.baseMoveSpeed = phase4BaseMoveSpeed;
                        cb.sprintingSpeedMultiplier = phase4SprintingSpeedMultiplier;
                        cb.baseMaxHealth = phase4BaseMaxHealth;
                        cb.levelMaxHealth = phase4BaseMaxHealth * 0.3f;
                        cb.baseDamage = 13f;
                        cb.levelDamage = 2.6f;
                        if (removeFallDamage)
                        {
                            cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                        }
                        if (shouldSprintInAnyDirection)
                        {
                            cb.bodyFlags |= CharacterBody.BodyFlags.SprintAnyDirection;
                        }
                        break;

                    case "BrotherGlassBody(Clone)":
                        cb.baseMaxHealth = phase4BaseMaxHealth;
                        cb.levelMaxHealth = phase4BaseMaxHealth * 0.3f;
                        break;
                }
            }
        }

        private void Changes()
        {
            if (disableCripple)
            {
                var leftWave = Utils.Paths.GameObject.BrotherUltLineProjectileRotateLeft.Load<GameObject>();
                var projectileDamage1 = leftWave.GetComponent<ProjectileDamage>();
                projectileDamage1.damageType = DamageType.Generic;

                var rightWave = Utils.Paths.GameObject.BrotherUltLineProjectileRotateRight.Load<GameObject>();
                var projectileDamage2 = rightWave.GetComponent<ProjectileDamage>();
                projectileDamage2.damageType = DamageType.Generic;
            }

            if (enableMovingEscapeLines)
            {
                var escapeLine = Utils.Paths.GameObject.BrotherUltLineProjectileStatic.Load<GameObject>();
                var rotateAroundAxis = escapeLine.GetComponent<RotateAroundAxis>();
                rotateAroundAxis.enabled = true;
                rotateAroundAxis.slowRotationSpeed = 25f;
                rotateAroundAxis.fastRotationSpeed = 25f;

                var projectileSimple = escapeLine.GetComponent<ProjectileSimple>();
                projectileSimple.desiredForwardSpeed = 50f;
            }
        }
    }
}