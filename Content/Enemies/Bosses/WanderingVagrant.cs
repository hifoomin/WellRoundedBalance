using System;
using EntityStates.BrotherMonster;
using IL.RoR2.Achievements.Treebot;
using RoR2.Skills;
using static RoR2.MasterCatalog;

namespace WellRoundedBalance.Enemies.Bosses {
    public class WanderingVagrant : EnemyBase<WanderingVagrant>
    {
        public override string Name => "::: Bosses ::: Wandering Vagrant";
        [ConfigField("Replace Primary?", "Replaces Vagrant's primary with a trispread of orbs.", true)]
        public static bool ReplacePrimary;
        [ConfigField("Nova Rework", "Reworks the Vagrant Nova to trigger once when the vagrant would normally die, granting it invincibility for the duration", true)]
        public static bool NovaTweak;
        [ConfigField("Change Vagrant Stats and Scale", "Tweaks the Wandering Vagrant to be much smaller and much more agile, highly recommended to leave on", true)]
        public static bool VagrantChanges;
        [ConfigField("Vagrant Chain Dash", "Gives Wandering Vagrant a chain melee ram attack", true)]
        public static bool EnableChainDash;

        // lazy init bc i dont want to subscribe to mastercat init just for 1 index
        public MasterIndex VagrantIndex {
            get {
                if (_VagrantIndex == MasterIndex.none) {
                    _VagrantIndex = MasterCatalog.FindMasterIndex("VagrantMaster");
                }

                return _VagrantIndex;
            }
        }

        public BodyIndex VagrantBody {
            get {
                if (_VagrantBody == BodyIndex.None) {
                    _VagrantBody = BodyCatalog.FindBodyIndex("VagrantBody");
                }

                return _VagrantBody;
            }
        }

        private MasterIndex _VagrantIndex = MasterIndex.none;
        private BodyIndex _VagrantBody = BodyIndex.None;

        public override void Hooks()
        {
            On.EntityStates.VagrantMonster.Weapon.JellyBarrage.OnEnter += ReplacePrimaryHook;
            On.EntityStates.VagrantMonster.ChargeMegaNova.OnEnter += GrantInvuln;
            On.EntityStates.VagrantMonster.FireMegaNova.Detonate += LowTierGod;
            On.RoR2.HealthComponent.TakeDamage += LTGMK2;

            TweakVagrantPrefab();
        }

        private void LTGMK2(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body.bodyIndex == VagrantBody && NovaTweak) {
                if (damageInfo.damage >= self.combinedHealth + self.barrier && damageInfo.damageType != DamageType.BypassOneShotProtection) {
                    EntityStateMachine machine = EntityStateMachine.FindByCustomName(self.gameObject, "Body");
                    machine.SetNextState(new EntityStates.VagrantMonster.ChargeMegaNova());
                    return;
                }
            }

            orig(self, damageInfo);
        }

        public void TweakMaster() {
            GameObject master = Utils.Paths.GameObject.VagrantMaster.Load<GameObject>();

            AISkillDriver driver = master.AddComponent<AISkillDriver>();
            driver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            driver.activationRequiresAimConfirmation = false;
            driver.activationRequiresAimTargetLoS = false;
            driver.activationRequiresTargetLoS = false;
            driver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            driver.customName = "DashAtEnemy";
            driver.maxDistance = 90f;
            driver.minDistance = 20f;
            driver.skillSlot = SkillSlot.Utility;
            driver.requireSkillReady = true;
            driver.noRepeat = true;

            On.RoR2.CharacterAI.BaseAI.Awake += (orig, self) => {
                orig(self);

                if (self.master.masterIndex == VagrantIndex) {
                    List<AISkillDriver> drivers = self.skillDrivers.ToList();
                    AISkillDriver last = drivers[drivers.Count - 1];
                    drivers.RemoveAt(drivers.Count - 1);
                    drivers.Insert(0, last);
                }
            };
        }

        public void ApplySkillDef(SkillLocator loc) {
            GenericSkill slot = loc.gameObject.AddComponent<GenericSkill>();
            slot.skillName = "WR-B";
            
            SkillFamily family = ScriptableObject.CreateInstance<SkillFamily>();
            (family as ScriptableObject).name = "VagrantChainDash";
            family.variants = new SkillFamily.Variant[] {
                new SkillFamily.Variant() {
                    skillDef = SetupDashSkillDef(),
                    unlockableName = null,
                    viewableNode = new("j", false, null)
                }
            };

            ContentAddition.AddSkillFamily(family);

            slot._skillFamily = family;
            loc.utility = slot;
        }

        public SkillDef SetupDashSkillDef() {
            SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
            def.activationStateMachineName = "Body";
            def.baseMaxStock = 1;
            def.beginSkillCooldownOnSkillEnd = true;
            def.skillNameToken = "LGM-A";
            def.skillDescriptionToken = "BAL-S";
            def.cancelSprintingOnActivation = true;
            def.canceledFromSprinting = false;
            def.isCombatSkill = true;
            def.stockToConsume = 1;
            def.baseRechargeInterval = 8f;
            def.activationState = new(typeof(Vagrant.ChainDashes));

            ContentAddition.AddSkillDef(def);
            return def;
        }

        public void TweakVagrantPrefab() {
            GameObject prefab = Utils.Paths.GameObject.VagrantBody15.Load<GameObject>();
            
            CharacterBody body = prefab.GetComponent<CharacterBody>();
            ModelLocator loc = body.GetComponent<ModelLocator>();

            if (VagrantChanges) {
                // stats
                body.baseMoveSpeed = 14f;
                body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

                // scale & rot
                loc.modelBaseTransform.localScale = new(3f, 3f, 3f);
                loc.modelBaseTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            }

            if (EnableChainDash) {
                ApplySkillDef(body.GetComponent<SkillLocator>());
                TweakMaster();

                foreach (QuaternionPID p in body.GetComponents<QuaternionPID>()) {
                    p.gain = 20;
                }
            }

            if (ReplacePrimary) {
                SkillDef skill = Utils.Paths.SkillDef.VagrantBodyJellyBarrage.Load<SkillDef>();
                skill.beginSkillCooldownOnSkillEnd = true;
                skill.baseRechargeInterval = 3.75f;
            }

            On.RoR2.CharacterBody.Start += (orig, self) => {
                orig(self);

                if (self.bodyIndex == VagrantBody && EnableChainDash) {
                    ModelLocator loc2 = self.modelLocator;

                    // melee hitbox
                    Transform mdl = loc2._modelTransform;

                    HitBoxGroup group = mdl.gameObject.AddComponent<HitBoxGroup>();
                    group.groupName = "VagrantChainDash";

                    GameObject hb = new("ChainDashHitbox");
                    hb.transform.SetParent(mdl);
                    hb.transform.localScale = new(3, 3, 5);
                    HitBox hitbox = hb.AddComponent<HitBox>();

                    group.hitBoxes = new HitBox[] { hitbox };

                    self.GetComponent<CapsuleCollider>().direction = 2;

                    hb.transform.localPosition = Vector3.zero;
                    hb.transform.position = Vector3.zero;
                }
            };
        }

        private void LowTierGod(On.EntityStates.VagrantMonster.FireMegaNova.orig_Detonate orig, EntityStates.VagrantMonster.FireMegaNova self)
        {
            if (NovaTweak) {
                self.outer.SetNextStateToMain();
                self.healthComponent.Suicide(null, null, DamageType.BypassOneShotProtection);
                return;
            }

            orig(self);
        }

        private void GrantInvuln(On.EntityStates.VagrantMonster.ChargeMegaNova.orig_OnEnter orig, EntityStates.VagrantMonster.ChargeMegaNova self)
        {
            if (NovaTweak) {
                self.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            }

            orig(self);
        }

        private void ReplacePrimaryHook(On.EntityStates.VagrantMonster.Weapon.JellyBarrage.orig_OnEnter orig, EntityStates.VagrantMonster.Weapon.JellyBarrage self)
        {
            if (ReplacePrimary) {
                self.outer.SetNextState(new Vagrant.OrbSpread());
                return;
            }

            orig(self);
        }

        private class ForceLook : MonoBehaviour {
            public InputBankTest input;
            public CharacterDirection direction;

            public void Start() {
                input = GetComponent<InputBankTest>();
                direction = GetComponent<CharacterDirection>();
            }

            public void FixedUpdate() {
                base.transform.forward = input.aimDirection;
            }
        }
    }
}