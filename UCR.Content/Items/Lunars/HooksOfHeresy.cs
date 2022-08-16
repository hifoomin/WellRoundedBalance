using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;

namespace UltimateCustomRun.Items.Lunars
{
    public class HooksOfHeresy : ItemBase
    {
        public static float Cooldown;
        public static int Charges;
        public static int Recharge;
        public static float MinimumDuration;
        public static float ChargeDuration;
        public static float MinimumSpeed;
        public static float MaximumSpeed;
        public static float SelfForce;
        public static float MinimumDamage;
        public static float MaximumDamage;
        public static float ExplosionRadius;
        public static float ExplosionDamage;
        public static float ExplosionProcCoefficient;
        public static float InitialHitDamage;
        public static float InitialHitProcCoefficient;
        public static float FuseTime;
        public override string Name => ":: Items ::::: Lunars :: Hooks of Heresy";
        public override string InternalPickupToken => "lunarSecondaryReplacement";
        public override bool NewPickup => false;
        public override string PickupText => "";

        public override string DescText => "<style=cIsUtility>Replace your Secondary Skill </style> with <style=cIsUtility>Slicing Maelstrom</style>.  \n\nCharge up a projectile that deals <style=cIsDamage>" + d(MinimumDamage * 5f) + "-" + d(MaximumDamage * 5f) + " damage per second</style> to nearby enemies, exploding after <style=cIsUtility>" + FuseTime + "</style> seconds to deal <style=cIsDamage>" + d(MinimumDamage * ExplosionDamage) + "-" + d(MaximumDamage * ExplosionDamage) + " damage</style> and <style=cIsDamage>root</style> enemies for <style=cIsUtility>3</style> <style=cStack>(+3 per stack)</style> seconds. Recharges after " + Cooldown + " <style=cStack>(+" + Cooldown + " per stack)</style> seconds.";

        public override void Init()
        {
            Charges = ConfigOption(1, "Stock Count", "Vanilla is 1");
            Cooldown = ConfigOption(5f, "Cooldown", "Vanilla is 5");
            Recharge = ConfigOption(1, "Stocks to Recharge", "Vanilla is 1");
            MinimumDuration = ConfigOption(0.2f, "Minimum Charge Duration", "Vanilla is 0.2");
            ChargeDuration = ConfigOption(2f, "Maximum Charge Duration", "Vanilla is 2");
            MinimumSpeed = ConfigOption(4f, "Minimum Speed", "Vanilla is 4");
            MaximumSpeed = ConfigOption(80f, "Maximum Speed", "Vanilla is 80");
            SelfForce = ConfigOption(1000f, "Self Knockback", "Vanilla is 1000");
            MinimumDamage = ConfigOption(7f, "Minimum Damage Multiplier", "Based on Charge Duration. Vanilla is 7");
            MaximumDamage = ConfigOption(7f, "Maximum Damage Multiplier", "Based on Charge Duration. Vanilla is 7");
            ExplosionRadius = ConfigOption(14f, "Explosion Range", "Vanilla is 14");
            ExplosionProcCoefficient = ConfigOption(1f, "Explosion Proc Coefficient", "Vanilla is 1");
            ExplosionDamage = ConfigOption(1f, "Explosion Damage Coefficient", "Vanilla is 1\nNote: this is a Multiplier from Minimum Damage and Maximum Damage");
            InitialHitDamage = ConfigOption(0.25f, "Initial Hit Damage Coefficient", "Vanilla is 0.25\nNote: this is a Multiplier from Minimum Damage and Maximum Damage");
            InitialHitProcCoefficient = ConfigOption(0.2f, "Initial Hit Proc Coefficient", "Vanilla is 0.2");
            FuseTime = ConfigOption(3f, "Initial Hit into Explosion Fuse Time", "Vanilla is 3");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            var DLCBalance = LegacyResourcesAPI.Load<SkillDef>("skilldefs/lunarreplacements/LunarSecondaryReplacement");
            DLCBalance.baseRechargeInterval = Cooldown;
            DLCBalance.baseMaxStock = Charges;
            DLCBalance.rechargeStock = Recharge;

            On.EntityStates.Mage.Weapon.BaseChargeBombState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary)
                {
                    self.baseDuration = ChargeDuration;
                    self.minChargeDuration = MinimumDuration;
                }
                orig(self);
            };
            On.EntityStates.Mage.Weapon.BaseThrowBombState.OnEnter += (orig, self) =>
            {
                if (self is EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary)
                {
                    self.minDamageCoefficient = MinimumDamage;
                    self.maxDamageCoefficient = MaximumDamage;
                    self.selfForce = SelfForce;
                }
                orig(self);
            };
            On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.ModifyProjectile += (On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.orig_ModifyProjectile orig, EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary self, ref FireProjectileInfo projectileInfo) =>
            {
                self.minSpeed = MinimumSpeed;
                self.maxSpeed = MaximumSpeed;
                orig(self, ref projectileInfo);
            };

            var zamn = LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/LunarSecondaryProjectile");
            var dooo = zamn.GetComponent<ProjectileExplosion>();
            dooo.blastRadius = ExplosionRadius;
            dooo.blastDamageCoefficient = ExplosionDamage;
            dooo.blastProcCoefficient = ExplosionProcCoefficient;
            var booo = zamn.GetComponent<ProjectileDotZone>();
            booo.damageCoefficient = InitialHitDamage;
            booo.overlapProcCoefficient = InitialHitProcCoefficient;
            zamn.GetComponent<ProjectileFuse>().fuse = FuseTime;
        }
    }
}