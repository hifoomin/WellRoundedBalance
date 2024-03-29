﻿using RoR2.Skills;

namespace WellRoundedBalance.Items.Lunars
{
    public class VisionsOfHeresy : ItemBase<VisionsOfHeresy>
    {
        public static Component[] burdel;
        public static Component[] butla;
        public static Component[] kara;

        public override string Name => ":: Items ::::: Lunars :: Visions of Heresy";
        public override ItemDef InternalPickup => RoR2Content.Items.LunarPrimaryReplacement;

        public override string PickupText => "Replace your Primary Skill with 'Hungering Gaze'.";
        public override string DescText => "<style=cIsUtility>Replace your Primary Skill</style> with <style=cIsUtility>Hungering Gaze</style>. \n\nFire a flurry of <style=cIsUtility>tracking shards</style> that deal <style=cIsDamage>70%</style> damage each, then explode for <style=cIsDamage>140%</style> base damage. Hold up to <style=cIsUtility>5</style> <style=cStack>(+5 per stack)</style> <style=cIsUtility>charges</style> that reload after <style=cIsUtility>1.7</style> <style=cStack>(+1.7 per stack)</style> seconds.";

        public override void Init()
        {
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += FireLunarNeedle_OnEnter;
        }

        private void FireLunarNeedle_OnEnter(On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.orig_OnEnter orig, EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle self)
        {
            EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.baseDuration = 0.1f;
            EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.damageCoefficient = 0.7f;
            EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.recoilAmplitude = 2f;
            EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.spreadBloomValue = 0f;
            EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.maxSpread = 3f;
            EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.fireSound = "Play_MULT_m1_grenade_launcher_shoot";
            orig(self);
        }

        private void Changes()
        {
            Vector3 configsize = new(4f, 4f, 4f);

            var thej = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/LunarNeedleProjectile");
            var p1 = thej.GetComponent<ProjectileImpactExplosion>();
            thej.GetComponent<ProjectileController>().procCoefficient = 1f;
            thej.GetComponent<ProjectileSimple>().desiredForwardSpeed = 70f;
            p1.blastRadius = 6f;
            p1.blastDamageCoefficient = 2f; // multiplier of EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.damageCoefficient = 0.7f;
            p1.blastProcCoefficient = 1f; // multiplier of thej.GetComponent<ProjectileController>().procCoefficient = 1f;

            var olbart = LegacyResourcesAPI.Load<SkillDef>("skilldefs/lunarreplacements/LunarPrimaryReplacement");
            olbart.baseRechargeInterval = 1.7f;
            olbart.baseMaxStock = 5;
            olbart.rechargeStock = 5;

            var a = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/LunarNeedleDamageEffect");
            a.transform.localScale = configsize;

            burdel = a.GetComponentsInChildren<Transform>();

            foreach (Transform kurwa in burdel)
            {
                kurwa.localScale = configsize;
            }

            var b = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/LunarNeedleExplosionEffect");
            b.GetComponent<EffectComponent>().soundName = "Play_voidRaid_m1_explode";
            b.transform.localScale = configsize;

            butla = b.GetComponentsInChildren<Transform>();

            foreach (Transform stary in butla)
            {
                stary.localScale = configsize;
            }

            var c = LegacyResourcesAPI.Load<GameObject>("prefabs/projectileghosts/LunarNeedleGhost");
            c.transform.localScale = configsize;

            kara = c.GetComponentsInChildren<Transform>();

            foreach (Transform szezdziesiona in kara)
            {
                szezdziesiona.localScale = configsize;
            }
        }
    }
}