using UnityEngine.SceneManagement;

namespace WellRoundedBalance.Enemies
{
    public static class Mithrix
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

        public static GameObject ramp1;
        public static GameObject ramp2;
        public static GameObject ramp3;
        public static GameObject rocks;

        public static void Init()
        {
            CharacterBody.onBodyAwakeGlobal += CharacterBody_onBodyAwakeGlobal;
            CharacterMaster.onStartGlobal += CharacterMaster_onStartGlobal;
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
            On.EntityStates.Missions.BrotherEncounter.Phase1.OnEnter += Phase1_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.Phase2.OnEnter += Phase2_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.Phase3.OnEnter += Phase3_OnEnter;
            On.EntityStates.Missions.BrotherEncounter.Phase4.OnEnter += Phase4_OnEnter;
            On.EntityStates.BrotherMonster.HoldSkyLeap.OnEnter += HoldSkyLeap_OnEnter;
            On.EntityStates.BrotherMonster.ExitSkyLeap.OnEnter += ExitSkyLeap_OnEnter;
            On.EntityStates.BrotherMonster.WeaponSlam.OnEnter += WeaponSlam_OnEnter;
            On.EntityStates.BrotherMonster.BaseSlideState.OnEnter += BaseSlideState_OnEnter;
            On.EntityStates.BrotherMonster.SprintBash.OnEnter += SprintBash_OnEnter;
            On.EntityStates.BrotherMonster.Weapon.FireLunarShards.OnEnter += FireLunarShards_OnEnter;
            On.EntityStates.BrotherMonster.FistSlam.OnEnter += FistSlam_OnEnter;
            On.EntityStates.BrotherMonster.SpellChannelEnterState.OnEnter += SpellChannelEnterState_OnEnter;
            On.EntityStates.BrotherMonster.SpellChannelState.OnEnter += SpellChannelState_OnEnter;
            On.EntityStates.BrotherMonster.SpellChannelExitState.OnEnter += SpellChannelExitState_OnEnter;
            On.EntityStates.BrotherMonster.StaggerEnter.OnEnter += StaggerEnter_OnEnter;
            On.EntityStates.BrotherMonster.StaggerExit.OnEnter += StaggerExit_OnEnter;
            On.EntityStates.BrotherMonster.StaggerLoop.OnEnter += StaggerLoop_OnEnter;
            On.EntityStates.BrotherMonster.TrueDeathState.OnEnter += TrueDeathState_OnEnter;
            On.EntityStates.BrotherMonster.UltChannelState.OnEnter += UltChannelState_OnEnter;
            On.EntityStates.BrotherHaunt.FireRandomProjectiles.OnEnter += FireRandomProjectiles_OnEnter;
        }

        private static void UltChannelState_OnEnter(On.EntityStates.BrotherMonster.UltChannelState.orig_OnEnter orig, EntityStates.BrotherMonster.UltChannelState self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.UltChannelState.totalWaves = 8;
                EntityStates.BrotherMonster.UltChannelState.maxDuration = 8f;
            }
            orig(self);
        }

        private static void FireRandomProjectiles_OnEnter(On.EntityStates.BrotherHaunt.FireRandomProjectiles.orig_OnEnter orig, EntityStates.BrotherHaunt.FireRandomProjectiles self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherHaunt.FireRandomProjectiles.maximumCharges = 150;
                EntityStates.BrotherHaunt.FireRandomProjectiles.chargeRechargeDuration = 0.06f;
                EntityStates.BrotherHaunt.FireRandomProjectiles.chanceToFirePerSecond = 0.33f;
                EntityStates.BrotherHaunt.FireRandomProjectiles.damageCoefficient = 9f;
            }
            orig(self);
        }

        private static void TrueDeathState_OnEnter(On.EntityStates.BrotherMonster.TrueDeathState.orig_OnEnter orig, EntityStates.BrotherMonster.TrueDeathState self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.TrueDeathState.dissolveDuration = 5f;
            }
            orig(self);
        }

        private static void StaggerLoop_OnEnter(On.EntityStates.BrotherMonster.StaggerLoop.orig_OnEnter orig, EntityStates.BrotherMonster.StaggerLoop self)
        {
            if (enableSkillAndAIChanges)
            {
                self.duration = 0f;
            }
            orig(self);
        }

        private static void StaggerExit_OnEnter(On.EntityStates.BrotherMonster.StaggerExit.orig_OnEnter orig, EntityStates.BrotherMonster.StaggerExit self)
        {
            if (enableSkillAndAIChanges)
            {
                self.duration = 0f;
            }
            orig(self);
        }

        private static void StaggerEnter_OnEnter(On.EntityStates.BrotherMonster.StaggerEnter.orig_OnEnter orig, EntityStates.BrotherMonster.StaggerEnter self)
        {
            if (enableSkillAndAIChanges)
            {
                self.duration = 0f;
            }
            orig(self);
        }

        private static void SpellChannelExitState_OnEnter(On.EntityStates.BrotherMonster.SpellChannelExitState.orig_OnEnter orig, EntityStates.BrotherMonster.SpellChannelExitState self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.SpellChannelExitState.lendInterval = 0f;
                EntityStates.BrotherMonster.SpellChannelExitState.duration = 2.5f;
            }
            orig(self);
        }

        private static void SpellChannelState_OnEnter(On.EntityStates.BrotherMonster.SpellChannelState.orig_OnEnter orig, EntityStates.BrotherMonster.SpellChannelState self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.SpellChannelState.stealInterval = 0f;
                EntityStates.BrotherMonster.SpellChannelState.delayBeforeBeginningSteal = 0f;
                EntityStates.BrotherMonster.SpellChannelState.maxDuration = 1f;
            }
            orig(self);
        }

        private static void SpellChannelEnterState_OnEnter(On.EntityStates.BrotherMonster.SpellChannelEnterState.orig_OnEnter orig, EntityStates.BrotherMonster.SpellChannelEnterState self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.SpellChannelEnterState.duration = 3f;
            }
            orig(self);
        }

        private static void FistSlam_OnEnter(On.EntityStates.BrotherMonster.FistSlam.orig_OnEnter orig, EntityStates.BrotherMonster.FistSlam self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.FistSlam.waveProjectileDamageCoefficient = 1.2f;
                EntityStates.BrotherMonster.FistSlam.healthCostFraction = 0f;
                EntityStates.BrotherMonster.FistSlam.waveProjectileCount = 20;
                EntityStates.BrotherMonster.FistSlam.baseDuration = 3.5f;
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

        private static void SprintBash_OnEnter(On.EntityStates.BrotherMonster.SprintBash.orig_OnEnter orig, EntityStates.BrotherMonster.SprintBash self)
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

        private static void BaseSlideState_OnEnter(On.EntityStates.BrotherMonster.BaseSlideState.orig_OnEnter orig, EntityStates.BrotherMonster.BaseSlideState self)
        {
            if (enableSkillAndAIChanges)
            {
                switch (self)
                {
                    case EntityStates.BrotherMonster.SlideBackwardState:
                        self.slideRotation = Quaternion.identity;
                        break;

                    case EntityStates.BrotherMonster.SlideLeftState:
                        self.slideRotation = Quaternion.AngleAxis(-40f, Vector3.up);
                        break;

                    case EntityStates.BrotherMonster.SlideRightState:
                        self.slideRotation = Quaternion.AngleAxis(40f, Vector3.up);
                        break;
                }
            }
            orig(self);
        }

        private static void WeaponSlam_OnEnter(On.EntityStates.BrotherMonster.WeaponSlam.orig_OnEnter orig, EntityStates.BrotherMonster.WeaponSlam self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.WeaponSlam.waveProjectileArc = 360f;
                EntityStates.BrotherMonster.WeaponSlam.waveProjectileCount = 8;
                EntityStates.BrotherMonster.WeaponSlam.waveProjectileForce = -2000f;
                EntityStates.BrotherMonster.WeaponSlam.weaponForce = -3000f;
                EntityStates.BrotherMonster.WeaponSlam.duration = 3f;
            }
            orig(self);
        }

        private static void ExitSkyLeap_OnEnter(On.EntityStates.BrotherMonster.ExitSkyLeap.orig_OnEnter orig, EntityStates.BrotherMonster.ExitSkyLeap self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.ExitSkyLeap.waveProjectileCount = 20;
                EntityStates.BrotherMonster.ExitSkyLeap.waveProjectileDamageCoefficient = 2.5f;
            }
            orig(self);
        }

        private static void HoldSkyLeap_OnEnter(On.EntityStates.BrotherMonster.HoldSkyLeap.orig_OnEnter orig, EntityStates.BrotherMonster.HoldSkyLeap self)
        {
            if (enableSkillAndAIChanges)
            {
                EntityStates.BrotherMonster.HoldSkyLeap.duration = 2f;
                if (NetworkServer.active)
                {
                    Util.CleanseBody(self.characterBody, true, false, false, true, true, false);
                }
            }
            orig(self);
        }

        private static void Phase4_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase4.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase4 self)
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

        private static void Phase3_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase3.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase3 self)
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

        private static void Phase2_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase2.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase2 self)
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

        private static void Phase1_OnEnter(On.EntityStates.Missions.BrotherEncounter.Phase1.orig_OnEnter orig, EntityStates.Missions.BrotherEncounter.Phase1 self)
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

        private static void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
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

        private static void CharacterMaster_onStartGlobal(CharacterMaster cm)
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

        private static void CharacterBody_onBodyAwakeGlobal(CharacterBody cb)
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
    }
}