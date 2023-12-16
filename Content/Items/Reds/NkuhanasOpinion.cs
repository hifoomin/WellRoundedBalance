using MonoMod.Cil;
using RoR2.Orbs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WellRoundedBalance.Items.Greens;

namespace WellRoundedBalance.Items.Reds
{
    public class NkuhanasOpinion : ItemBase<NkuhanasOpinion>
    {
        public override string Name => ":: Items ::: Reds :: Nkuhanas Opinion";
        public override ItemDef InternalPickup => RoR2Content.Items.NovaOnHeal;

        public override string PickupText => "Activating your Special skill also launches a skull. Recharges over time.";

        public override string DescText => (baseDamageFromHealing > 0 || damageFromHealingPerStack > 0 ? "Store <style=cIsHealing>" + d(baseDamageFromHealing) + "</style> " +
                                           (damageFromHealingPerStack > 0 ? "<style=cStack>(+" + d(damageFromHealingPerStack) + " per stack)</style> " : "") +
                                           " of healing as <style=cIsHealing>Soul Elegy</style>. " : "") +
                                           "Activating your <style=cIsUtility>Special skill</style> also fires a <style=cIsDamage>skull</style> that deals <style=cIsDamage>" + d(baseDamage) + "</style>" +
                                           (damagePerStack > 0 ? "<style=cStack>(+" + d(damagePerStack) + " per stack</style>" : "") + " damage" +
                                           (baseDamageFromHealing > 0 || damageFromHealingPerStack > 0 ? ", plus all <style=cIsHealing>Soul Elegy</style> as <style=cIsDamage>extra damage</style>. " : ". ") +
                                           "The <style=cIsDamage>skull</style> reinvigorates over <style=cIsDamage>" + cooldown + "</style> seconds.";

        // guys I just had to change Soul Energy to Soul Elegy cause Termina

        [ConfigField("Base Damage", "Decimal.", 3.5f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 0f)]
        public static float damagePerStack;

        [ConfigField("Base Extra Damage From Healing Percent", "Decimal.", 0.5f)]
        public static float baseDamageFromHealing;

        [ConfigField("Extra Damage From Healing Percent Per Stack", "Decimal.", 0.25f)]
        public static float damageFromHealingPerStack;

        [ConfigField("Cooldown", "", 10f)]
        public static float cooldown;

        [ConfigField("Proc Coefficient", 0.33f)]
        public static float procCoefficient;

        public static GameObject projectile;
        public static GameObject hitBox;
        public static BuffDef cooldownBuff;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            projectile = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.MinorConstructProjectile.Load<GameObject>(), "Nkuhanas Skull Projectile");

            var sphereCollider = projectile.GetComponent<SphereCollider>();
            sphereCollider.material = Utils.Paths.PhysicMaterial.physmatDefault.Load<PhysicMaterial>();

            var projectileSimple = projectile.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = 8f;
            projectileSimple.lifetimeExpiredEffect = Utils.Paths.GameObject.PoisonNovaProc.Load<GameObject>();
            projectileSimple.desiredForwardSpeed = 100f;
            projectileSimple.enableVelocityOverLifetime = true;
            projectileSimple.velocityOverLifetime = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 3f));

            projectile.RemoveComponent<ProjectileSingleTargetImpact>();

            projectile.layer = LayerIndex.debris.intVal;

            hitBox = new GameObject("Hitbox");
            hitBox.transform.parent = projectile.transform;
            hitBox.AddComponent<HitBox>();
            hitBox.layer = LayerIndex.projectile.intVal;

            var hitBoxGroup = projectile.AddComponent<HitBoxGroup>();
            hitBoxGroup.hitBoxes = new HitBox[] { hitBox.GetComponent<HitBox>() };

            var projectileDotZone = projectile.AddComponent<ProjectileDotZone>();
            projectileDotZone.damageCoefficient = 1f;
            projectileDotZone.attackerFiltering = AttackerFiltering.NeverHitSelf;
            projectileDotZone.overlapProcCoefficient = procCoefficient * globalProc;
            projectileDotZone.fireFrequency = 60f;
            projectileDotZone.resetFrequency = -1f;
            projectileDotZone.lifetime = -1f;

            var projectileController = projectile.GetComponent<ProjectileController>();

            projectile.transform.localScale *= 7f;

            var newGhost = PrefabAPI.InstantiateClone(Utils.Paths.GameObject.DevilOrbEffect.Load<GameObject>(), "Nkuhanas Skull Ghost", false);

            newGhost.RemoveComponent<OrbEffect>();

            for (int i = 0; i < newGhost.transform.childCount; i++)
            {
                var child = newGhost.transform.GetChild(i);
                child.localScale *= 7f;
            }

            var projectileGhostController = newGhost.AddComponent<ProjectileGhostController>();

            projectileController.ghostPrefab = newGhost;

            /*
            var projectileOverlapAttack = projectile.AddComponent<ProjectileOverlapAttack>();
            projectileOverlapAttack.damageCoefficient = 1f;
            projectileOverlapAttack.overlapProcCoefficient = procCoefficient * globalProc;
            */

            PrefabAPI.RegisterNetworkPrefab(projectile);

            // IL.RoR2.HealthComponent.ServerFixedUpdate += HealthComponent_ServerFixedUpdate;
            // On.RoR2.Orbs.DevilOrb.Begin += DevilOrb_Begin;
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal1;
            IL.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
            On.RoR2.EffectComponent.Start += HonestlyJustFuckOff;
        }

        private void HonestlyJustFuckOff(On.RoR2.EffectComponent.orig_Start orig, EffectComponent self)
        {
            Transform transform = null;
            if ((self.positionAtReferencedTransform | self.parentToReferencedTransform) && self.effectData != null)
            {
                transform = self.effectData.ResolveChildLocatorTransformReference();
            }
            if (transform)
            {
                if (self.positionAtReferencedTransform)
                {
                    self.transform.position = transform.position;
                    self.transform.rotation = transform.rotation;
                }
                if (self.parentToReferencedTransform)
                {
                    self.transform.SetParent(transform, true);
                }
            }
            if (self.applyScale && self.effectData != null)
            {
                float scale = self.effectData.scale;
                if (!self.disregardZScale)
                {
                    self.transform.localScale = new Vector3(scale, scale, scale);
                    return;
                }
                self.transform.localScale = new Vector3(scale, scale, self.transform.localScale.z);
            }
        }

        private float HealthComponent_Heal1(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            var body = self.body;
            if (body)
            {
                var inventory = body.inventory;
                if (inventory)
                {
                    var stack = inventory.GetItemCount(RoR2Content.Items.NovaOnHeal);
                    if (stack > 0)
                    {
                        var controller = self.GetComponent<NkuhanasOpinionController>();
                        if (controller)
                        {
                            controller.healPool += amount * (baseDamageFromHealing + damageFromHealingPerStack * (stack - 1));
                        }
                    }
                }
            }
            return orig(self, amount, procChainMask, nonRegen);
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody body)
        {
            if (NetworkServer.active)
            {
                var stack = body.inventory.GetItemCount(RoR2Content.Items.NovaOnHeal);
                body.AddItemBehavior<NkuhanasOpinionController>(stack);
            }
        }

        private void HealthComponent_Heal(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld("RoR2.HealthComponent/ItemCounts", "novaOnHeal"),
                x => x.MatchLdcI4(0)))
            {
                c.Index += 2;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return int.MaxValue;
                });
            }
            else
            {
                Main.WRBLogger.LogError("Failed to Apply Nkuhanas Opinion Deletion hook");
            }
        }

        private void DevilOrb_Begin(On.RoR2.Orbs.DevilOrb.orig_Begin orig, RoR2.Orbs.DevilOrb self)
        {
            self.procCoefficient = procCoefficient * globalProc;
            orig(self);
        }

        private void HealthComponent_ServerFixedUpdate(ILContext il)
        {
            ILCursor c = new(il);
            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(2.5f)))
            {
                c.Next.Operand = baseDamage;
            }
            else
            {
                Logger.LogError("Failed to apply Nkuhanas Opinion Damage hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(40f)))
            {
                // c.Next.Operand = baseRange;
            }
            else
            {
                Logger.LogError("Failed to apply Nkuhanas Opinion Range hook");
            }
        }
    }

    public class NkuhanasOpinionController : CharacterBody.ItemBehavior
    {
        public SkillLocator skillLocator;
        public float damage = 4f;
        public float cooldown = 10f;
        public float healPool;

        public GameObject skullObject;

        public void Start()
        {
            damage = NkuhanasOpinion.baseDamage + NkuhanasOpinion.damagePerStack * (stack - 1);
            cooldown = NkuhanasOpinion.cooldown;

            skillLocator = GetComponent<SkillLocator>();
            body.onSkillActivatedServer += Body_onSkillActivatedServer;
        }

        private void Body_onSkillActivatedServer(GenericSkill skill)
        {
            var body = skill.GetComponent<CharacterBody>();
            if (!body)
            {
                return;
            }
            if (skill != skillLocator.special)
            {
                return;
            }

            var inputBank = body.inputBank;
            if (!inputBank)
            {
                return;
            }

            var aimDirection = inputBank.GetAimRay().direction;

            StartCoroutine(FireProjectile(aimDirection));
        }

        public IEnumerator FireProjectile(Vector3 aimDirection)
        {
            var fpi = new FireProjectileInfo()
            {
                owner = gameObject,
                crit = body.RollCrit(),
                damage = body.damage * damage + healPool,
                damageColorIndex = DamageColorIndex.Poison,
                force = 3000f,
                position = body.corePosition,
                procChainMask = default,
                projectilePrefab = NkuhanasOpinion.projectile,
                rotation = Util.QuaternionSafeLookRotation(aimDirection)
            };

            healPool = 0f;

            ProjectileManager.instance.FireProjectile(fpi);

            yield return null;
        }

        public void OnDestroy()
        {
            body.onSkillActivatedServer -= Body_onSkillActivatedServer;
        }
    }
}