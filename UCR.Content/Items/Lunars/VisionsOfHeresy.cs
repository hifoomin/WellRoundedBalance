using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;

namespace UltimateCustomRun.Items.Lunars
{
    public class VisionsOfHeresy : ItemBase
    {
        public static int Charges;
        public static float Cooldown;
        public static float Duration;
        public static float InitialHitDamage;
        public static float InitialHitProcCoefficient;
        public static float Recoil;
        public static float Bloom;
        public static float MaxSpeed;
        public static float ProjectileSpeed;
        public static float ExplosionDamage;
        public static float ExplosionRadius;
        public static float ExplosionProcCoefficient;
        public static float Size;
        public static bool EnableTheFeelTarlukNailgunCopeSeethePlusTwoAoeEffect;
        public static Component[] burdel;
        public static Component[] butla;
        public static Component[] kara;

        public override string Name => ":: Items ::::: Lunars :: Visions of Heresy";
        public override string InternalPickupToken => "lunarPrimaryReplacement";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>Replace your Primary Skill</style> with <style=cIsUtility>Hungering Gaze</style>. \n\nFire a flurry of <style=cIsUtility>tracking shards</style> that detonate after a delay, dealing <style=cIsDamage>" + d(ExplosionDamage) + "</style> base Damage. Hold up to " + Charges + " Charges <style=cStack>(+" + Charges + " per stack)</style> that reload after " + Cooldown + " seconds <style=cStack>(+" + Cooldown + " per stack)</style>.";

        public override void Init()
        {
            Charges = ConfigOption(12, "Stock Count", "Vanilla is 12.\nRecommended Value: 5");
            Cooldown = ConfigOption(2f, "Cooldown", "Vanilla is 2.\nRecommended Value: 1.7");
            Duration = ConfigOption(0.11f, "Duration Per Shot", "Vanilla is 0.11.\nRecommended Value: 0.1");
            InitialHitDamage = ConfigOption(0.05f, "Damage Coeffcient of each Initial Hit", "Decimal. Vanilla is 0.05.\nRecommended Value: 0.75");
            InitialHitProcCoefficient = ConfigOption(0.1f, "Proc Coefficient of each Initial Hit", "Vanilla is 0.1.\nRecommended Value: 0.1");
            Recoil = ConfigOption(1.5f, "Recoil", "Vanilla is 1.5.\nRecommended Value: 4");
            Bloom = ConfigOption(0.4f, "Spread", "Vanilla is 0.4.\nRecommended Value: 0");
            MaxSpeed = ConfigOption(3f, "Maximum Spread", "Vanilla is 3.\nRecommended Value: 0");
            ProjectileSpeed = ConfigOption(40f, "Projectile Speed", "Vanilla is 40.\nRecommended Value: 70");
            ExplosionDamage = ConfigOption(24f, "Explosion Damage", "Vanilla is 24.\nRecommended Value: 2\nNote: This is a multiplier of Initial Hit's Damage");
            ExplosionRadius = ConfigOption(2f, "Explosion Range", "Vanilla is 2.\nRecommended Value: 6");
            ExplosionProcCoefficient = ConfigOption(10f, "Explosion Proc Coefficient", "Vanilla is 10.\nRecommended Value: 10\nNote: This is a multiplier of Initial Hit's Proc Coefficient");
            Size = ConfigOption(1f, "Visual Size", "Vanilla is 1.\nRecommended Value: 7");
            EnableTheFeelTarlukNailgunCopeSeethePlusTwoAoeEffect = ConfigOption(false, "Improve Visuals and Sound?", "Vanilla is false.\nRecommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Change();
        }

        public static void Change()
        {
            Vector3 configsize = new Vector3(Size, Size, Size);
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += (orig, self) =>
            {
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.baseDuration = Duration;
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.damageCoefficient = InitialHitDamage;
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.recoilAmplitude = Recoil;
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.spreadBloomValue = Bloom;
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.maxSpread = MaxSpeed;
                orig(self);
            };

            var thej = Resources.Load<GameObject>("prefabs/projectiles/LunarNeedleProjectile");
            var p1 = thej.GetComponent<ProjectileImpactExplosion>();
            var p2 = thej.GetComponent<ProjectileDirectionalTargetFinder>();
            thej.GetComponent<ProjectileController>().procCoefficient = InitialHitProcCoefficient;
            thej.GetComponent<ProjectileSimple>().desiredForwardSpeed = ProjectileSpeed;
            p1.blastRadius = ExplosionRadius;
            p1.blastDamageCoefficient = ExplosionDamage;
            p1.blastProcCoefficient = ExplosionProcCoefficient;

            var olbart = Resources.Load<SkillDef>("skilldefs/lunarreplacements/LunarPrimaryReplacement");
            olbart.baseRechargeInterval = Cooldown;
            olbart.baseMaxStock = Charges;
            olbart.rechargeStock = Charges;
            olbart.fullRestockOnAssign = true;

            if (EnableTheFeelTarlukNailgunCopeSeethePlusTwoAoeEffect)
            {
                On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += (orig, self) =>
                {
                    orig(self);
                    EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.fireSound = "Play_MULT_m1_grenade_launcher_shoot";
                };

                var a = Resources.Load<GameObject>("prefabs/effects/impacteffects/LunarNeedleDamageEffect");
                a.transform.localScale = configsize;

                burdel = a.GetComponentsInChildren<Transform>();

                foreach (Transform kurwa in burdel)
                {
                    kurwa.localScale = configsize;
                }

                var b = Resources.Load<GameObject>("prefabs/effects/impacteffects/LunarNeedleExplosionEffect");
                b.GetComponent<EffectComponent>().soundName = "Play_imp_overlord_attack1_throw";
                b.transform.localScale = configsize;

                butla = b.GetComponentsInChildren<Transform>();

                foreach (Transform stary in butla)
                {
                    stary.localScale = configsize;
                }

                var c = Resources.Load<GameObject>("prefabs/projectileghosts/LunarNeedleGhost");
                c.transform.localScale = configsize;

                kara = c.GetComponentsInChildren<Transform>();

                foreach (Transform szezdziesiona in kara)
                {
                    szezdziesiona.localScale = configsize;
                }

                // TODO: Change colors?
                // not sure how to do it in the clean way tho, instead of getting all the children individually
            }
        }
    }
}