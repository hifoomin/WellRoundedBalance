using MonoMod.Cil;
using System;
using UnityEngine.Events;
using WellRoundedBalance.Misc;

namespace WellRoundedBalance.Items.Greens
{
    public class Shuriken : ItemBase<Shuriken>
    {
        public static BuffDef countdown;
        public override string Name => ":: Items :: Greens :: Shuriken";
        public override ItemDef InternalPickup => DLC1Content.Items.PrimarySkillShuriken;

        public override string PickupText => "Activating your Primary skill also throws a shuriken. Recharges over time.";

        public override string DescText => "Activating your <style=cIsUtility>Primary skill</style> also throws a <style=cIsDamage>shuriken</style> that deals <style=cIsDamage>" + d(baseDamage) + "</style> <style=cStack>(+" + d(damagePerStack) + " per stack)</style> base damage. The <style=cIsDamage>shuriken</style> reloads over <style=cIsDamage>" + cooldown + "</style> seconds.";

        [ConfigField("Base Damage", "Decimal.", 4f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 2f)]
        public static float damagePerStack;

        [ConfigField("Projectile Lifetime", "", 10f)]
        public static float projectileLifetime;

        [ConfigField("Cooldown", "", 12f)]
        public static float cooldown;

        [ConfigField("Projectile Speed", "", 80f)]
        public static float projectileSpeed;

        [ConfigField("Enable Boomerang?", "", true)]
        public static bool enableBoomerang;

        [ConfigField("Enable Pierce?", "", true)]
        public static bool enablePierce;

        [ConfigField("Enable Homing?", "", false)]
        public static bool enableHoming;

        [ConfigField("Size Multiplier", "", 2f)]
        public static float sizeMultiplier;

        [ConfigField("Distance Multiplier", "", 0.5f)]
        public static float distanceMultiplier;

        [ConfigField("Proc Coefficient", "", 1f)]
        public static float procCoefficient;

        public float timer;

        public override void Init()
        {
            countdown = ScriptableObject.CreateInstance<BuffDef>();
            countdown.isCooldown = true;
            countdown.isDebuff = false;
            countdown.isHidden = false;
            countdown.canStack = true;
            countdown.buffColor = new Color(0.4151f, 0.4014f, 0.4014f, 1f); // wolfo consistency :kirn:
            countdown.iconSprite = Utils.Paths.BuffDef.bdPrimarySkillShurikenBuff.Load<BuffDef>().iconSprite;
            countdown.name = "Shuriken Cooldown";

            ContentAddition.AddBuffDef(countdown);

            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.RoR2.PrimarySkillShurikenBehavior.FixedUpdate += PrimarySkillShurikenBehavior_FixedUpdate;
            IL.RoR2.PrimarySkillShurikenBehavior.FixedUpdate += PrimarySkillShurikenBehavior_FixedUpdate1;
            IL.RoR2.PrimarySkillShurikenBehavior.FireShuriken += PrimarySkillShurikenBehavior_FireShuriken;
            On.RoR2.PrimarySkillShurikenBehavior.FireShuriken += PrimarySkillShurikenBehavior_FireShuriken1;
            On.RoR2.PrimarySkillShurikenBehavior.Start += PrimarySkillShurikenBehavior_Start;
            On.RoR2.PrimarySkillShurikenBehavior.GetRandomRollPitch += PrimarySkillShurikenBehavior_GetRandomRollPitch;
            On.RoR2.Projectile.ProjectileSimple.Start += ProjectileSimple_Start;
        }

        private void PrimarySkillShurikenBehavior_Start(On.RoR2.PrimarySkillShurikenBehavior.orig_Start orig, PrimarySkillShurikenBehavior self)
        {
            orig(self);
            self.body.AddTimedBuff(countdown, cooldown - 2f / 60f);
        }

        private void PrimarySkillShurikenBehavior_FireShuriken1(On.RoR2.PrimarySkillShurikenBehavior.orig_FireShuriken orig, PrimarySkillShurikenBehavior self)
        {
            orig(self);
            self.body.AddTimedBuff(countdown, cooldown - 2f / 60f);
        }

        private void ProjectileSimple_Start(On.RoR2.Projectile.ProjectileSimple.orig_Start orig, ProjectileSimple self)
        {
            var shurikenProjectile = self.gameObject;
            if (shurikenProjectile.name == "ShurikenProjectile(Clone)")
            {
                if (enablePierce || enableBoomerang && shurikenProjectile.transform.GetChild(0) == null)
                {
                    GameObject hitBox = new("hitBox");
                    hitBox.transform.SetParent(shurikenProjectile.transform);
                    hitBox.transform.localPosition = Vector3.zero;

                    hitBox.AddComponent<HitBox>();

                    hitBox.transform.localScale = new Vector3(sizeMultiplier * 1.2f, sizeMultiplier * 1.2f, sizeMultiplier * 1.2f);

                    shurikenProjectile.layer = 13;
                    hitBox.layer = 14;

                    var hitBoxGroup = shurikenProjectile.GetComponent<HitBoxGroup>();
                    hitBoxGroup.hitBoxes = new HitBox[] { hitBox.GetComponent<HitBox>() };
                }
            }

            orig(self);
        }

        private Quaternion PrimarySkillShurikenBehavior_GetRandomRollPitch(On.RoR2.PrimarySkillShurikenBehavior.orig_GetRandomRollPitch orig, PrimarySkillShurikenBehavior self)
        {
            return Quaternion.identity;
        }

        private void PrimarySkillShurikenBehavior_FireShuriken(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(3f),
                x => x.MatchLdcR4(1f)))
            {
                c.Next.Operand = baseDamage;
                c.Index += 1;
                c.Next.Operand = damagePerStack;
            }
            else
            {
                Logger.LogError("Failed to apply Shuriken Damage hook");
            }
        }

        private void PrimarySkillShurikenBehavior_FixedUpdate1(ILContext il)
        {
            ILCursor c = new(il);

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld(typeof(CharacterBody.ItemBehavior), "stack"),
                x => x.MatchLdcI4(2)))
            {
                c.Index += 1;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 1;
                });
                // stack => 1
                c.Index += 1;
                c.EmitDelegate<Func<int, int>>((useless) =>
                {
                    return 0;
                });
                // 2 => 0
            }
            else
            {
                Logger.LogError("Failed to apply Shuriken Count and Cooldown hook");
            }

            c.Index = 0;

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdcR4(10f)))
            {
                c.Next.Operand = cooldown;
            }
            else
            {
                Main.WRBLogger.LogError("Failed to apply Shuriken Cooldown hook");
            }
        }

        private void PrimarySkillShurikenBehavior_FixedUpdate(On.RoR2.PrimarySkillShurikenBehavior.orig_FixedUpdate orig, PrimarySkillShurikenBehavior self)
        {
            orig(self);
            timer += Time.fixedDeltaTime;
            if (self.inputBank.skill1.down && timer >= 1f)
            {
                if (self.body.GetBuffCount(DLC1Content.Buffs.PrimarySkillShurikenBuff) > 0)
                {
                    self.body.RemoveBuff(DLC1Content.Buffs.PrimarySkillShurikenBuff);
                    self.FireShuriken();
                    timer = 0f;
                }
            }
        }

        public static BoomerangProjectile boomerangProjectile;
        public static ProjectileOverlapAttack projectileOverlapAttack;
        public static ProjectileDotZone projectileDotZone;
        public UnityEvent UnityGames = new();

        private void Changes()
        {
            var shurikenProjectile = Utils.Paths.GameObject.ShurikenProjectile.Load<GameObject>();
            var projectileSimple = shurikenProjectile.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = projectileLifetime;
            projectileSimple.desiredForwardSpeed = projectileSpeed;

            shurikenProjectile.transform.localScale = new Vector3(sizeMultiplier / 7f, sizeMultiplier / 7f, sizeMultiplier / 7f);

            var projectileSingleTargetImpact = shurikenProjectile.GetComponent<ProjectileSingleTargetImpact>();

            var sphereCollider = shurikenProjectile.GetComponent<SphereCollider>();
            sphereCollider.radius = sizeMultiplier / 10f;

            var projectileController = shurikenProjectile.GetComponent<ProjectileController>();
            projectileController.procCoefficient = procCoefficient;

            if (!enableHoming)
            {
                var projectileSteerTowardTarget = shurikenProjectile.GetComponent<ProjectileSteerTowardTarget>();
                projectileSteerTowardTarget.enabled = false;
                var projectileDirectionalTargetFinder = shurikenProjectile.GetComponent<ProjectileDirectionalTargetFinder>();
                projectileDirectionalTargetFinder.enabled = false;
                var projectileTargetComponent = shurikenProjectile.GetComponent<ProjectileTargetComponent>();
                projectileTargetComponent.enabled = false;
            }

            if (enableBoomerang)
            {
                boomerangProjectile = shurikenProjectile.AddComponent<BoomerangProjectile>();
                boomerangProjectile.travelSpeed = projectileSpeed;
                boomerangProjectile.charge = 1f;
                boomerangProjectile.transitionDuration = projectileLifetime / 3f;
                boomerangProjectile.canHitCharacters = false;
                boomerangProjectile.canHitWorld = true;
                boomerangProjectile.distanceMultiplier = distanceMultiplier;
                boomerangProjectile.impactSpark = Utils.Paths.GameObject.ShurikenImpact.Load<GameObject>();
                shurikenProjectile.AddComponent<DestroyStuckObject>();
                UnityGames.AddListener(OnFlyBack);
            }

            if (enablePierce)
            {
                projectileOverlapAttack = shurikenProjectile.AddComponent<ProjectileOverlapAttack>();
                projectileOverlapAttack.damageCoefficient = 1f;
                projectileOverlapAttack.impactEffect = Utils.Paths.GameObject.ShurikenImpact.Load<GameObject>();
                projectileOverlapAttack.forceVector = new Vector3(0f, 0f, 0f);
                projectileOverlapAttack.fireFrequency = 60f;
                projectileOverlapAttack.resetInterval = -1f;

                UnityEngine.Object.Destroy(projectileSingleTargetImpact);

                projectileDotZone = shurikenProjectile.AddComponent<ProjectileDotZone>();
                projectileDotZone.damageCoefficient = 1f / 7f;
                projectileDotZone.attackerFiltering = AttackerFiltering.NeverHitSelf;
                projectileDotZone.impactEffect = Utils.Paths.GameObject.OmniImpactVFXSlash.Load<GameObject>();
                projectileDotZone.forceVector = new Vector3(0f, 0f, 0f);
                projectileDotZone.overlapProcCoefficient = procCoefficient;
                projectileDotZone.fireFrequency = 8f;
                projectileDotZone.resetFrequency = -1f;
                projectileDotZone.lifetime = -1f;
            }
        }

        public void OnFlyBack()
        {
            if (boomerangProjectile && UnityGames != null)
            {
                if (projectileOverlapAttack)
                    projectileOverlapAttack.ResetOverlapAttack();
                if (projectileDotZone)
                    projectileDotZone.ResetOverlap();
            }
        }
    }
}