using RoR2.Skills;
using RoR2.Projectile;
using UnityEngine;

namespace UltimateCustomRun
{
    public static class VisionsOfHeresy
    {
        public static void VisionsChanges()
        {
            var vs = Resources.Load<SkillDef>("skilldefs/lunarreplacements/lunarprimaryreplacement");
            vs.baseMaxStock = Main.VisionsCharges.Value;
            vs.baseRechargeInterval = Main.VisionsCooldown.Value;
            vs.rechargeStock = Main.VisionsCharges.Value;
            var vp = Resources.Load<GameObject>("prefabs/projectiles/lunarneedleprojectile").GetComponent<ProjectileController>();
            vp.procCoefficient = Main.VisionsInitialProcCoefficient.Value;
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += (orig, self) =>
            {
                EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.damageCoefficient = Main.VisionsInitialHitDamage.Value;
                orig(self);
            };
            // fuck i hate hopo games sometimes
            // also this is unfinished
        }
    }
}
