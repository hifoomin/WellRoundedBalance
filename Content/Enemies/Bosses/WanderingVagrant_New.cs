/*
using System;
using EntityStates.BrotherMonster;
using IL.RoR2.Achievements.Treebot;
using RoR2.Skills;
using static RoR2.MasterCatalog;

namespace WellRoundedBalance.Enemies.Bosses {
    public class WanderingVagrant_New : EnemyBase<WanderingVagrant_New>
    {
        public override string Name => "::: Bosses ::: Wandering Vagrant";
        [ConfigField("Replace Primary?", "Replaces Vagrant's primary with a trispread of orbs.", true)]
        public static bool ReplacePrimary;
        [ConfigField("Nova Rework", "Reworks the Vagrant Nova to trigger once when the vagrant would normally die, granting it invincibility for the duration", true)]
        public static bool NovaTweak;
        [ConfigField("Change Vagrant Stats and Scale", "Tweaks the Wandering Vagrant to be much smaller and much more agile, highly recommended to leave on", true)]
        public static bool VagrantChanges;
        [ConfigField("Vagrant Chain Dash", "Gives Wandering Vagrant a chain dash which spews seeking orbs", true)]
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

        public static GameObject VagrantSeekerOrb;

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
            driver.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            driver.customName = "DashAtEnemy";
            driver.maxDistance = 140f;
            driver.minDistance = 30f;
            driver.skillSlot = SkillSlot.Utility;
            driver.requireSkillReady = true;
            driver.noRepeat = true;
            driver.aimType = AISkillDriver.AimType.MoveDirection;
            driver.moveInputScale = 2f;

            On.RoR2.CharacterAI.BaseAI.Awake += (orig, self) => {
                orig(self);

                if (self.master.masterIndex == VagrantIndex) {
                    List<AISkillDriver> drivers = self.skillDrivers.ToList();
                    AISkillDriver last = drivers[drivers.Count - 1];
                    drivers.RemoveAt(drivers.Count - 1);
                    drivers.Insert(0, last);
                    drivers.RemoveAll(x => x.skillSlot == SkillSlot.Special);
                    self.skillDrivers = drivers.ToArray();
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
            def.baseRechargeInterval = 12f;
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
                body.baseMoveSpeed = 16f;
                body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

                body.GetComponent<RigidbodyMotor>().canTakeImpactDamage = false;

                // scale & rot
                loc.modelBaseTransform.localScale = new(4f, 4f, 4f);
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
                skill.baseRechargeInterval = 4.75f;
            }

            VagrantSeekerOrb = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.VagrantCannon.Load<GameObject>(), "VagrantSeekerBolt");
            GameObject VagrantSeekerGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.VagrantCannonGhost.Load<GameObject>(), "VagrantSeekerBolt");
            VagrantSeekerGhost.AddComponent<VagrantSeekerGhostController>();

            LineRenderer renderer = VagrantSeekerGhost.AddComponent<LineRenderer>();
            renderer.startWidth = 0.5f;
            renderer.endWidth = 0.5f;
            renderer.material = Utils.Paths.Material.matCaptainTracerTrail.Load<Material>();

            VagrantSeekerOrb.AddComponent<ProjectileTargetComponent>();
            var finder = VagrantSeekerOrb.AddComponent<ProjectileSphereTargetFinder>();
            finder.allowTargetLoss = false;
            finder.lookRange = 650f;
            finder.testLoS = false;

            VagrantSeekerOrb.GetComponent<ProjectileSimple>().updateAfterFiring = true;
            VagrantSeekerOrb.GetComponent<ProjectileSimple>().enableVelocityOverLifetime = false;

            VagrantSeekerOrb.GetComponent<ProjectileController>().ghostPrefab = VagrantSeekerGhost;
            VagrantSeekerOrb.AddComponent<VagrantSeekerController>();

            VagrantSeekerOrb.GetComponent<ProjectileImpactExplosion>().blastRadius = 1.5f;
            VagrantSeekerOrb.GetComponent<ProjectileImpactExplosion>().blastDamageCoefficient = 1f;

            ContentAddition.AddProjectile(VagrantSeekerOrb);
        }

        private class VagrantSeekerController : MonoBehaviour {
            public ProjectileSimple simple;
            public ProjectileTargetComponent targetComp;
            public float duration = 1.5f;
            public float ramSpeed = 165f;
            public float initialSpeed = 40f;
            public float speedDecPerSec;
            public bool begunRam = false;
            public Vector3 forward;
            public void Start() {
                simple = GetComponent<ProjectileSimple>();
                initialSpeed = 60f * UnityEngine.Random.Range(0.75f, 2f);
                targetComp = GetComponent<ProjectileTargetComponent>();
                simple.lifetime = 20;
                simple.desiredForwardSpeed = initialSpeed;
                speedDecPerSec = initialSpeed / (duration - 1f);
                forward = base.transform.forward;

                ProjectileController controller = GetComponent<ProjectileController>();
                controller.ghost.GetComponent<VagrantSeekerGhostController>().component = targetComp;
                controller.ghost.GetComponent<VagrantSeekerGhostController>().owner = this;
            }

            public void FixedUpdate() {
                if (duration >= 0f) {
                    initialSpeed -= speedDecPerSec * Time.fixedDeltaTime;
                    simple.desiredForwardSpeed = Mathf.Max(initialSpeed, 0f);
                    base.transform.forward = forward;
                    duration -= Time.fixedDeltaTime;
                }

                if (duration <= 0f && !begunRam) {
                    begunRam = true;

                    if (targetComp.target) {
                        Vector3 facing = (targetComp.target.position - base.transform.position).normalized;
                        base.transform.forward = facing;
                        simple.desiredForwardSpeed = ramSpeed;
                    }
                    else {
                        simple.lifetime = 0f;
                    }
                }
            }
        }

        private class VagrantSeekerGhostController : MonoBehaviour {
            public static List<Color> colors = new() {
                Color.red, Color.yellow, Color.green, Color.cyan
            };

            public ProjectileTargetComponent component;
            public VagrantSeekerController owner;
            public LineRenderer lr;
            public Color32 color;

            public void Start() {
                color = colors.GetRandom();

                lr = GetComponent<LineRenderer>();
                lr.startColor = color;
                lr.endColor = color;

                foreach (Renderer renderer in GetComponentsInChildren<Renderer>()) {
                    if (renderer != lr) {
                        renderer.material.SetColor("_Color", color);
                        renderer.material.SetInt("_FEON", 0);
                        renderer.material.SetInt("_FlowmapOn", 0);
                        renderer.material.SetShaderKeywords(new string[0]);
                    }
                }
            }

            public void Update() {
                if (!component.target || owner.begunRam) {
                    lr.widthMultiplier = 0f;
                    return;
                }

                lr.SetPosition(0, base.transform.position);
                Ray ray = new Ray(base.transform.position, (component.target.position - base.transform.position).normalized);
                lr.SetPosition(1, ray.GetPoint(400));

                lr.widthMultiplier = 1f - ((1.5f - owner.duration) / 1.5f);

                if (lr.widthMultiplier <= 0.05f) {
                    lr.widthMultiplier = 1.5f;
                    lr.startColor = Color.white;
                    lr.endColor = Color.white;
                }
            }
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
    }
}
*/