using RoR2.Skills;
using RoR2.Projectile;
using UnityEngine;

namespace UltimateCustomRun
{
    static class HooksOfHeresy
    {
        public static void HooksChanges()
        {
            var hs = Resources.Load<SkillDef>("skilldefs/lunarreplacements/lunarsecondaryreplacement");
            hs.baseMaxStock = Main.HooksCharges.Value;
            hs.baseRechargeInterval = Main.HooksCooldown.Value;
            hs.rechargeStock = Main.HooksCharges.Value;
            var he1 = Resources.Load<GameObject>("prefabs/projectiles/lunarsecondaryprojectile").GetComponent<ProjectileController>();
            var he2 = Resources.Load<GameObject>("prefabs/projectiles/lunarsecondaryprojectile").GetComponent<ProjectileExplosion>();
            he1.procCoefficient = Main.HooksExplosionProcCoefficient.Value;
            he2.blastRadius = Main.HooksExplosionRadius.Value;
            var hs2 = Resources.Load<GameObject>("prefabs/projectiles/lunarsecondaryprojectile").GetComponent<ProjectileDotZone>();
            hs2.overlapProcCoefficient = Main.HooksProcCoefficient.Value;
            // unfinished
        }
    }
}
