using RoR2.Projectile;
using UnityEngine;
using RoR2.Skills;
using RoR2;

namespace UltimateCustomRun
{
    public class VisionsOfHeresy : ItemBase
    {
        public static int charges;
        public static float cd;
        public static float dur;
        public static float idmg;
        public static float iproc;
        public static float recoil;
        public static float bloom;
        public static float maxs;
        public static float pspeed;
        public static float edmg;
        public static float eaoe;
        public static float eproc;
        public static float size;
        public static bool EnableTheFeelTarlukNailgunCopeSeethePlusTwoAoeEffect;
        public static Component[] burdel;
        public static Component[] butla;
        public static Component[] kara;

        public override string Name => ":: Items ::::: Lunars :: Visions of Heresy";
        public override string InternalPickupToken => "lunarPrimaryReplacement";
        public override bool NewPickup => false;
        public override string PickupText => "";
        public override string DescText => "<style=cIsUtility>Replace your Primary Skill</style> with <style=cIsUtility>Hungering Gaze</style>. \n\nFire a flurry of <style=cIsUtility>tracking shards</style> that detonate after a delay, dealing <style=cIsDamage>" + d(edmg) + "</style> base damage. Hold up to " + charges + " charges <style=cStack>(+" + charges + " per stack)</style> that reload after " + cd + " seconds <style=cStack>(+" + cd + " per stack)</style>.";


        public override void Init()
        {
            charges = ConfigOption(12, "Stock Count", "Vanilla is 12. Recommended Value: 5");
            cd = ConfigOption(2f, "Cooldown", "Vanilla is 2. Recommended Value: 1.7");
            dur = ConfigOption(0.11f, "Duration Per Shot", "Vanilla is 0.11. Recommended Value: 0.1");
            idmg = ConfigOption(0.05f, "Damage Coeffcient of each Initial Hit", "Decimal. Vanilla is 0.05. Recommended Value: 0.75");
            iproc = ConfigOption(0.1f, "Proc Coefficient of each Initial Hit", "Vanilla is 0.1. Recommended Value: 0.1");
            recoil = ConfigOption(1.5f, "Recoil", "Vanilla is 1.5. Recommended Value: 4");
            bloom = ConfigOption(0.4f, "Spread", "Vanilla is 0.4. Recommended Value: 0");
            maxs = ConfigOption(3f, "Maximum Spread", "Vanilla is 3. Recommended Value: 0");
            pspeed = ConfigOption(40f, "Projectile Speed", "Vanilla is 40. Recommended Value: 70");
            edmg = ConfigOption(24f, "Explosion Damage", "Vanilla is 24. Recommended Value: 2\nNote: This is a multiplier of Initial Hit's Damage");
            eaoe = ConfigOption(2f, "Explosion Range", "Vanilla is 2. Recommended Value: 6");
            eproc = ConfigOption(10f, "Explosion Proc Coefficient", "Vanilla is 10. Recommended Value: 10\nNote: This is a multiplier of Initial Hit's Proc Coefficient");
            size = ConfigOption(1f, "Visual Size", "Vanilla is 1. Recommended Value: 7");
            EnableTheFeelTarlukNailgunCopeSeethePlusTwoAoeEffect = ConfigOption(false, "Improve Visuals and Sound?", "Vanilla is false. Recommended Value: true");
            base.Init();
        }

        public override void Hooks()
        {
            Change();
        }

        public static void Change()
        {
            Vector3 configsize = new Vector3(size, size, size);
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += (orig, self) =>
            {
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.baseDuration = dur;
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.damageCoefficient = idmg;
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.recoilAmplitude = recoil;
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.spreadBloomValue = bloom;
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.maxSpread = maxs;
                orig(self);
            };
            
            var thej = Resources.Load<GameObject>("prefabs/projectiles/LunarNeedleProjectile");
            var p1 = thej.GetComponent<ProjectileImpactExplosion>();
            var p2 = thej.GetComponent<ProjectileDirectionalTargetFinder>();
            thej.GetComponent<ProjectileController>().procCoefficient = iproc;
            thej.GetComponent<ProjectileSimple>().desiredForwardSpeed = pspeed;
            p1.blastRadius = eaoe;
            p1.blastDamageCoefficient = edmg;
            p1.blastProcCoefficient = eproc;

            var olbart = Resources.Load<SkillDef>("skilldefs/lunarreplacements/LunarPrimaryReplacement");
            olbart.baseRechargeInterval = cd;
            olbart.baseMaxStock = charges;
            olbart.rechargeStock = charges;
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
