using EntityStates.BrotherMonster;
using Inferno.Stat_AI;
using Rewired.ComponentControls.Effects;
using UnityEngine.SceneManagement;

namespace WellRoundedBalance.Enemies.FinalBosses
{
    public class Mithrix : EnemyBase<Mithrix>
    {
        [ConfigField("Phase 1 & 3 Base Move Speed", "Disabled if playing Inferno.", 17f)]
        public static float phase13BaseMoveSpeed;

        [ConfigField("Phase 4 Base Move Speed", "Disabled if playing Inferno.", 10f)]
        public static float phase4BaseMoveSpeed;

        [ConfigField("Phase 4 Base Max Health", "Disabled if playing Inferno.", 450f)]
        public static float phase4BaseMaxHealth;

        [ConfigField("Phase 4 Sprinting Speed Multiplier", "Disabled if playing Inferno.", 1.45f)]
        public static float phase4SprintingSpeedMultiplier;

        [ConfigField("Remove Fall Damage?", "Disabled if playing Inferno.", true)]
        public static bool removeFallDamage;

        [ConfigField("Should Sprint in Any Direction?", "Disabled if playing Inferno.", true)]
        public static bool shouldSprintInAnyDirection;

        [ConfigField("Enable Orb Slam Aftershock?", "Disabled if playing Inferno.", true)]
        public static bool aftershock;

        [ConfigField("Enable Killing NPC on Phase 4?", "Disabled if playing Inferno.", true)]
        public static bool npc;

        [ConfigField("Enable Faster Big Spinny?", "Disabled if playing Inferno.", true)]
        public static bool bigSpinny;

        [ConfigField("Enable Escape Sequence Damage and Count changes?", "Disabled if playing Inferno.", true)]
        public static bool escapeSequence;

        [ConfigField("Enable Faster Mithrix Dissolve?", "Disabled if playing Inferno.", true)]
        public static bool dissolve;

        [ConfigField("Disable Mithrix Stagger?", "Disabled if playing Inferno.", true)]
        public static bool stagger;

        [ConfigField("Enable Item Steal Rework?", "Disabled if playing Inferno.", true)]
        public static bool stealRework;

        [ConfigField("Enable Orb Slam Changes?", "Disabled if playing Inferno.", true)]
        public static bool orbSlam;

        [ConfigField("Enable Lunar Shards Changes?", "Disabled if playing Inferno.", true)]
        public static bool shards;

        [ConfigField("Enable Hammer Bash Changes?", "Disabled if playing Inferno.", true)]
        public static bool bash;

        [ConfigField("Enable Slide Changes?", "Disabled if playing Inferno.", true)]
        public static bool slide;

        [ConfigField("Enable Hammer Slam Changes?", "Disabled if playing Inferno.", true)]
        public static bool slam;

        [ConfigField("Enable Sky Leap Changes?", "Disabled if playing Inferno.", true)]
        public static bool leap;

        [ConfigField("Enable Clones?", "Disabled if playing Inferno.", true)]
        public static bool clones;

        [ConfigField("Enable AI Changes?", "Disabled if playing Inferno.", true)]
        public static bool ai;

        [ConfigField("Enable Stat Changes?", "Disabled if playing Inferno.", true)]
        public static bool stat;

        [ConfigField("Disable Ramps?", "Disabled if playing Inferno.", true)]
        public static bool disableRamps;

        [ConfigField("Disable Phase 2?", "Disabled if playing Inferno.", true)]
        public static bool disablePhase2;

        [ConfigField("Disable Big Spinny Cripple?", "", true)]
        public static bool disableCripple;

        [ConfigField("Enable Moving Escape Lines?", "", true)]
        public static bool enableMovingEscapeLines;

        [ConfigField("Enable Phase 4 Player Speed Buff?", "", true)]
        public static bool phase4SpeedBuff;

        [ConfigField("Heal all players on Phase 4 Start?", "", true)]
        public static bool phase4Heal;

        public static GameObject ramp1;
        public static GameObject ramp2;
        public static GameObject ramp3;
        public static GameObject rocks;

        public static BuffDef speedBuff;

        public static SpawnCard mithrixGlass = LegacyResourcesAPI.Load<SpawnCard>("SpawnCards/CharacterSpawnCards/cscBrotherGlass");

        public static CharacterBody stationaryBody = Utils.Paths.GameObject.EngiTurretBody.Load<GameObject>().GetComponent<CharacterBody>();
        public static CharacterBody walkerBody = Utils.Paths.GameObject.EngiWalkerTurretBody.Load<GameObject>().GetComponent<CharacterBody>();
        public static CharacterMaster stationaryMaster = Utils.Paths.GameObject.EngiTurretMaster.Load<GameObject>().GetComponent<CharacterMaster>();
        public static CharacterMaster walkerMaster = Utils.Paths.GameObject.EngiWalkerTurretMaster.Load<GameObject>().GetComponent<CharacterMaster>();

        public override string Name => ":::: Final Bosses :: Mithrix";

        public override void Init()
        {
            base.Init();
            speedBuff = ScriptableObject.CreateInstance<BuffDef>();
            speedBuff.isHidden = true;
            speedBuff.isCooldown = false;
            speedBuff.isDebuff = false;
            speedBuff.canStack = false;

            ContentAddition.AddBuffDef(speedBuff);
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
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            Changes();
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(speedBuff))
            {
                args.moveSpeedMultAdd += 0.2f;
            }
        }

        private void FistSlam_OnExit(On.EntityStates.BrotherMonster.FistSlam.orig_OnExit orig, FistSlam self)
        {
            orig(self);
            if (aftershock)
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
                if (npc)
                    disableAllyNPC = false;
                if (phase4SpeedBuff)
                {
                    var players = CharacterBody.readOnlyInstancesList.Where(x => x.isPlayerControlled).ToList();
                    foreach (CharacterBody body in players)
                    {
                        body.RemoveBuff(speedBuff);
                    }
                }
            }
            orig(self);
        }

        public static bool disableAllyNPC = false;

        private CharacterMaster MasterSummon_Perform(On.RoR2.MasterSummon.orig_Perform orig, MasterSummon self)
        {
            if (npc)
            {
                var master = self.masterPrefab.GetComponent<CharacterMaster>();
                if (disableAllyNPC && NetworkServer.active)
                {
                    if (master && (self.teamIndexOverride == TeamIndex.Player || master.teamIndex == TeamIndex.Player) && master != stationaryMaster && master != walkerMaster)
                        return null;
                }
            }

            return orig(self);
        }

        public static float timer;
        public static float interval = 0.1f;

        private void BrotherEncounterPhaseBaseState_FixedUpdate(On.EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState.orig_FixedUpdate orig, EntityStates.Missions.BrotherEncounter.BrotherEncounterPhaseBaseState self)
        {
            if (self is EntityStates.Missions.BrotherEncounter.Phase4 && NetworkServer.active && npc)
            {
                disableAllyNPC = true;

                timer += Time.fixedDeltaTime;
                if (timer >= interval)
                {
                    for (int i = 0; i < CharacterBody.readOnlyInstancesList.ToList().Count; i++)
                    {
                        var body = CharacterBody.readOnlyInstancesList[i];
                        if (body.teamComponent.teamIndex == TeamIndex.Player && !body.isPlayerControlled && body != stationaryBody && body != walkerBody)
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
            if (bigSpinny)
            {
                UltChannelState.totalWaves = 8;
                UltChannelState.maxDuration = 8f;
            }
            orig(self);
        }

        private void FireRandomProjectiles_OnEnter(On.EntityStates.BrotherHaunt.FireRandomProjectiles.orig_OnEnter orig, EntityStates.BrotherHaunt.FireRandomProjectiles self)
        {
            if (escapeSequence)
            {
                EntityStates.BrotherHaunt.FireRandomProjectiles.maximumCharges = 150;
                EntityStates.BrotherHaunt.FireRandomProjectiles.chargeRechargeDuration = 0.06f;
                EntityStates.BrotherHaunt.FireRandomProjectiles.chanceToFirePerSecond = 0.33f;
                EntityStates.BrotherHaunt.FireRandomProjectiles.damageCoefficient = 6.5f;
            }
            orig(self);
        }

        private void TrueDeathState_OnEnter(On.EntityStates.BrotherMonster.TrueDeathState.orig_OnEnter orig, TrueDeathState self)
        {
            if (dissolve)
            {
                TrueDeathState.dissolveDuration = 5f;
            }
            orig(self);
        }

        private void StaggerLoop_OnEnter(On.EntityStates.BrotherMonster.StaggerLoop.orig_OnEnter orig, StaggerLoop self)
        {
            if (stagger)
            {
                self.duration = 0f;
            }
            orig(self);
        }

        private void StaggerExit_OnEnter(On.EntityStates.BrotherMonster.StaggerExit.orig_OnEnter orig, StaggerExit self)
        {
            if (stagger)
            {
                self.duration = 0f;
            }
            orig(self);
        }

        private void StaggerEnter_OnEnter(On.EntityStates.BrotherMonster.StaggerEnter.orig_OnEnter orig, StaggerEnter self)
        {
            if (stagger)
            {
                self.duration = 0f;
            }
            orig(self);
        }

        private void SpellChannelExitState_OnEnter(On.EntityStates.BrotherMonster.SpellChannelExitState.orig_OnEnter orig, SpellChannelExitState self)
        {
            if (stealRework)
            {
                SpellChannelExitState.lendInterval = -1;
                SpellChannelExitState.duration = 2.5f;
            }
            orig(self);
        }

        private void SpellChannelState_OnEnter(On.EntityStates.BrotherMonster.SpellChannelState.orig_OnEnter orig, SpellChannelState self)
        {
            if (stealRework)
            {
                SpellChannelState.stealInterval = 0;
                SpellChannelState.delayBeforeBeginningSteal = 0f;
                SpellChannelState.maxDuration = 1f;
            }
            orig(self);
        }

        private void SpellChannelEnterState_OnEnter(On.EntityStates.BrotherMonster.SpellChannelEnterState.orig_OnEnter orig, SpellChannelEnterState self)
        {
            if (stealRework)
            {
                SpellChannelEnterState.duration = 3f;
            }
            orig(self);
        }

        private void FistSlam_OnEnter(On.EntityStates.BrotherMonster.FistSlam.orig_OnEnter orig, FistSlam self)
        {
            if (orbSlam)
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
            if (shards)
            {
                EntityStates.BrotherMonster.Weapon.FireLunarShards.spreadBloomValue = 20f;
                EntityStates.BrotherMonster.Weapon.FireLunarShards.recoilAmplitude = 2f;
            }
            orig(self);
        }

        private void SprintBash_OnEnter(On.EntityStates.BrotherMonster.SprintBash.orig_OnEnter orig, SprintBash self)
        {
            if (bash)
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
            if (slide)
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
            if (slam)
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
            if (leap)
            {
                ExitSkyLeap.waveProjectileCount = 20;
                ExitSkyLeap.waveProjectileDamageCoefficient = 2.5f;
            }
            orig(self);
        }

        private void HoldSkyLeap_OnEnter(On.EntityStates.BrotherMonster.HoldSkyLeap.orig_OnEnter orig, HoldSkyLeap self)
        {
            if (leap)
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
            for (int i = 0; i < CharacterBody.readOnlyInstancesList.Count; i++)
            {
                var body = CharacterBody.readOnlyInstancesList[i];
                if (body.isPlayerControlled)
                {
                    if (phase4Heal)
                        body.healthComponent?.HealFraction(1f, default);
                    if (phase4SpeedBuff)
                        body.AddBuff(speedBuff);
                    if (clones)
                    {
                        var directorSpawnRequest = new DirectorSpawnRequest(mithrixGlass, new DirectorPlacementRule
                        {
                            placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                            minDistance = 30f,
                            maxDistance = 45f,
                            spawnOnTarget = body.transform,
                        }, RoR2Application.rng)
                        {
                            summonerBodyObject = self.gameObject,
                            teamIndexOverride = TeamIndex.Monster,
                            ignoreTeamMemberLimit = true
                        };

                        DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                    }
                }
            }
            orig(self);
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
            if (!ai)
            {
                return;
            }
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

                        break;

                    case "BrotherHurtMaster(Clone)":
                        AISkillDriver MithrixWeakSlam = (from x in cm.GetComponents<AISkillDriver>()
                                                         where x.customName == "SlamGround"
                                                         select x).First();
                        MithrixWeakSlam.maxUserHealthFraction = Mathf.Infinity;
                        MithrixWeakSlam.movementType = AISkillDriver.MovementType.StrafeMovetarget;

                        AISkillDriver MithrixWeakShards = (from x in cm.GetComponents<AISkillDriver>()
                                                           where x.customName == "Shoot"
                                                           select x).First();
                        MithrixWeakShards.movementType = AISkillDriver.MovementType.StrafeMovetarget;

                        break;
                }
            }
        }

        private void CharacterBody_onBodyAwakeGlobal(CharacterBody cb)
        {
            if (!stat)
            {
                return;
            }
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
                }
            }
        }

        private void Changes()
        {
            var glass = Utils.Paths.GameObject.BrotherGlassBody.Load<GameObject>().GetComponent<CharacterBody>();
            glass.baseMaxHealth = phase4BaseMaxHealth * 0.25f;
            glass.levelMaxHealth = phase4BaseMaxHealth * 0.25f * 0.3f;
            glass.baseDamage = 9f;
            glass.levelDamage = 1.8f;

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
                rotateAroundAxis.slowRotationSpeed = 13f;
                rotateAroundAxis.fastRotationSpeed = 13f;

                var projectileSimple = escapeLine.GetComponent<ProjectileSimple>();
                projectileSimple.desiredForwardSpeed = 50f;
            }
        }
    }
}