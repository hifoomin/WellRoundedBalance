using RoR2.Skills;
using RoR2.Projectile;
using UnityEngine;
using System.Linq;

namespace UltimateCustomRun
{
    public class HooksOfHeresy : ItemBase
    {
        public static float cd;
        public static int charges;
        public static int recharge;
        public static float mindur;
        public static float chargedur;
        public static float minspd;
        public static float maxspd;
        public static float selffrc;
        public static float mindmg;
        public static float maxdmg;
        public static float eaoe;
        public static float edmg;
        public static float eproc;
        public static float rdmg;
        public static float rproc;
        public static float fuse;
        public override string Name => ":: Items ::::: Lunars :: Hooks of Heresy";
        public override string InternalPickupToken => "lunarSecondaryReplacement";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>Replace your Secondary Skill </style> with <style=cIsUtility>Slicing Maelstrom</style>.  \n\nCharge up a projectile that deals <style=cIsDamage>" + d(mindmg) + "-" + d(maxdmg) + " damage per second</style> to nearby enemies, exploding after <style=cIsUtility>" + fuse + "</style> seconds to deal <style=cIsDamage>" + d(mindmg * edmg) + "-" + d(maxdmg * edmg) + " damage</style> and <style=cIsDamage>root</style> enemies for <style=cIsUtility>3</style> <style=cStack>(+3 per stack)</style> seconds. Recharges after " + cd + " <style=cStack>(+" + cd + " per stack)</style> seconds.";
        
        public override void Init()
        {
            charges = ConfigOption(1, "Stock Count", "Vanilla is 1. Recommended Value: 1");
            cd = ConfigOption(5f, "Cooldown", "Vanilla is 5. Recommended Value: 5");
            recharge = ConfigOption(1, "Stocks to Recharge", "Vanilla is 1. Recommended Value: 1");
            mindur = ConfigOption(0.2f, "Minimum Charge Duration", "Vanilla is 0.2. Recommended Value: 0");
            chargedur = ConfigOption(2f, "Maximum Charge Duration", "Vanilla is 2. Recommended Value: 1");
            minspd = ConfigOption(4f, "Minimum Speed", "Vanilla is 4. Recommended Value: 5");
            maxspd = ConfigOption(80f, "Maximum Speed", "Vanilla is 80. Recommended Value: 5");
            selffrc = ConfigOption(1000f, "Self Knockback", "Vanilla is 1000. Recommended Value: 2500");
            mindmg = ConfigOption(7f, "Minimum Damage Multiplier", "Based on Charge Duration. Vanilla is 7. Recommended Value: 1");
            maxdmg = ConfigOption(7f, "Maximum Damage Multiplier", "Based on Charge Duration. Vanilla is 7. Recommended Value: 4.5");
            eaoe = ConfigOption(14f, "Explosion Range", "Vanilla is 14. Recommended Value: 14");
            eproc = ConfigOption(1f, "Explosion Proc Coefficient", "Vanilla is 1. Recommended Value: 1");
            edmg = ConfigOption(1f, "Explosion Damage Coefficient", "Vanilla is 1. Recommended Value: 1\nNote: this is a Multiplier from Minimum Damage and Maximum Damage");
            rdmg = ConfigOption(0.25f, "Initial Hit Damage Coefficient", "Vanilla is 0.25. Recommended Value: 0.25\nNote: this is a Multiplier from Minimum Damage and Maximum Damage");
            rproc = ConfigOption(0.2f, "Initial Hit Proc Coefficient", "Vanilla is 0.2. Recommended Value: 0.2");
            fuse = ConfigOption(3f, "Initial Hit into Explosion Fuse Time", "Vanilla is 3. Recommended Value: 3");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var DLCBalance = Resources.Load<SkillDef>("skilldefs/lunarreplacements/LunarSecondaryReplacement");
            DLCBalance.baseRechargeInterval = cd;
            DLCBalance.baseMaxStock = charges;
            DLCBalance.rechargeStock = recharge;

            On.EntityStates.Mage.Weapon.BaseChargeBombState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary)
                {
                    self.baseDuration = chargedur;
                    self.minChargeDuration = mindur;
                    orig(self);
                }
            };
            On.EntityStates.Mage.Weapon.BaseThrowBombState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary)
                {
                    self.minDamageCoefficient = mindmg;
                    self.maxDamageCoefficient = maxdmg;
                    self.selfForce = selffrc;
                    orig(self);
                }
            };
            On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.ModifyProjectile += (On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.orig_ModifyProjectile orig, EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary self, ref FireProjectileInfo projectileInfo) =>
            {
                self.minSpeed = minspd;
                self.maxSpeed = maxspd;
                orig(self, ref projectileInfo);
            };

            var zamn = Resources.Load<GameObject>("prefabs/projectiles/LunarSecondaryProjectile");
            var dooo = zamn.GetComponent<ProjectileExplosion>();
            dooo.blastRadius = eaoe;
            dooo.blastDamageCoefficient = edmg;
            dooo.blastProcCoefficient = eproc;
            var booo = zamn.GetComponent<ProjectileDotZone>();
            booo.damageCoefficient = rdmg;
            booo.overlapProcCoefficient = rproc;
            zamn.GetComponent<ProjectileFuse>().fuse = fuse;
        }
    }
}
