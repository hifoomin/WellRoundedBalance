using MonoMod.Cil;
using System;
using UnityEngine.Events;

namespace WellRoundedBalance.Items.Greens
{
    public class Shuriken : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Shuriken";
        public override string InternalPickupToken => "primarySkillShuriken";

        public override string PickupText => "Activating your Primary skill also throws a shuriken. Recharges over time.";

        public override string DescText => "Activating your <style=cIsUtility>Primary skill</style> also throws a <style=cIsDamage>shuriken</style> that deals <style=cIsDamage>" + d(baseDamage) + "</style> <style=cStack>(+" + d(damagePerStack) + " per stack)</style> base damage. The <style=cIsDamage>shuriken</style> reloads over <style=cIsUtility>10</style> seconds.";

        [ConfigField("Base Damage", "Decimal.", 5f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 2.5f)]
        public static float damagePerStack;

        [ConfigField("Projectile Lifetime", "", 10f)]
        public static float projectileLifetime;

        [ConfigField("Projectile Speed", "", 80f)]
        public static float projectileSpeed;

        [ConfigField("Enable Boomerang?", "", true)]
        public static bool enableBoomerang;

        [ConfigField("Enable Pierce?", "", true)]
        public static bool enablePierce;

        [ConfigField("Enable Homing?", "", false)]
        public static bool enableHoming;

        [ConfigField("Size Multiplier", "Not actually to scale with Vanilla, I'm not sure why.", 2f)]
        public static float sizeMultiplier;

        [ConfigField("Distance Multiplier", "", 0.5f)]
        public static float distanceMultiplier;

        public float timer;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            On.RoR2.PrimarySkillShurikenBehavior.FixedUpdate += PrimarySkillShurikenBehavior_FixedUpdate;
            IL.RoR2.PrimarySkillShurikenBehavior.FixedUpdate += PrimarySkillShurikenBehavior_FixedUpdate1;
            IL.RoR2.PrimarySkillShurikenBehavior.FireShuriken += PrimarySkillShurikenBehavior_FireShuriken;
            On.RoR2.PrimarySkillShurikenBehavior.GetRandomRollPitch += PrimarySkillShurikenBehavior_GetRandomRollPitch;
            Changes();
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
                Main.WRBLogger.LogError("Failed to apply Shuriken Damage hook");
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
                Main.WRBLogger.LogError("Failed to apply Shuriken Count and Cooldown hook");
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
        public static GameObject shuriken;
        public UnityEvent UnityGames = new();

        private void Changes()
        {
            var shurikenProjectile = Utils.Paths.GameObject.ShurikenProjectile.Load<GameObject>();
            shuriken = shurikenProjectile;
            var projectileSimple = shurikenProjectile.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = projectileLifetime;
            projectileSimple.desiredForwardSpeed = projectileSpeed;

            shurikenProjectile.transform.localScale = new Vector3(sizeMultiplier / 7f, sizeMultiplier / 7f, sizeMultiplier / 7f);

            var projectileSingleTargetImpact = shurikenProjectile.GetComponent<ProjectileSingleTargetImpact>();

            var sphereCollider = shuriken.GetComponent<SphereCollider>();
            sphereCollider.radius = sizeMultiplier / 5f;

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

                var projectileDotZone = shurikenProjectile.AddComponent<ProjectileDotZone>();
                projectileDotZone.damageCoefficient = 1f;
                projectileDotZone.attackerFiltering = AttackerFiltering.NeverHitSelf;
                projectileDotZone.impactEffect = Utils.Paths.GameObject.OmniImpactVFXSlash.Load<GameObject>();
                projectileDotZone.forceVector = new Vector3(0f, 0f, 0f);
                projectileDotZone.overlapProcCoefficient = 1f;
                projectileDotZone.fireFrequency = 30f;
                projectileDotZone.resetFrequency = 10f;
                projectileDotZone.lifetime = -1f;
            }

            if (enablePierce || enableBoomerang)
            {
                GameObject hitBox = new("hitBox");
                hitBox.transform.parent = shuriken.transform;

                // Main.WRBLogger.LogError("HitBox GameObject is " + hitBox);
                // Main.WRBLogger.LogError("static shuriken GameObject is " + shuriken);

                hitBox.AddComponent<HitBox>();
                var hitBoxGroup = shuriken.AddComponent<HitBoxGroup>();
                hitBoxGroup.hitBoxes = new HitBox[] { hitBox.GetComponent<HitBox>() };

                hitBox.transform.localScale = new Vector3(sizeMultiplier, sizeMultiplier, sizeMultiplier);
            }
        }

        public void OnFlyBack()
        {
            if (boomerangProjectile && projectileOverlapAttack && UnityGames != null)
            {
                projectileOverlapAttack.ResetOverlapAttack();
            }
        }
    }
}