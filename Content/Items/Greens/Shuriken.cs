using MonoMod.Cil;
using System;

namespace WellRoundedBalance.Items.Greens
{
    public class Shuriken : ItemBase
    {
        public override string Name => ":: Items :: Greens :: Shuriken";
        public override string InternalPickupToken => "primarySkillShuriken";

        public override string PickupText => "Activating your Primary skill also throws a shuriken. Recharges over time.";

        public override string DescText => "Activating your <style=cIsUtility>Primary skill</style> also throws a <style=cIsDamage>shuriken</style> that deals <style=cIsDamage>400%</style> <style=cStack>(+200% per stack)</style> base damage. The <style=cIsDamage>shuriken</style> reloads over <style=cIsUtility>10</style> seconds.";

        [ConfigField("Base Damage", "Decimal.", 4f)]
        public static float baseDamage;

        [ConfigField("Damage Per Stack", "Decimal.", 2f)]
        public static float damagePerStack;

        [ConfigField("Projectile Lifetime", "", 10f)]
        public static float projectileLifetime;

        [ConfigField("Projectile Speed", "", 100f)]
        public static float projectileSpeed;

        [ConfigField("Enable Boomerang?", "", true)]
        public static bool enableBoomerang;

        [ConfigField("Enable Pierce?", "", true)]
        public static bool enablePierce;

        [ConfigField("Enable Homing?", "", false)]
        public static bool enableHoming;

        [ConfigField("Size Multiplier", "", 1f)]
        public static float sizeMultiplier;

        [ConfigField("Distance Multiplier", "", 0.4f)]
        public static float distanceMultiplier;

        public float timer;

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.RoR2.PrimarySkillShurikenBehavior.FixedUpdate += PrimarySkillShurikenBehavior_FixedUpdate;
            IL.RoR2.PrimarySkillShurikenBehavior.FixedUpdate += PrimarySkillShurikenBehavior_FixedUpdate1;
            IL.RoR2.PrimarySkillShurikenBehavior.FireShuriken += PrimarySkillShurikenBehavior_FireShuriken;
            On.RoR2.PrimarySkillShurikenBehavior.GetRandomRollPitch += PrimarySkillShurikenBehavior_GetRandomRollPitch;
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
                c.Index += 1;
                // stack => 1
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

        private void Changes()
        {
            var shurikenProjectile = Utils.Paths.GameObject.ShurikenProjectile.Load<GameObject>();
            var projectileSimple = shurikenProjectile.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = projectileLifetime;
            projectileSimple.desiredForwardSpeed = projectileSpeed;

            shurikenProjectile.transform.localScale *= sizeMultiplier * 0.2f;

            var projectileSingleTargetImpact = shurikenProjectile.GetComponent<ProjectileSingleTargetImpact>();

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
                var boomerangProjectile = shurikenProjectile.AddComponent<BoomerangProjectile>();
                boomerangProjectile.travelSpeed = projectileSpeed;
                boomerangProjectile.charge = 1f;
                boomerangProjectile.transitionDuration = projectileLifetime / 2f;
                boomerangProjectile.canHitCharacters = false;
                boomerangProjectile.canHitWorld = false;
                boomerangProjectile.distanceMultiplier = 1f;

                projectileSingleTargetImpact.enabled = false;
            }

            if (enablePierce)
            {
                var projectileOverlapAttack = shurikenProjectile.AddComponent<ProjectileOverlapAttack>();
                projectileOverlapAttack.damageCoefficient = 1f;
                projectileOverlapAttack.impactEffect = null;
                projectileOverlapAttack.forceVector = new Vector3(0f, 0f, 0f);
                projectileOverlapAttack.fireFrequency = 60f;
                projectileOverlapAttack.resetInterval = 60f;

                projectileSingleTargetImpact.enabled = false;
            }
        }
    }
}